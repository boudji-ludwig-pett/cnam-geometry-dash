using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHomeButton : MonoBehaviour
{
    public void GoToHome()
    {
        PlayerPrefs.SetInt("CreateMode", 0);
        PlayerPrefs.SetInt("EditMode", 0);
        SceneManager.LoadScene("HomeScene");
    }
}
