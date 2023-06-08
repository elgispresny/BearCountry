using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBank : MonoBehaviour
{
    public static ResourceBank instance;

    public int startingFood = 0;
    public int startingWood = 0;
    public int startingGold = 0;
    public int startingEnergy = 0;

    [HideInInspector] public int currentFood;
    private int currentWood;
    private int currentGold;
    [HideInInspector] public int currentEnergy;
    public int energyMax = 100; // Initial value of energyMax

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentFood = startingFood;
        currentWood = startingWood;
        currentGold = startingGold;
        currentEnergy = startingEnergy;
    }

    public void AddFood(int amount)
    {
        currentFood += amount;
    }

    public void AddWood(int amount)
    {
        currentWood += amount;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
    }

    public void AddEnergy(int amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0, energyMax);
    }

    public bool CanAfford(int foodCost, int woodCost, int goldCost, int energyCost)
    {
        return currentFood >= foodCost && currentWood >= woodCost && currentGold >= goldCost && currentEnergy >= energyCost;
    }

    public void SpendResources(int foodCost, int woodCost, int goldCost, int energyCost)
    {
        currentFood -= foodCost;
        currentWood -= woodCost;
        currentGold -= goldCost;
        currentEnergy -= energyCost;
    }

    public int GetFood()
    {
        return currentFood;
    }

    public int GetWood()
    {
        return currentWood;
    }

    public int GetGold()
    {
        return currentGold;
    }

    public int GetEnergy()
    {
        return currentEnergy;
    }
    
    public int GetEnergyMax()
    {
        return energyMax;
    }
}
