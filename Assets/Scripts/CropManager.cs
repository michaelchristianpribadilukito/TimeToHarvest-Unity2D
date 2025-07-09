using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CropManager : MonoBehaviour
{
    public Tilemap interactableMap;
    public GameObject plantPrefab; 

    private Dictionary<Vector3Int, CropState> plantedCrops = new Dictionary<Vector3Int, CropState>();

    private class CropState
    {
        public CropData cropData;
        public int growthStage;
        public Coroutine growthCoroutine;
        public GameObject plantInstance; 

        public CropState(CropData data)
        {
            cropData = data;
            growthStage = 0;
            growthCoroutine = null;
            plantInstance = null;
        }

        public bool IsFullyGrown()
        {
            // Perhatikan: growthSprites.Length, bukan growthTiles.Length
            return growthStage >= cropData.growthSprites.Length - 1;
        }
    }

    public void Plant(Vector3Int position, CropData cropToPlant)
    {
        if (plantedCrops.ContainsKey(position)) return;

        // Pastikan ada sprite untuk ditanam
        if (cropToPlant.growthSprites == null || cropToPlant.growthSprites.Length == 0)
        {
            Debug.LogError("CropData " + cropToPlant.itemName + " tidak memiliki growthSprites!");
            return;
        }

        // Buat instance CropState baru
        CropState newCropState = new CropState(cropToPlant);

        // Instantiate prefab tanaman
        Vector3 worldPosition = interactableMap.GetCellCenterWorld(position);
        Debug.Log("Menanam di posisi dunia: " + worldPosition);
        GameObject plantGO = Instantiate(plantPrefab, worldPosition, Quaternion.identity);

        // Atur sprite awal
        plantGO.GetComponent<SpriteRenderer>().sprite = cropToPlant.growthSprites[0];

        // Simpan instance GameObject ke dalam CropState
        newCropState.plantInstance = plantGO;

        // Tambahkan ke dictionary
        plantedCrops.Add(position, newCropState);
        Debug.Log("Menanam " + cropToPlant.itemName);
    }

    public void Water(Vector3Int position)
    {
        Debug.Log("Fungsi CropManager.Water() berhasil dipanggil!");
        if (!plantedCrops.ContainsKey(position)) return;

        CropState cropState = plantedCrops[position];

        if (cropState.IsFullyGrown() || cropState.growthCoroutine != null)
        {
            return;
        }

        GameManager.instance.tileManager.WaterTile(position);
        cropState.growthCoroutine = StartCoroutine(GrowCrop(position));
        Debug.Log("Menyiram " + cropState.cropData.itemName + ", proses tumbuh dimulai.");
    }

    public void Harvest(Vector3Int position)
    {
        if (!plantedCrops.ContainsKey(position)) return;

        CropState cropState = plantedCrops[position];
        if (cropState.IsFullyGrown())
        {
            if (AudioManager.instance != null && AudioManager.instance.harvestSfx != null)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.harvestSfx);
            }

            // Hancurkan GameObject tanaman dari scene
            if (cropState.plantInstance != null)
            {
                Destroy(cropState.plantInstance);
            }

            // Hapus dari dictionary
            plantedCrops.Remove(position);

            // Kembalikan tile ke semula
            GameManager.instance.tileManager.RevertToDefaultTile(position);

            GameManager.instance.player.AddPoints(cropState.cropData.pointsOnHarvest);

            // Munculkan hasil panen dengan efek memantul
            Vector3 spawnPosition = interactableMap.GetCellCenterWorld(position);
            for (int i = 0; i < cropState.cropData.yieldAmount; i++)
            {
                Vector2 spawnOffset = Random.insideUnitCircle * 1.1f;
                Item droppedItem = Instantiate(cropState.cropData.yieldItem, spawnPosition + (Vector3)spawnOffset, Quaternion.identity);
                if (droppedItem.rb != null)
                {
                    droppedItem.rb.AddForce(spawnOffset * 0.3f, ForceMode2D.Impulse);
                }
            }
        }
    }

    // --- COROUTINE GROWCROP YANG BARU ---
    private IEnumerator GrowCrop(Vector3Int position)
    {
        CropState cropState = plantedCrops[position];
        CropData cropData = cropState.cropData;

        // Hitung waktu jeda
        float timePerStage = cropData.totalGrowthTimeInSeconds / (cropData.growthSprites.Length - 1);

        while (!cropState.IsFullyGrown())
        {
            yield return new WaitForSeconds(timePerStage);

            cropState.growthStage++;

            // Ganti sprite pada GameObject tanaman
            if (cropState.plantInstance != null)
            {
                cropState.plantInstance.GetComponent<SpriteRenderer>().sprite = cropData.growthSprites[cropState.growthStage];
            }
        }

        Debug.Log(cropData.itemName + " di " + position + " sudah siap panen!");
        cropState.growthCoroutine = null;
    }

    public bool IsCropFullyGrown(Vector3Int position)
    {
        if (!plantedCrops.ContainsKey(position)) return false;
        return plantedCrops[position].IsFullyGrown();
    }

    public bool IsPlowed(Vector3Int position)
    {
        string tileName = GameManager.instance.tileManager.GetTileName(position);
        return tileName == "Summer_Plow" || tileName == "Summer_Wet";
    }

    public bool HasCrop(Vector3Int position)
    {
        return plantedCrops.ContainsKey(position);
    }
}