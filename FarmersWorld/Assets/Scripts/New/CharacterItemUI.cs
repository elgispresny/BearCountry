using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;

public class CharacterItemUI : MonoBehaviour
{
	[SerializeField] Color itemNotSelectedColor;
	[SerializeField] Color itemSelectedColor;

	[Space (20f)]
	[SerializeField] Image characterImage;
	[SerializeField] TMP_Text characterNameText;
	// [SerializeField] Image characterSpeedFill;
	// [SerializeField] Image characterPowerFill;
	[SerializeField] TMP_Text characterPriceText;
	[SerializeField] TMP_Text characterSpeedText;

	[SerializeField] TMP_Text characterDurabilityText;
	[SerializeField] Button characterPurchaseButton;

	[Header("Item Buttons")]
	[SerializeField] Button repairButton;
	[SerializeField] public Button removeButton;

	[Space (20f)]
	[SerializeField] Button itemButton;
	[SerializeField] Image itemImage;
	[SerializeField] Outline itemOutline;

	private int itemIndex;

	private void OnEnable()
	{
		removeButton.onClick.AddListener(() =>
        {
            RemoveItem();
        });
	}

	private void RemoveItem()
	{
		CharacterShopUI.instance.DestroyNewItem(1);
	}
	
	//--------------------------------------------------------------
	public void SetItemPosition (Vector2 pos)
	{
		GetComponent <RectTransform> ().anchoredPosition += pos;
	}

	public void SetIndex(int index)
    {
        itemIndex = index;
    }

	public int GetIndex()
    {
        return itemIndex;
    }


	public void SetCharacterImage (Sprite sprite)
	{
		characterImage.sprite = sprite;
	}

	public void SetCharacterName (string name)
	{
		characterNameText.text = name;
	}

	public void SetCharacterDurability (int durability, float durabilityMax)
	{
		characterDurabilityText.text = durability + "/" + durabilityMax;
	}

	public void SetCharacterSpeed (int speed)
	{
		characterSpeedText.text = speed.ToString ();
	}

	// public void SetCharacterPower (float power)
	// {
	// 	characterPowerFill.fillAmount = power / 100;
	// }

	public void SetCharacterPrice (int price)
	{
		characterPriceText.text = price.ToString ();
	}

	public void SetCharacterAsPurchased ()
	{
		characterPurchaseButton.gameObject.SetActive (false);
		itemButton.interactable = true;

		itemImage.color = itemNotSelectedColor;
	}

	public void OnItemPurchase (int itemIndex, UnityAction<int> action)
	{
		characterPurchaseButton.onClick.RemoveAllListeners ();
		characterPurchaseButton.onClick.AddListener (() => action.Invoke (itemIndex));
	}

	public void OnItemSelect (int itemIndex, UnityAction<int> action)
	{
		itemButton.interactable = true;

		itemButton.onClick.RemoveAllListeners ();
		itemButton.onClick.AddListener (() => action.Invoke (itemIndex));
	}

    /*************************************************************/
	public void SetRemoveButtonClickAction(UnityAction<int> action)
    {
        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(() => action.Invoke(itemIndex));
    }

	public void SelectItem ()
	{
		itemOutline.enabled = true;
		// itemImage.color = itemSelectedColor;
		itemButton.interactable = false;
	}

	public void DeselectItem ()
	{
		itemOutline.enabled = false;
		// itemImage.color = itemNotSelectedColor;
		itemButton.interactable = true;
	}

	public void AnimateShakeItem ()
	{
		//End all animations first
		transform.DOComplete ();

		transform.DOShakePosition (1f, new Vector3 (8f, 0, 0), 10, 0).SetEase (Ease.Linear);
	}
}
