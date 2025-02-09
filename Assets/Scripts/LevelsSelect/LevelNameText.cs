using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelNameText : MonoBehaviour
{
    public LevelsLoader levelsLoader;
    public Text levelNameText;

    public void Start()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
        levelNameText.text = levelsLoader.level.Name;
    }
}
