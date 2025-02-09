using UnityEngine;

public class LevelPreviousButton : MonoBehaviour
{
    public LevelsLoader levelsLoader;

    public void Start()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
    }

    public void PreviousLevel()
    {
        levelsLoader.PreviousLevel();
    }
}
