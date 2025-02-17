using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelNameButton : MonoBehaviour
{
    public void PlayLevel()
    {
        SceneManager.LoadScene("LevelScene");
    }
}
