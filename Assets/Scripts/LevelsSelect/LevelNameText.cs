using UnityEngine;
using UnityEngine.UI;

public class LevelNameText : MonoBehaviour
{
    public Text levelNameText;
    public LevelsLoader levelsLoader;

    public void Start()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
        levelNameText.text = levelsLoader.levelCurrent.name;
    }

    public void Update()
    {
        levelNameText.text = levelsLoader.levelCurrent.name;
    }
}
