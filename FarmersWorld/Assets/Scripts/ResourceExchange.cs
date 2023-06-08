using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceExchange : MonoBehaviour
{
    public ResourceBank resourceBank;
    public TMP_InputField foodCostInputField;
    private int foodCost = 0; 

    [SerializeField] private Button increaseFoodCostButton, decreaseFoodCostButton;

    [SerializeField] private Button openExchangePanelButton, closeExchangePanelButton;

    [SerializeField] private TMP_Text foodExchange, energyGet;
    [SerializeField] private GameObject exchangePanel;

    private void OnEnable()
    {
        increaseFoodCostButton.onClick.AddListener(IncreaseFoodCost);
        decreaseFoodCostButton.onClick.AddListener(DecreaseFoodCost);
        openExchangePanelButton.onClick.AddListener(OpenExchangePanel);
        closeExchangePanelButton.onClick.AddListener(CloseExchangePanel);
    }

    private void OnDisable()
    {
        increaseFoodCostButton.onClick.RemoveListener(IncreaseFoodCost);
        decreaseFoodCostButton.onClick.RemoveListener(DecreaseFoodCost);
        closeExchangePanelButton.onClick.RemoveListener(CloseExchangePanel);
    }

    public void IncreaseFoodCost()
    {
        if (foodCost + 1 <= resourceBank.GetFood())
        {
            foodCost++;
            UpdateFoodCostInputField();
        }
    }

    public void DecreaseFoodCost()
    {
        if (foodCost > 0)
        {
            foodCost--;
            UpdateFoodCostInputField();
        }
    }

    private void UpdateFoodCostInputField()
    {
        foodCostInputField.text = foodCost.ToString();
    }

    private void Update()
    {
        int energyAmount = foodCost * 5;
        foodExchange.text = foodCost.ToString();
        energyGet.text = "= " + energyAmount.ToString();
    }

    public void ExchangeFoodForEnergy()
    {
        int energyAmount = foodCost * 5;

        if (resourceBank.CanAfford(foodCost, 0, 0, 0))
        {
            resourceBank.SpendResources(foodCost, 0, 0, 0);
            resourceBank.AddEnergy(energyAmount);
            foodExchange.text = foodCost.ToString();
            energyGet.text = "= " + energyAmount.ToString();
            foodCost = 0; 
            UpdateFoodCostInputField();
        }
        else
        {
            Debug.Log("Insufficient resources to perform the exchange");
        }
    }

    public void OpenExchangePanel()
    {
        exchangePanel.SetActive(true);
    }

    public void CloseExchangePanel()
    {
        exchangePanel.SetActive(false);
    }
}
