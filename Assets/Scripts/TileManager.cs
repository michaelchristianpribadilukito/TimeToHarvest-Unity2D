using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public Tilemap interactableMap;
    public Tile hiddenInteractableTile;
    public Tile plowedTile;
    public Tile wateredTile;
    void Start()
    {
        foreach (var position in interactableMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = interactableMap.GetTile(position);

            if (tile != null && tile.name == "Interactable_Visible")
            {
                interactableMap.SetTile(position, hiddenInteractableTile);
            }
        }
    }

    public void PlowTile(Vector3Int position)
    {
        interactableMap.SetTile(position, plowedTile);
    }

    public void WaterTile(Vector3Int position)
    {
        interactableMap.SetTile(position, wateredTile);
    }

    public void RevertToDefaultTile(Vector3Int position)
    {
        // Mengembalikan tile ke kondisi awal yang bisa di-interact
        interactableMap.SetTile(position, hiddenInteractableTile);
    }

    public string GetTileName(Vector3Int position)
    {
        if (interactableMap != null)
        {
            TileBase tile = interactableMap.GetTile(position);

            if (tile != null)
            {
                return tile.name;
            }
        }

        return "";
    }
}