using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelTotalJumps : MonoBehaviour
{
    public Text levelTotalJumpsText;

    public LevelsLoader levelsLoader;

    private string GetText()
    {
        int number = levelsLoader.levelCurrent.TotalJumps;
        FormattableString message = $"{number:N0}";
        return "Total Jumps: " + FormattableString.Invariant(message);
    }

    public void Start()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
        levelTotalJumpsText.text = GetText();
    }

    public void Update()
    {
        levelTotalJumpsText.text = GetText();
    }
}
