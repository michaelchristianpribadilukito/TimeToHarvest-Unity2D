using UnityEngine;

[CreateAssetMenu(fileName = "CropData", menuName = "Crop Data", order = 51)]
public class CropData : ItemData
{

    [Header("Crop Specific Info")]
    public Item yieldItem;
    public int yieldAmount = 1;
    public int pointsOnHarvest = 1;

    [Header("Growth Info")]
    public Sprite[] growthSprites;
    public float totalGrowthTimeInSeconds = 20f;
}