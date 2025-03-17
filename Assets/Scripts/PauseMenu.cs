using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseButton;
    public LevelLoader levelLoader;

    public void Pause()
    {
        Time.timeScale = 0;
        levelLoader.audioSource.Pause();

        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void Home()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("HomeScene");
    }

    public void Resume()
    {
        Time.timeScale = 1;
        levelLoader.audioSource.Play();

        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
    }
}
