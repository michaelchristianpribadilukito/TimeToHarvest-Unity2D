using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public Dictionary<string, InventoryUI> inventoryUIByName = new Dictionary<string, InventoryUI>();
    public List<InventoryUI> inventoryUIs;

    public GameObject inventoryPanel;
    public GameObject pausePanel;

    [Header("Player Status UI")]
    public TextMeshProUGUI persistentMoneyText;
    public TextMeshProUGUI persistentPointsText;

    [Header("Shop UI")]
    public GameObject shopPanel;
    public Shop_UI shopUI;
    public TextMeshProUGUI playerMoneyText;

    public static Slot_UI draggedSlot;
    public static Image draggedIcon;

    public static bool dragSingle;
      
    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        ToggleInventoryUI();
        if (shopPanel != null) shopPanel.SetActive(false);

        UpdatePlayerMoneyUI(GameManager.instance.player.money);
        UpdatePlayerPointsUI(GameManager.instance.player.points);

        if (pausePanel != null) pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventoryUI();
        }

        if (Input.GetKeyDown(KeyCode.B)) // Tambahkan blok ini
        {
            ToggleShopUI();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            dragSingle = true;
        }
        else
        {
            dragSingle = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (shopPanel != null && shopPanel.activeSelf)
            {
                ToggleShopUI(); // Jika toko terbuka, tombol Esc akan menutup toko
            }
            else
            {
                TogglePause();
            }
        }
    }

    public void ToggleInventoryUI()
    {
        if (GameManager.instance.isPaused && (inventoryPanel == null || !inventoryPanel.activeSelf)) return;

        if (inventoryPanel != null)
        {
            if (!inventoryPanel.activeSelf)
            {
                inventoryPanel.SetActive(true);
                RefreshInventoryUI("backpack"); 
            }
            else
            {
                inventoryPanel.SetActive(false);
            }
        }
    }

    public void ToggleShopUI()
    {
        if (GameManager.instance.isPaused && (shopPanel == null || !shopPanel.activeSelf)) return; // Jangan buka toko jika game dijeda

        if (shopPanel != null)
        {
            bool isOpening = !shopPanel.activeSelf;
            shopPanel.SetActive(isOpening);

            if (isOpening)
            {
                if (shopUI != null)
                {
                    shopUI.EnterBuyMode();
                }
                UpdatePlayerMoneyUI(GameManager.instance.player.money);
            }
            else
            {
                if (shopUI != null)
                {
                    shopUI.ResetShop();
                }
            }
        }
    }

    public void UpdatePlayerMoneyUI(int currentMoney) 
    {
        if (playerMoneyText != null)
        {
            playerMoneyText.text = currentMoney.ToString();
        }
        if (persistentMoneyText != null)
        {
            persistentMoneyText.text = currentMoney.ToString();
        }
    }
    public void UpdatePlayerPointsUI(int currentPoints)
    {
        if (persistentPointsText != null)
        {
            persistentPointsText.text = currentPoints.ToString();
        }
    }

    public void RefreshInventoryUI(string inventoryName)
    {
        if (inventoryUIByName.ContainsKey(inventoryName))
        {
            inventoryUIByName[inventoryName].Refresh();
        }
    }

    public void RefreshAll()
    {
        foreach (KeyValuePair<string, InventoryUI> keyValuePair in inventoryUIByName)
        {
            keyValuePair.Value.Refresh();
        }
    }

    public InventoryUI GetInventoryUI(string inventoryName)
    {
        if (inventoryUIByName.ContainsKey(inventoryName))
        {
            return inventoryUIByName[inventoryName];
        }

        return null;
    }

    private void Initialize()
    {
        foreach (InventoryUI ui in inventoryUIs)
        {
            if (!inventoryUIByName.ContainsKey(ui.inventoryName))
            {
                inventoryUIByName.Add(ui.inventoryName, ui);
            }
        }
    }

    public void TogglePause()
    {
        if (GameManager.instance.isPaused)
        {
            GameManager.instance.ResumeGame();
        }
        else
        {
            GameManager.instance.PauseGame();
        }
    }

}