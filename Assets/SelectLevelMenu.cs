using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectLevelMenu : MonoBehaviour
{
    public void PlayLevel()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void BackBtn()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void LastLevel()
    {
        // TODO
    }

    public void NextLevel()
    {
        // TODO
    }

    public void LevelStatsBtn()
    {
        // SceneManager.LoadSceneAsync(?);
    }
}
