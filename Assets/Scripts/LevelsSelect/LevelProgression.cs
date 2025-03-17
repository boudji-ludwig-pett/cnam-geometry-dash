using UnityEngine;
using UnityEngine.UI;

public class LevelProgression : MonoBehaviour
{
    public Text levelProgressionText;

    public LevelsLoader levelsLoader;

    private string GetText()
    {
        return "Progression Max: " + levelsLoader.levelCurrent.ProgressionPercentMax + "%";
    }

    public void Start()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
        levelProgressionText.text = GetText();
    }

    public void Update()
    {
        levelProgressionText.text = GetText();
    }
}
