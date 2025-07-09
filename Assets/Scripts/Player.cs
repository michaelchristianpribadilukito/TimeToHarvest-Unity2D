using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public int money = 500;
    public int points = 0;
    public bool isPerformingAction = false;

    public InventoryManager inventoryManager;
    private TileManager tileManager;
    private CropManager cropManager;
    private Animator animator;

    private Vector3Int targetTilePosition;
    private Vector3Int direction = Vector3Int.zero;

    private void Start()
    {
        tileManager = GameManager.instance.tileManager;
        cropManager = GameManager.instance.cropManager;
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        float lastMoveX = animator.GetFloat("horizontal");
        float lastMoveY = animator.GetFloat("vertical");

        if (Mathf.Abs(lastMoveX) > Mathf.Abs(lastMoveY))
        {
            direction = lastMoveX > 0 ? Vector3Int.right : Vector3Int.left;
        }

        else if (Mathf.Abs(lastMoveY) > Mathf.Abs(lastMoveX))
        {
            direction = lastMoveY > 0 ? Vector3Int.up : Vector3Int.down;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        if (tileManager == null || cropManager == null || animator == null)
        {
            Debug.LogError("Salah satu komponen Manajer belum di-assign di Player!");
            return;
        }

        targetTilePosition = Vector3Int.FloorToInt(transform.position ) + direction;
        Inventory.Slot selectedSlot = inventoryManager.toolbar.selectedSlot;

        if (selectedSlot == null)
        {
            Debug.LogWarning("Tidak ada slot yang terpilih di toolbar.");
            return;
        }

        Debug.Log("--- Interaksi Dimulai ---");
        Debug.Log("Item Terpilih: " + selectedSlot.itemName);
        Debug.Log("Target Tile Position: " + targetTilePosition);

        // 1. Aksi Mencangkul (Hoe)
        if (selectedSlot.itemName == "Hoe")
        {
            string tileName = tileManager.GetTileName(targetTilePosition);
            if (tileName == "Interactable")
            {
                Debug.Log("Aksi: Mencangkul tile 'Interactable'.");
                isPerformingAction = true;
                animator.SetTrigger("plowAction");
                return;
            }
        }

        // 2. Aksi Menanam Bibit (Seed)
        Item selectedItem = GameManager.instance.itemManager.GetItemByName(selectedSlot.itemName);

        if (selectedItem == null)
        {
            // Debug.Log("Item '" + selectedSlot.itemName + "' tidak ditemukan di ItemManager. Ini bukan bibit, lanjut ke aksi lain.");
        }
        else if (selectedItem.data is CropData)
        {
            Debug.Log("Item adalah bibit (CropData). Mencoba menanam...");
            bool isPlowed = cropManager.IsPlowed(targetTilePosition);
            bool hasCrop = cropManager.HasCrop(targetTilePosition);
            Debug.Log("Kondisi Tile: IsPlowed? " + isPlowed + ", HasCrop? " + hasCrop);

            if (isPlowed && !hasCrop)
            {
                Debug.Log("Kondisi terpenuhi! MENANAM SEKARANG.");
                AudioManager.instance.PlaySFX(AudioManager.instance.plantSfx);

                cropManager.Plant(targetTilePosition, selectedItem.data as CropData);
                inventoryManager.toolbar.Remove(inventoryManager.toolbar.slots.IndexOf(selectedSlot));
                GameManager.instance.uiManager.RefreshAll();
                return;
            }
        }

        // 3. Aksi Menyiram (Watering Pot)
        if (selectedSlot.itemName == "Watering Pot")
        {
            Debug.Log("Item adalah Watering Pot. Mencoba menyiram...");
            bool hasCropToWater = cropManager.HasCrop(targetTilePosition);
            Debug.Log("Kondisi Tile: HasCrop? " + hasCropToWater);

            if (hasCropToWater)
            {
                Debug.Log("Kondisi terpenuhi! MENYIRAM SEKARANG.");
                isPerformingAction = true;
                animator.SetTrigger("waterAction");
                return;
            }
        }

        // 4. Aksi Memanen
        if (cropManager.HasCrop(targetTilePosition))
        {
            bool isGrown = cropManager.IsCropFullyGrown(targetTilePosition);
            Debug.Log("Mencoba memanen... Kondisi Tile: HasCrop? true, IsFullyGrown? " + isGrown);
            if (isGrown)
            {
                Debug.Log("Kondisi terpenuhi! MEMANEN SEKARANG.");
                cropManager.Harvest(targetTilePosition);
                return;
            }
        }

        Debug.Log("--- Tidak ada aksi yang valid ditemukan ---");
    }

    public void PlowTileEvent()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.plowSfx);

        tileManager.PlowTile(targetTilePosition);
    }

    public void WaterTileEvent()
    {
        if (cropManager != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.waterSfx);
            cropManager.Water(targetTilePosition);
        }
    }

    public void DropItem(Item item)
    {
        Vector2 spawnLocation = transform.position;
        Vector2 spawnOffset = Random.insideUnitCircle * 1.25f;

        Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);
        droppedItem.rb.AddForce(spawnOffset * .2f, ForceMode2D.Impulse);
    }

    public void DropItem(Item item, int numToDrop)
    {
        for (int i = 0; i < numToDrop; i++)
        {
            DropItem(item);
        }
    }

    public void AddPoints(int amount)
    {
        points += amount;
        // Kita akan memanggil fungsi update UI di sini nanti
        GameManager.instance.uiManager.UpdatePlayerPointsUI(points);
    }

    public bool SpendMoney(int amount)
    {
        if(money >= amount)
        {
            money -= amount;
            GameManager.instance.uiManager.UpdatePlayerMoneyUI(money);
            return true;
        }
        else
        {
            Debug.Log("Not enough money!");
            return false;
        }
    }

    public void EarnMoney(int amount)
    {
        money += amount;
        GameManager.instance.uiManager.UpdatePlayerMoneyUI(money);
    }

    public void EndAction()
    {
        isPerformingAction = false;
    }
}