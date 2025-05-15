using UnityEngine;

public class LevelNextButton : MonoBehaviour
{
    public LevelsLoader levelsLoader;

    public void Start()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextLevel();
        }
    }

    public void NextLevel()
    {
        levelsLoader.NextLevel();
    }
}
