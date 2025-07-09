using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public List<ItemData> itemsForSale;

    public bool BuyItem(string itemName, int quantity)
    {
        Item itemToBuy = GameManager.instance.itemManager.GetItemByName(itemName);
        if (itemToBuy == null)
        {
            Debug.LogError("Item " + itemName + " tidak ditemukan di ItemManager!");
            return false;
        }

        int totalCost = itemToBuy.data.price * quantity;
        Player player = GameManager.instance.player;

        if (player.SpendMoney(totalCost))
        {
            for (int i = 0; i < quantity; i++)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.coinsSfx);
                // 1. Coba tambahkan ke toolbar terlebih dahulu
                bool addedToToolbar = player.inventoryManager.Add("toolbar", itemToBuy);

                // 2. Jika gagal (karena toolbar penuh), coba tambahkan ke backpack
                if (!addedToToolbar)
                {
                    bool addedToBackpack = player.inventoryManager.Add("backpack", itemToBuy);

                    // Opsional: Beri tahu player jika backpack juga penuh
                    if (!addedToBackpack)
                    {
                        Debug.Log("Pembelian gagal sebagian: Toolbar dan Backpack penuh!");
                    }
                }
            }

            GameManager.instance.uiManager.RefreshAll();
            Debug.Log("Membeli " + quantity + " " + itemName + " seharga " + totalCost);
            return true;
        }
        else
        {
            Debug.Log("Pembelian gagal, uang tidak cukup.");
            return false;
        }
    }
}