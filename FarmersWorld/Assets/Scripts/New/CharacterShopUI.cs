using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CharacterShopUI : MonoBehaviour
{
    public static CharacterShopUI instance {get; private set;}

    [Header("Layout Settings")]
    [SerializeField] private float itemSpacing = .5f;
    private float itemHeight;

    [Header("UI elements")]
    [SerializeField] private Image selectedCharacterIcon;
    [SerializeField] private Transform ShopItemsContainer;
    [SerializeField] private GameObject itemPrefab;
    [Space(20)]
    [SerializeField] private CharacterShopDatabase characterDB;

    [Space(20)]
    [Header("Main Menu")]
    [SerializeField] private Image mainMenuCharacterImage;
    [SerializeField] private TMP_Text mainMenuCharacterName;
    [SerializeField] private TMP_Text mainMenuCharacterLevel;
    [SerializeField] private TMP_Text mainMenuCharacterRewardRate;
    [SerializeField] private TMP_Text mainMenuCharacterChargeTime;
    [SerializeField] private TMP_Text mainMenuCharacterEnergyConsumed;
    [SerializeField] private TMP_Text mainMenuCharacterDurabilityConsumed;
    [SerializeField] private TMP_Text mainMenuCharacterNumber;

    [SerializeField] private Image mainMenuCharacterEnergy;
    [SerializeField] private TMP_Text mainMenuCharacterEnergyText;

    [SerializeField] private Transform foodTransform;
    [SerializeField] private Transform goldTransform;
    [SerializeField] private Transform woodTransform;

    private int newSelectedItemIndex = 0;
    private int previousSelectedItemIndex = 0;

    private CharacterItemUI newItemUI;

    private GameObject newItem;
    private Dictionary<int, GameObject> objectLinks = new Dictionary<int, GameObject>();

    private ResourceBank resourceBank;
    private Timer timer;

    
    private void Awake()
    {
        instance = this;
        resourceBank = ResourceBank.instance;
    }

private void Start()
{
    // if (newItemUI != null)
    // {
    //     newItemUI.SetRemoveButtonClickAction(index =>
    //     {
    //         DestroyNewItem(index);
    //     });
    // }

    GenerateShopItemsUI();

    SetSelectedCharacter();

    SelectItemUI(GameDataManager.GetSelectedCharacterIndex());

    ChangePlayerSkin();

    foreach (Tool character in characterDB.characters)
    {
        character.button.onClick.AddListener(() =>
        {
            CreateNewItem(character, character.ID);
            OnCharacterButtonClicked(character);
            UpdateCharacterDurabilityUI(character);
        });

        character.repairButton.onClick.AddListener(() =>
        {
            Repair(character);
        });
    }
}



    private void OnCharacterButtonClicked(Tool tool)
    {
        tool.button.interactable = false;
        tool.timer
            .SetDuration(tool.timerDuration)
            .OnEnd(() =>
            {
                tool.button.interactable = true;

                if (tool.durability <= 0)
                {
                    tool.button.interactable = false;
                }
                CollectResources(tool);

                ResetTimer(tool);
                
            })
            .Begin();
    }


     
    public void Repair(Tool tool)
    {
        int durabilityCost = (int)(tool.durabilityMax - tool.durability);
        int goldCost = durabilityCost / 5;

        // Check if the player can afford the repair cost
        if (ResourceBank.instance.CanAfford(0, 0, goldCost, 0))
        {
            // Deduct the repair cost from the player's resources
            ResourceBank.instance.SpendResources(0, 0, goldCost, 0);

            // Replenish the durability to its maximum value
            tool.durability = (int)tool.durabilityMax;

            // Update the tool's durability UI or perform any other necessary actions
            // ...
            tool.button.interactable = true;

            Debug.Log("Tool repaired!");

            mainMenuCharacterDurabilityConsumed.text = tool.durability.ToString();
            float fillAmount = (float)tool.durability / tool.durabilityMax;
            mainMenuCharacterEnergy.DOFillAmount(fillAmount, 0.5f);
            mainMenuCharacterEnergyText.text = tool.durability + "/" + tool.durabilityMax;
        }
        else
        {
            Debug.Log("Insufficient resources to repair the tool!");
        }
    }




    private void UpdateCharacterDurabilityUI(Tool character)
{
    if (character.durability <= 0)
    {
        Debug.Log("Character durability depleted!");
        return;
    }

    character.SubtractDurability(character.reducedurability);
    resourceBank.SpendResources(0, 0, 0, character.energyСonsumption);

    mainMenuCharacterDurabilityConsumed.text = character.durability.ToString();
    float fillAmount = (float)character.durability / character.durabilityMax;
    mainMenuCharacterEnergy.DOFillAmount(fillAmount, 0.5f);
    mainMenuCharacterEnergyText.text = character.durability + "/" + character.durabilityMax;
}


    private void CollectResources(Tool character)
    {
        switch (character.resourceType)
        {
            case Tool.ResourceType.Food:
                CollectFood(character.foodMining);
                break;
            case Tool.ResourceType.Wood:
                CollectWood(character.woodMining);
                break;
            case Tool.ResourceType.Gold:
                CollectGold(character.goldMining);
                break;
            default:
                Debug.LogError("Invalid resourceType: " + character.resourceType);
                break;
        }
    }

    private void CollectFood(int amount)
    {
        resourceBank.AddFood(amount);
        Debug.Log("Collected " + amount + " Food");
    }

    private void CollectWood(int amount)
    {
        resourceBank.AddWood(amount);
        Debug.Log("Collected " + amount + " Wood");
    }

    private void CollectGold(int amount)
    {
        resourceBank.AddGold(amount);
        Debug.Log("Collected " + amount + " Gold");
    }


    public void StartTimer(Tool tool)
    {
        timer = tool.timer.GetComponent<Timer>();
        if (timer != null)
        {
            timer.StartTimer(tool);
        }
        else
        {
            Debug.LogError("Timer component not found on the tool object.");
        }
    }


    public void ResetTimer(Tool tool)
    {
        timer = tool.timer.GetComponent<Timer>();
        if (timer != null)
        {
            timer.ResetTimer(tool);
        }
        else
        {
            Debug.LogError("Timer component not found on the tool object.");
        }
    }

    private void SetSelectedCharacter()
    {
        int index = GameDataManager.GetSelectedCharacterIndex();
        GameDataManager.SetSelectedCharacter(characterDB.GetCharacter(index), index);
    }

    private void GenerateShopItemsUI()
    {
        for (int i = 0; i < GameDataManager.GetAllPurchasedCharacter().Count; i++)
        {
            int purchasedCharacterIndex = GameDataManager.GetPurchasedCharacter(i);
            characterDB.PurchaseCharacter(purchasedCharacterIndex);
        }

        itemHeight = ShopItemsContainer.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
        Destroy(ShopItemsContainer.GetChild(0).gameObject);
        ShopItemsContainer.DetachChildren();

        for (int i = 0; i < characterDB.CharactersCount; i++)
        {
            Tool character = characterDB.GetCharacter(i);
            CharacterItemUI uiItem = Instantiate(itemPrefab, ShopItemsContainer).GetComponent<CharacterItemUI>();
            uiItem.SetItemPosition(Vector2.down * i * (itemHeight + itemSpacing));
            uiItem.gameObject.name = "Item" + i + "-" + character.name;
            uiItem.SetCharacterName(character.name);
            uiItem.SetCharacterImage(character.image);
            // uiItem.SetCharacterSpeed(character.resourceAmount);
            // uiItem.SetCharacterPrice(character.energyCost);
            uiItem.SetCharacterDurability(character.durability, character.durabilityMax);

            if (character.isPurchased)
            {
                uiItem.OnItemSelect(i, OnItemSelected);
            }

            ShopItemsContainer.GetComponent<RectTransform>().sizeDelta = Vector2.up * ((itemHeight + itemSpacing) * characterDB.CharactersCount + itemSpacing);
        }
    }

    private void ChangePlayerSkin()
{
    Tool selectedCharacter = GameDataManager.GetSelectedCharacter();

    foreach (Tool character in characterDB.characters)
    {
        bool isSelectedCharacter = character.ID == selectedCharacter.ID;
        character.button.gameObject.SetActive(isSelectedCharacter);
        character.repairButton.gameObject.SetActive(isSelectedCharacter);
        if (isSelectedCharacter)
        {
            character.timer.gameObject.transform.localScale = Vector3.one;
        }
        else
        {
            character.timer.gameObject.transform.localScale = Vector3.zero;
        }
        StartTimer(selectedCharacter);
    }

    if (selectedCharacter.image != null)
    {
        mainMenuCharacterImage.sprite = selectedCharacter.image;
        mainMenuCharacterName.text = selectedCharacter.name;
        mainMenuCharacterLevel.text = selectedCharacter.level.ToString();
        mainMenuCharacterRewardRate.text = selectedCharacter.rewardRate.ToString();
        mainMenuCharacterChargeTime.text = selectedCharacter.chargeTime.ToString() + " mins";
        mainMenuCharacterEnergyConsumed.text = selectedCharacter.energyConsumed.ToString();
        mainMenuCharacterDurabilityConsumed.text = selectedCharacter.durabilityConsumed.ToString();
        mainMenuCharacterNumber.text = (selectedCharacter.ID + "/" + characterDB.CharactersCount).ToString();

        // Animate the fill amount of mainMenuCharacterEnergy
        float fillAmount = selectedCharacter.durability / selectedCharacter.durabilityMax;
        mainMenuCharacterEnergy.DOFillAmount(fillAmount, 0.5f);

        mainMenuCharacterEnergyText.text = selectedCharacter.durability + "/" + selectedCharacter.durabilityMax;
        selectedCharacterIcon.sprite = selectedCharacter.image;
    }
}


    private void CreateNewItem(Tool character, int id)
{
    newItem = Instantiate(character.prefab);
    newItemUI = newItem.GetComponent<CharacterItemUI>();
    newItemUI.SetCharacterName(character.name);
    newItemUI.SetCharacterImage(character.image);


    objectLinks.Add(id, newItem);

    Transform targetTransform = GetTargetTransform(character.resourceType);
    if (targetTransform != null)
    {
        RectTransform newItemTransform = newItem.GetComponent<RectTransform>();
        newItemTransform.SetParent(targetTransform, false);
        newItemTransform.localScale = Vector3.one;
        newItemTransform.anchoredPosition = new Vector2(15f, -15f);

        Vector2 offset = new Vector2(15f, -15f);
        offset *= targetTransform.childCount - 1;
        newItemTransform.anchoredPosition += offset;
    }

    newItem.transform.localScale = Vector3.zero;
    newItem.transform.DOScale(1f, 0.5f);
    DestroyNewItemAfterDelay(character);
}


public void DestroyNewItem(int id)
{
    if (objectLinks.ContainsKey(id))
    {
        // Retrieve the GameObject associated with the link
        GameObject objectToDelete = objectLinks[id];

        // Get the CharacterItemUI component from the object
        CharacterItemUI newItemUI = objectToDelete.GetComponent<CharacterItemUI>();

        // Call the SetIndex method on newItemUI
        newItemUI.SetIndex(id);

        // Destroy the GameObject
        Destroy(objectToDelete);

        // Remove the link from the dictionary
        objectLinks.Remove(id);
    }
}






private void DestroyNewItemAfterDelay(Tool tool)
{
    if (newItem != null)
    {
        Destroy(newItem, tool.timerDuration);
    }
}


    private Transform GetTargetTransform(Tool.ResourceType resourceType)
    {
        switch (resourceType)
        {
            case Tool.ResourceType.Food:
                return foodTransform;
            case Tool.ResourceType.Wood:
                return woodTransform;
            case Tool.ResourceType.Gold:
                return goldTransform;
            default:
                Debug.LogError("Invalid resourceType: " + resourceType);
                return null;
        }
    }

    private void OnItemSelected(int index)
    {
        SelectItemUI(index);
        GameDataManager.SetSelectedCharacter(characterDB.GetCharacter(index), index);
        ChangePlayerSkin();
    }

    private void SelectItemUI(int itemIndex)
    {
        previousSelectedItemIndex = newSelectedItemIndex;
        newSelectedItemIndex = itemIndex;

        CharacterItemUI prevUiItem = GetItemUI(previousSelectedItemIndex);
        CharacterItemUI newUiItem = GetItemUI(newSelectedItemIndex);

        prevUiItem.DeselectItem();
        newUiItem.SelectItem();
    }

    private CharacterItemUI GetItemUI(int index)
    {
        return ShopItemsContainer.GetChild(index).GetComponent<CharacterItemUI>();
    }

    private void OnItemPurchased(int index)
    {
        Tool character = characterDB.GetCharacter(index);
        CharacterItemUI uiItem = GetItemUI(index);

        if (GameDataManager.CanSpendCoins(character.price))
        {
            GameDataManager.SpendCoins(character.price);
            characterDB.PurchaseCharacter(index);
            uiItem.OnItemSelect(index, OnItemSelected);
            GameDataManager.AddPurchasedCharacter(index);
        }
        else
        {
            uiItem.AnimateShakeItem();
        }
    }
}
