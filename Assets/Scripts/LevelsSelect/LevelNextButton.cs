using UnityEngine;

public class LevelNextButton : MonoBehaviour
{
    public LevelsLoader levelsLoader;

    public void Start()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
    }

    public void NextLevel()
    {
        levelsLoader.NextLevel();
    }
}
