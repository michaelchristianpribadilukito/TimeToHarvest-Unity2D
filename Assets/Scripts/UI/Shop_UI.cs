using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop_UI : MonoBehaviour
{
    [Header("References")]
    public ShopManager shopManager;
    public GameObject shopSlotPrefab;
    public Transform slotContainer;

    [Header("Selected Item UI")]
    public Image selectedItemIcon;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemPrice;
    public TextMeshProUGUI quantityText;

    [Header("Buttons")]
    public Button buyButton;
    public Button closeButton;
    public Button increaseQtyBtn;
    public Button decreaseQtyBtn;
    public Button navBuyButton;
    public Button navSellButton;

    [Header("Mode Colors")]
    public Color activeModeColor = Color.yellow;
    public Color inactiveModeColor = Color.white;

    private ItemData currentSelectedItem;
    private int currentQuantity = 1;
    private bool isSellMode = false;

    void Start()
    {
        buyButton.onClick.AddListener(BuyItem);
        closeButton.onClick.AddListener(() => GameManager.instance.uiManager.ToggleShopUI());
        increaseQtyBtn.onClick.AddListener(IncreaseQuantity);
        decreaseQtyBtn.onClick.AddListener(DecreaseQuantity);

        navBuyButton.onClick.AddListener(EnterBuyMode);
        navSellButton.onClick.AddListener(EnterSellMode);

        // Atur kondisi awal saat toko dibuka
        isSellMode = false;

        RefreshUI();

        selectedItemIcon.transform.parent.gameObject.SetActive(false);
    }

    public void OnItemSelected(ItemData item)
    {
        currentSelectedItem = item;
        currentQuantity = 1;

        selectedItemIcon.transform.parent.gameObject.SetActive(true);

        UpdateSelectionUI();
    }

    private void IncreaseQuantity()
    {
        currentQuantity++;
        UpdateSelectionUI();
    }

    private void DecreaseQuantity()
    {
        currentQuantity = Mathf.Max(1, currentQuantity - 1);
        UpdateSelectionUI();
    }

    private void UpdateSelectionUI()
    {
        if (currentSelectedItem != null)
        {
            selectedItemIcon.sprite = currentSelectedItem.icon;
            selectedItemName.text = currentSelectedItem.itemName;
            quantityText.text = currentQuantity.ToString();

            int totalPrice = currentSelectedItem.price * currentQuantity;
            selectedItemPrice.text = "Total: " + totalPrice.ToString() + " G";
        }
    }

    private void BuyItem()
    {
        if (currentSelectedItem != null && currentQuantity > 0)
        {
            shopManager.BuyItem(currentSelectedItem.itemName, currentQuantity);
        }
    }

    public void EnterBuyMode()
    {
        if (isSellMode) 
        {
            isSellMode = false;
            RefreshUI();
        }
    }

    public void EnterSellMode()
    {
        if (!isSellMode)
        {
            isSellMode = true;
            RefreshUI();
        }
    }

    public void ResetShop()
    {
        // 1. Reset item yang sedang dipilih
        currentSelectedItem = null;

        // 2. Sembunyikan panel detail item (Item_Border)
        if (selectedItemIcon != null && selectedItemIcon.transform.parent != null)
        {
            selectedItemIcon.transform.parent.gameObject.SetActive(false);
        }

        // 3. Kembalikan ke mode Beli setiap kali toko ditutup
        if (isSellMode)
        {
            isSellMode = false;
            RefreshUI(); // Panggil RefreshUI agar judul dan daftar item kembali ke mode Beli
        }
    }

    private void RefreshUI()
    {
        currentSelectedItem = null;
        // Sembunyikan panel detail item saat beralih mode
        if (selectedItemIcon.transform.parent.gameObject.activeSelf)
        {
            selectedItemIcon.transform.parent.gameObject.SetActive(false);
        }

        // Hapus semua item di daftar
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        if (isSellMode)
        {
            // Atur warna tombol
            navSellButton.image.color = activeModeColor;
            navBuyButton.image.color = inactiveModeColor;

            GetComponentInChildren<TextMeshProUGUI>().text = "SELL";
            PopulateSellList();
        }
        else
        {
            // Atur warna tombol
            navBuyButton.image.color = activeModeColor;
            navSellButton.image.color = inactiveModeColor;

            GetComponentInChildren<TextMeshProUGUI>().text = "SHOP";
            PopulateBuyList();
        }
    }

    private void PopulateBuyList()
    {
        // Logika ini adalah isi dari fungsi PopulateShop() Anda yang lama
        foreach (ItemData item in shopManager.itemsForSale)
        {
            GameObject slotGO = Instantiate(shopSlotPrefab, slotContainer);
            ShopSlot_UI slotUI = slotGO.GetComponent<ShopSlot_UI>();
            slotUI.SetData(item, this);
            slotGO.GetComponent<Button>().onClick.AddListener(slotUI.OnClick);
        }
    }

    private void PopulateSellList()
    {
        // Ambil inventory pemain
        Inventory backpack = GameManager.instance.player.inventoryManager.GetInventoryByName("backpack");
        Inventory toolbar = GameManager.instance.player.inventoryManager.GetInventoryByName("toolbar");

        // Gabungkan kedua inventory untuk ditampilkan
        List<Inventory.Slot> itemsToDisplay = new List<Inventory.Slot>();
        itemsToDisplay.AddRange(toolbar.slots);
        itemsToDisplay.AddRange(backpack.slots);

        foreach (Inventory.Slot slot in itemsToDisplay)
        {
            // Periksa apakah slot tidak kosong dan bukan item yang dilarang
            if (!slot.IsEmpty && slot.itemName != "Hoe" && slot.itemName != "Watering Pot")
            {
                GameObject slotGO = Instantiate(shopSlotPrefab, slotContainer);
                ShopSlot_UI slotUI = slotGO.GetComponent<ShopSlot_UI>();

                // Kita perlu fungsi baru di ShopSlot_UI untuk menampilkan data jual
                slotUI.SetSellData(slot, this);
                // Listener diatur di dalam ShopSlot_UI
            }
        }
    }

    public void SellItem(Inventory.Slot slotToSell, ItemData itemData)
    {
        // Hitung pendapatan
        int earnings = itemData.price * slotToSell.count;

        // Tambahkan uang ke pemain
        AudioManager.instance.PlaySFX(AudioManager.instance.coinsSfx);
        GameManager.instance.player.EarnMoney(earnings);

        // Hapus item dari inventory
        // Cari tahu item ini ada di toolbar atau backpack
        Inventory backpack = GameManager.instance.player.inventoryManager.backpack;
        Inventory toolbar = GameManager.instance.player.inventoryManager.toolbar;

        if (toolbar.slots.Contains(slotToSell))
        {
            toolbar.Remove(toolbar.slots.IndexOf(slotToSell), slotToSell.count);
        }
        else if (backpack.slots.Contains(slotToSell))
        {
            backpack.Remove(backpack.slots.IndexOf(slotToSell), slotToSell.count);
        }

        // Refresh UI Toko dan Inventory
        GameManager.instance.uiManager.RefreshAll();
        RefreshUI();
    }
}