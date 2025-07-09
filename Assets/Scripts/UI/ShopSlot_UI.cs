using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot_UI : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI itemPriceText;
    private ItemData itemData;
    private Shop_UI shopUI;
    private Inventory.Slot sellSlot;

    public void SetData(ItemData data, Shop_UI ui)
    {
        itemData = data;
        shopUI = ui;
        sellSlot = null;

        itemIcon.sprite = data.icon;
        itemPriceText.text = data.price.ToString() + " G";
    }

    public void SetSellData(Inventory.Slot slot, Shop_UI ui)
    {
        // Ambil ItemData dari ItemManager
        Item item = GameManager.instance.itemManager.GetItemByName(slot.itemName);
        if (item == null) return;

        itemData = item.data;
        shopUI = ui;
        sellSlot = slot;

        itemIcon.sprite = itemData.icon;
        // Tampilkan total harga jual (harga * kuantitas)
        itemPriceText.text = (itemData.price * slot.count).ToString() + " G";

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (sellSlot != null) // Jika sellSlot tidak null, berarti ini mode Jual
        {
            shopUI.SellItem(sellSlot, itemData);
        }
        else // Jika tidak, ini mode Beli
        {
            shopUI.OnItemSelected(itemData);
        }
    }
}