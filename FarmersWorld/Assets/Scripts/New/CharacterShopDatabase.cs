using UnityEngine;

public class CharacterShopDatabase: MonoBehaviour
{
    public Tool[] characters;

    public int CharactersCount
    {
        get { return characters.Length; }
    }

    public Tool GetCharacter(int index)
    {
        return characters[index];
    }

    public void PurchaseCharacter(int index)
    {
        characters[index].isPurchased = true;
    }
}
