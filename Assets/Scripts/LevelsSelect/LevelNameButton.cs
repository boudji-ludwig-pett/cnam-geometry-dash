using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelNameButton : MonoBehaviour
{
    public void PlayLevel()
    {
        SceneManager.LoadScene("LevelScene");
    }
    public void EditLevel()
    {
        PlayerPrefs.SetInt("CreateMode", 0);
        SceneManager.LoadScene("LevelEditorScene");
    }
}
