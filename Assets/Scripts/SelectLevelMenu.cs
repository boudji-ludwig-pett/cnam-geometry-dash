using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectLevelMenu : MonoBehaviour
{
    public Text levelNameText;

    public void PlayLevel()
    {
        SceneManager.LoadScene("LevelScene");
    }

    public void BackBtn()
    {
        SceneManager.LoadScene("HomeScene");
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
