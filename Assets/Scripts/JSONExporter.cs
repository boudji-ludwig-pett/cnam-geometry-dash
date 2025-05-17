using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using SimpleFileBrowser;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(LevelEditor))]
public class JSONExporter : MonoBehaviour
{
    public TMP_Text statusText;
    public int difficultyToExport = 1;
    private LevelEditor editor;
    private string levelsFolder;
    private string assetFolderPath;

    private void Awake()
    {
        editor = GetComponent<LevelEditor>();
        levelsFolder = Path.Combine(Application.dataPath, "Resources/Levels");
        if (!Directory.Exists(levelsFolder))
            Directory.CreateDirectory(levelsFolder);

        assetFolderPath = "Assets/Resources/Levels";

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
                scaleY = Mathf.Round(scale.y * 100f) / 100f,
                rotationZ = Mathf.Round(go.transform.rotation.eulerAngles.z * 100f) / 100f
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
            difficulty = difficultyToExport,
            elements = elements.ToArray()
        };
        string json = JsonUtility.ToJson(data, prettyPrint: true);

        try
        {
            File.WriteAllText(destPath, json);
            SetStatus($"Export successful: {fileName}.json (diff {difficultyToExport})", Color.green);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Export error: " + e);
            SetStatus("Export error. See console.", Color.red);
            yield break;
        }

#if UNITY_EDITOR
        string assetPath = Path.Combine(assetFolderPath, fileName + ".json");
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
#endif

        var loader = FindObjectOfType<LevelsLoader>();
        loader?.RefreshLevels();
    }

    private void SetStatus(string message, Color color)
    {
        if (statusText == null) return;
        statusText.text = message;
        statusText.color = color;
        statusText.gameObject.SetActive(false);
        statusText.gameObject.SetActive(true);
        Canvas.ForceUpdateCanvases();
    }

    [System.Serializable]
    private class SerializableElement
    {
        public string type;
        public float x, y, scaleX, scaleY, rotationZ;
    }

    [System.Serializable]
    private class LevelData
    {
        public string name;
        public string musicName;
        public int order;
        public int difficulty;
        public SerializableElement[] elements;
    }
}
