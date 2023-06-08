using UnityEngine;

public class CharacterDurabilitySaveManager : MonoBehaviour
{
    public static CharacterDurabilitySaveManager instance { get; private set; }

    private Tool character;

    private const string DurabilityKey = "CharacterDurability";

    private void Start()
    {
        instance = this;
        LoadDurability();
    }

    private void OnDestroy()
    {
        SaveDurability();
    }

    public void SaveDurability()
    {
        PlayerPrefs.SetInt(DurabilityKey, character.durability);
        PlayerPrefs.Save();
    }

    private void LoadDurability()
    {
        if (PlayerPrefs.HasKey(DurabilityKey))
        {
            int savedDurability = PlayerPrefs.GetInt(DurabilityKey);
            character.durability = savedDurability;
        }
    }
}
