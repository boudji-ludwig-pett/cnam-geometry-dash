using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelTotalAttempts : MonoBehaviour
{
    public Text levelTotalAttemptsText;

    public LevelsLoader levelsLoader;

    private string GetText()
    {
        int number = levelsLoader.levelCurrent.TotalAttempts;
        FormattableString message = $"{number:N0}";
        return "Total Attempts: " + FormattableString.Invariant(message);
    }

    public void Start()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
        levelTotalAttemptsText.text = GetText();
    }

    public void Update()
    {
        levelTotalAttemptsText.text = GetText();
    }
}
