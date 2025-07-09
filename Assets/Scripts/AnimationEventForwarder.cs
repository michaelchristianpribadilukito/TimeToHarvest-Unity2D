using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    // Variabel untuk menampung referensi ke skrip Player utama
    private Player playerController;

    private void Awake()
    {
        // Secara otomatis mencari skrip Player.cs di parent object
        playerController = GetComponentInParent<Player>();
    }

    // Fungsi ini akan terlihat oleh Animation Event
    public void PlowTileEvent()
    {
        // Meneruskan panggilan ke fungsi yang sebenarnya di Player.cs
        if (playerController != null)
        {
            playerController.PlowTileEvent();
        }
    }

    // Fungsi ini juga akan terlihat oleh Animation Event
    public void WaterTileEvent()
    {
        // Meneruskan panggilan ke fungsi yang sebenarnya di Player.cs
        if (playerController != null)
        {
            playerController.WaterTileEvent();
        }
    }

    public void EndActionEvent()
    {
        if (playerController != null)
        {
            playerController.EndAction();
        }
    }
}