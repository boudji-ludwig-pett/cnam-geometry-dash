using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using SimpleFileBrowser;
using TMPro;

[RequireComponent(typeof(LevelEditor))]
public class JSONExporter : MonoBehaviour
{
    public TMP_Text statusText;
    private LevelEditor editor;
    private string levelsFolder;

    private void Awake()
    {
        editor = GetComponent<LevelEditor>();
        levelsFolder = Path.Combine(Application.dataPath, "Resources/Levels");
        if (!Directory.Exists(levelsFolder))
            Directory.CreateDirectory(levelsFolder);

        if (statusText == null)
        {
            var statusObj = GameObject.Find("StatusText");
            if (statusObj != null)
                statusText = statusObj.GetComponent<TMP_Text>();
        }
    }

    private void Start()
    {
        SetStatus("Ready to export...", Color.white);
    }

    public void ExportJSON()
    {
        SetStatus("Exporting...", Color.yellow);
        StartCoroutine(ShowSaveDialog());
    }

    private IEnumerator ShowSaveDialog()
    {
        yield return FileBrowser.WaitForSaveDialog(
            FileBrowser.PickMode.Files,
            false,
            levelsFolder,
            "NewLevel.json",
            "Save Level JSON",
            "Save"
        );

        if (!FileBrowser.Success)
        {
            SetStatus("Save canceled.", Color.red);
            yield break;
        }

        string chosenPath = FileBrowser.Result[0];
        string fileName = Path.GetFileNameWithoutExtension(chosenPath);
        string destPath = Path.Combine(levelsFolder, fileName + ".json");

        var elements = new List<SerializableElement>();
        var allCols = Object.FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
        foreach (var col in allCols)
        {
            var go = col.gameObject;
            if (!go.name.Contains("(Clone)") || go.name.ToLower().Contains("ground"))
                continue;

            Vector3 scale = go.transform.localScale;
            Vector3 pos = go.transform.position;

            elements.Add(new SerializableElement
            {
                type = go.name.Replace("(Clone)", ""),
                x = Mathf.Round(pos.x * 100f) / 100f,
                y = Mathf.Round(pos.y * 100f) / 100f,
                scaleX = Mathf.Round(scale.x * 100f) / 100f,
                scaleY = Mathf.Round(scale.y * 100f) / 100f
            });
        }

        if (elements.Count == 0)
        {
            SetStatus("No elements to export.", Color.red);
            yield break;
        }

        LevelData data = new LevelData
        {
            name = fileName,
            musicName = "",
            order = 0,
            elements = elements.ToArray()
        };
        string json = JsonUtility.ToJson(data, prettyPrint: true);

        try
        {
            File.WriteAllText(destPath, json);
            SetStatus("Export successful: " + fileName + ".json", Color.green);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Export error: " + e);
            SetStatus("Export error. See console.", Color.red);
        }

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
        var loader = Object.FindAnyObjectByType<LevelsLoader>();
        if (loader != null)
            loader.RefreshLevels();
    }

    private void SetStatus(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color;
            statusText.gameObject.SetActive(false);
            statusText.gameObject.SetActive(true);
            Canvas.ForceUpdateCanvases();
        }
    }

    [System.Serializable]
    private class SerializableElement
    {
        public string type;
        public float x;
        public float y;
        public float scaleX;
        public float scaleY;
    }

    [System.Serializable]
    private class LevelData
    {
        public string name;
        public string musicName;
        public int order;
        public SerializableElement[] elements;
    }
}
