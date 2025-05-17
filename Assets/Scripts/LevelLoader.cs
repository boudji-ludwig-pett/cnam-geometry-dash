using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LevelLoader : MonoBehaviour
{
    public LevelsLoader levelsLoader;
    public bool editMode;
    public bool createMode;
    public AudioSource musicSource;
    public Text progressionText;
    private readonly float groundY = -6.034f;

    private GameObject GetPrefab(string type)
    {
        var prefab = Resources.Load<GameObject>("Prefabs/" + type);
        if (prefab == null)
        {
            Debug.LogError($"Prefab introuvable pour : {type}");
        }
        return prefab;
    }

    private void LoadAudio()
    {
        musicSource.clip = Resources.Load<AudioClip>(Path.Combine("Musics", levelsLoader.levelCurrent.musicName));
        if (editMode)
        {
            return;
        }
        if (PlayerPrefs.HasKey("Volume"))
        {
            musicSource.volume = PlayerPrefs.GetFloat("Volume");
        }
        else
        {
            musicSource.volume = 1f;
        }
        musicSource.Play();
    }

    private void LoadElements()
    {
        Level current = levelsLoader.levelCurrent;
        foreach (var element in current.elements)
        {
            GameObject prefab = GetPrefab(element.type);
            if (prefab == null) continue;

            GameObject instance = Instantiate(
                prefab,
                new Vector3(element.x, element.y, 0),
                Quaternion.identity
            );

            if (editMode)
            {
                foreach (Transform child in instance.transform)
                {
                    if (child.name.Contains("ObstacleKiller"))
                    {
                        var col = child.GetComponent<BoxCollider2D>();
                        if (col != null && col.size.y > 2f) // Trop grand
                        {
                            Debug.LogWarning($"⚠️ Collider {child.name} trop grand, réduction appliquée.");
                            col.size = new Vector2(col.size.x, 1f);
                            col.offset = new Vector2(col.offset.x, -2f); // Ajuste selon ton design
                        }
                    }
                }
            }

            // En mode jeu/test uniquement → ajout du AICollider
            if (!editMode)
            {
                Instantiate(
                    Resources.Load<GameObject>("AICollider"),
                    new Vector3(element.x - 1, element.y, 0),
                    Quaternion.identity
                );
            }

            // Appliquer l'échelle personnalisée
            Vector3 originalScale = instance.transform.localScale;
            float newScaleX = element.scaleX > 0 ? element.scaleX : originalScale.x;
            float newScaleY = element.scaleY > 0 ? element.scaleY : originalScale.y;
            instance.transform.localScale = new Vector3(newScaleX, newScaleY, originalScale.z);
        }

        // Sol uniquement en mode jeu
        if (!editMode)
        {
            GameObject groundPrefab = GetPrefab("Ground");
            if (groundPrefab != null)
            {
                GameObject groundInstance = Instantiate(
                    groundPrefab,
                    new Vector3(current.LastX / 2, groundY, 0),
                    Quaternion.identity
                );
                groundInstance.transform.localScale = new Vector3(current.LastX / 5f * 2, 1, 1);
            }

            GameObject winWall = GetPrefab("WinnerWall");
            Instantiate(
                winWall,
                new Vector3(current.LastX, 0, 0),
                Quaternion.Euler(0, 0, 90)
            );
        }
    }

    private void Awake()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
        Level current = levelsLoader.levelCurrent;
        createMode = PlayerPrefs.GetInt("CreateMode", 0) == 1;
        if (!editMode)
        {
            GameObject groundPrefab = GetPrefab("Ground");
            GameObject groundInstance = Instantiate(groundPrefab, new Vector3(current.LastX / 2, groundY, 0), Quaternion.identity);
            float groundWidth = current.LastX;
            groundInstance.transform.localScale = new Vector3(groundWidth / 5f * 2, 1, 1);
            Instantiate(GetPrefab("WinnerWall"), new Vector3(current.LastX, 0, 0), Quaternion.Euler(0, 0, 90));
        }
    }

    public void Start()
    {
        if (!createMode)
        {
            levelsLoader = GameObject
                .FindGameObjectWithTag("LevelsLoader")
                .GetComponent<LevelsLoader>();

            levelsLoader.IncreaseTotalAttempts();

            LoadElements();
            LoadAudio();
        }
    }

    public void Update()
    {
        if (!editMode)
        {
            progressionText.text = levelsLoader.levelCurrent.ProgressionPercent + "%";
        }
    }
}
