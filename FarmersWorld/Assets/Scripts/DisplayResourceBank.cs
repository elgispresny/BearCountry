using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DisplayResourceBank : MonoBehaviour
{
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI goldText;

    public TextMeshProUGUI energyText;
    [SerializeField] private Image energyImg;

    private ResourceBank resourceBank;

    private void Start()
    {
        resourceBank = ResourceBank.instance;
    }

    private void Update()
    {
        UpdateResourceText();
    }

    private void UpdateResourceText()
    {
        // DOVirtual.Float(resourceBank.currentFood, 3f, (v) => foodText.text = Mathf.Floor (v).ToString());
        foodText.text = resourceBank.GetFood().ToString();
        woodText.text = resourceBank.GetWood().ToString();
        goldText.text = resourceBank.GetGold().ToString();
        energyText.text = resourceBank.GetEnergy().ToString();

        float fillAmount = (float)resourceBank.currentEnergy / resourceBank.energyMax;
        energyImg.fillAmount = fillAmount;
        energyImg.DOFillAmount(fillAmount, 0.5f);

        energyText.text = resourceBank.currentEnergy + "/" + resourceBank.energyMax;
    }
}
