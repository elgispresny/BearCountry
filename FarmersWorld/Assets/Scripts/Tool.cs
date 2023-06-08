using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Tool
{
    public int ID;
    public string name;
    public Sprite image;
    public Level level;
    [Range (0, 100)] public int rewardRate;
    [Range (0, 100)] public int chargeTime;
    [Range (0, 100)] public int energyConsumed;
    [Range (0, 100)] public int durabilityConsumed;
    public int durability;
    public float durabilityMax;
    public int reducedurability;

    public ResourceType resourceType;
    public Button button;
    public Button repairButton;
    
    [Header("Timer")]
    public Timer timer;
    public float timerDuration;

    public GameObject prefab;
    
    [Header("Mining")]
    public int foodMining;
    public int woodMining;
    public int goldMining;
    public int energyСonsumption;

    public bool isPurchased;
    public int price;

    public void SubtractDurability(int amount)
    {
        durability -= amount;
        durability = Mathf.Max(durability, 0);
    }

    public int Durability()
    {
        return durability;
    }

    public enum ResourceType
    {
        Food,
        Wood,
        Gold
    }

    public enum Level
    {
        Common,
        Rare,
        Epic
    }
}
