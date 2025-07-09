using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void ResumeGame()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.clickSfx);
        
        GameManager.instance.ResumeGame();
    }

    public void RetryGame()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.clickSfx);

        GameManager.instance.RetryGame();
    }

    public void BackToMainMenu()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.clickSfx);

        GameManager.instance.ReturnToMenu();
    }

    public void QuitGame()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.clickSfx);

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}