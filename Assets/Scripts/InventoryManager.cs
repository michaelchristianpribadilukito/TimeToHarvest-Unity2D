using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Dictionary<string, Inventory> inventoryByName = new Dictionary<string, Inventory>();

    [Header("Backpack")]
    public Inventory backpack;
    public int backpackSlotsCount = 21;

    [Header("Toolbar")]
    public Inventory toolbar;
    public int toolbarSlotsCount = 6;

    [Header("Starting Items")]
    public Item hoeItem;
    public Item wateringPotItem;

    private void Awake()
    {
        backpack = new Inventory(backpackSlotsCount);
        toolbar = new Inventory(toolbarSlotsCount);

        inventoryByName.Add("backpack", backpack);
        inventoryByName.Add("toolbar", toolbar);

        if (hoeItem != null)
        {
            toolbar.Add(hoeItem); 
        }
        if (wateringPotItem != null)
        {
            toolbar.Add(wateringPotItem); 
        }
    }

    public Inventory GetInventoryByName(string name)
    {
        if (inventoryByName.ContainsKey(name))
        {
            return inventoryByName[name];
        }

        return null;
    }

    public bool Add(string inventoryName, Item item)
    {
        if (inventoryByName != null)
        {
            if (inventoryByName.ContainsKey(inventoryName))
            {
                // Panggil metode Add yang baru dan kembalikan hasilnya
                return inventoryByName[inventoryName].Add(item);
            }
        }
        return false; // Gagal jika nama inventory tidak ditemukan
    }
}