using UnityEngine;
using TMPro; 

public class GameOverUI : MonoBehaviour
{
    [Header("Text Elements")]
    public TextMeshProUGUI harvestPointsText;
    public TextMeshProUGUI moneyPointsText;
    public TextMeshProUGUI totalPointsText;

    // Fungsi ini akan dipanggil oleh GameManager untuk mengisi skor
    public void ShowScores(int harvestPoints, int currentMoney)
    {
        // Hitung poin dari uang (5 uang = 1 poin)
        int moneyPoints = currentMoney / 5;

        // Hitung total poin
        int totalPoints = harvestPoints + moneyPoints;

        harvestPointsText.text = $"HARVEST POINT<pos=85%>{harvestPoints} pt";
        moneyPointsText.text = $"MONEY POINT ({currentMoney} G)<pos=85%>{moneyPoints} pt";
        totalPointsText.text = $"TOTAL POINT<pos=85%>{totalPoints} pt";
    }
        
    public void Retry()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.clickSfx);

        GameManager.instance.RetryGame();
    }
        
    public void BackToMenu()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.clickSfx);

        GameManager.instance.ReturnToMenu();
    }
}