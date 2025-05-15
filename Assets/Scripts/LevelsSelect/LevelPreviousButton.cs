using UnityEngine;

public class LevelPreviousButton : MonoBehaviour
{
    public LevelsLoader levelsLoader;

    public void Start()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousLevel();
        }
    }

    public void PreviousLevel()
    {
        levelsLoader.PreviousLevel();
    }
}
