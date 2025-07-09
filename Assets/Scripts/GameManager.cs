using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public ItemManager itemManager;
    public TileManager tileManager;
    public UI_Manager uiManager;
    public ShopManager shopManager;
    public CropManager cropManager;

    [Header("Game UI")]
    public GameObject gameOverPanel;

    [Header("Game Timer")]
    public float timeLimitInSeconds = 180f; 
    public TextMeshProUGUI timerText;      

    private float currentTime;
    private bool isTimerRunning = false;
    public bool isPaused = false;

    public Player player;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);

        itemManager = GetComponent<ItemManager>();
        tileManager = GetComponent<TileManager>();
        uiManager = GetComponent<UI_Manager>();
        shopManager = GetComponent<ShopManager>();
        cropManager = GetComponent<CropManager>();

        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Inisialisasi Timer
        currentTime = timeLimitInSeconds;
        isTimerRunning = true;
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                UpdateTimerUI(currentTime);
            }
            else
            {
                // Waktu habis
                currentTime = 0;
                isTimerRunning = false;
                UpdateTimerUI(currentTime); // Pastikan UI menunjukkan 00:00
                GameOver();
            }
        }
    }

    void UpdateTimerUI(float timeToDisplay)
    {
        if (timerText == null) return;

        // Pastikan waktu tidak pernah negatif
        timeToDisplay = Mathf.Max(timeToDisplay, 0);

        // Konversi total detik ke format menit dan detik
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // Format string menjadi "MM:SS" (misal: 03:00)
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void GameOver()
    {
        Debug.Log("WAKTU HABIS! GAME OVER.");
        isPaused = true;
        Time.timeScale = 0f;

        if (gameOverPanel != null)
        {
            // Dapatkan komponen skrip dari panel
            GameOverUI gameOverUI = gameOverPanel.GetComponent<GameOverUI>();
            if (gameOverUI != null)
            {
                // Kirim data skor player ke fungsi ShowScores sebelum panel ditampilkan
                gameOverUI.ShowScores(player.points, player.money);
            }

            // Tampilkan panel Game Over setelah data diatur
            gameOverPanel.SetActive(true);
        }
    }

    // Fungsi untuk mengulang game
    public void RetryGame()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayRandomBGM();
        }

        isPaused = false;
        Time.timeScale = 1f;

        // Hancurkan GameManager saat ini untuk memastikan tidak ada duplikat
        Destroy(this.gameObject);

        SceneManager.LoadScene("GameMain");
    }

    public void ReturnToMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;

        // Hancurkan instance GameManager juga saat kembali ke menu
        Destroy(this.gameObject);

        // Muat scene menu utama
        SceneManager.LoadScene("MainMenu");
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Menghentikan waktu dalam game
        if (uiManager != null && uiManager.shopPanel != null && !uiManager.shopPanel.activeSelf)
        {
            uiManager.pausePanel.SetActive(true); // Menampilkan panel pause
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Mengembalikan waktu ke normal
        if (uiManager != null)
        {
            uiManager.pausePanel.SetActive(false); // Menyembunyikan panel pause
        }
    }
}