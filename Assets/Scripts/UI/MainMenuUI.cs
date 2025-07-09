using UnityEngine;
using UnityEngine.SceneManagement; // Diperlukan untuk mengelola scene

public class MainMenuUI : MonoBehaviour
{
    // Fungsi ini akan dipanggil oleh tombol "Play"
    private void Start()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayRandomBGM();
        }
    }
    public void StartGame()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.clickSfx);

            AudioManager.instance.PlayRandomBGM();
        }

        SceneManager.LoadScene("GameMain");
    }

    public void QuitGame()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.clickSfx);
        }

        Application.Quit();

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}