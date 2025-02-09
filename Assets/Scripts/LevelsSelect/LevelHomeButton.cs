using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHomeButton : MonoBehaviour
{
    public void GoToHome()
    {
        SceneManager.LoadScene("HomeScene");
    }
}
