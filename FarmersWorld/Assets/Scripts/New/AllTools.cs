using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AllTools : MonoBehaviour
{
    public static AllTools instance {get; private set;}

    [Header("Layout Settings")]
    [SerializeField] private float itemSpacing = .5f;
    private float itemHeight;

    [Header("UI elements")]
    [SerializeField] private Image selectedCharacterIcon;
    [SerializeField] private Transform ShopItemsContainer;
    [SerializeField] private GameObject itemPrefab;
    [Space(20)]
    [SerializeField] private CharacterShopDatabase characterDB;

    private int newSelectedItemIndex = 0;
    private int previousSelectedItemIndex = 0;

    private ResourceBank resourceBank;
    private CharacterItemUI characterItemUI;

    private void Awake()
    {
        instance = this;
        resourceBank = ResourceBank.instance;
    }

    private void Start()
    {
        GenerateShopItemsUI();

        SetSelectedCharacter();

        SelectItemUI(GameDataManager.GetSelectedCharacterIndex());

        ChangePlayerSkin();

        foreach (Tool character in characterDB.characters)
        {
            character.button.onClick.AddListener(() =>
            {
                OnCharacterButtonClicked(character);

                UpdateCharacterDurabilityUI(character);
            });

            character.repairButton.onClick.AddListener(() =>
            {
                Repair(character);
            });
        }
    }

    private void Update()
    {
        foreach (Tool character in characterDB.characters)
        {
            UpdateCharacterDurabilityUI(character);
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
            UpdateCharacterDurabilityUI(tool);

            Debug.Log("Tool repaired!");

            // mainMenuCharacterDurabilityConsumed.text = tool.durability.ToString();
            // float fillAmount = (float)tool.durability / tool.durabilityMax;
            // mainMenuCharacterEnergy.DOFillAmount(fillAmount, 0.5f);
            // mainMenuCharacterEnergyText.text = tool.durability + "/" + tool.durabilityMax;
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

    int index = character.ID;
    CharacterItemUI uiItem = GetItemUI(index);
    if (uiItem != null)
    {
        uiItem.SetCharacterDurability(character.durability, character.durabilityMax);
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

           // ShopItemsContainer.GetComponent<RectTransform>().sizeDelta = Vector2.up * ((itemHeight + itemSpacing) * characterDB.CharactersCount + itemSpacing);
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
        }

        if (selectedCharacter.image != null)
        {
            selectedCharacterIcon.sprite = selectedCharacter.image;
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
    if (index >= 0 && index < ShopItemsContainer.childCount)
    {
        return ShopItemsContainer.GetChild(index).GetComponent<CharacterItemUI>();
    }
    else
    {
        Debug.LogWarning("Invalid item index: " + index);
        return null;
    }
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
