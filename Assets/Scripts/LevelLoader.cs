using UnityEngine;
using System.IO;

public class LevelLoader : MonoBehaviour
{
    public LevelsLoader levelsLoader;
    public AudioSource audioSource;

    private GameObject GetPrefab(string type)
    {
        return Resources.Load<GameObject>("Prefabs/" + type);
    }

    private void LoadAudio()
    {
        audioSource.clip = Resources.Load<AudioClip>(Path.Combine("Musics", levelsLoader.levelCurrent.musicName));
        audioSource.Play();
    }

    private void LoadElements()
    {
        Level current = levelsLoader.levelCurrent;
        foreach (var element in current.elements)
        {
            GameObject prefab = GetPrefab(element.type);
            GameObject instance = Instantiate(prefab, new Vector3(element.x, element.y, 0), Quaternion.identity);

            Vector3 originalScale = instance.transform.localScale;
            float newScaleX = element.scaleX > 0 ? element.scaleX : originalScale.x;
            float newScaleY = element.scaleY > 0 ? element.scaleY : originalScale.y;

            instance.transform.localScale = new Vector3(newScaleX, newScaleY, originalScale.z);
        }

        LevelElement lastElement = current.elements[^1];
        float lastX = 15;
        if (lastElement != null)
        {
            lastX += lastElement.x;
        }
        Instantiate(GetPrefab("WinnerWall"), new Vector3(lastX, 0, 0), Quaternion.Euler(0, 0, 90));
    }

    public void Start()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
        levelsLoader.IncreaseTotalAttempts();

        LoadAudio();
        LoadElements();
    }

    public void Update()
    {

    }
}
