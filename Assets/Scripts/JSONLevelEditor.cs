using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using SimpleFileBrowser;
using TMPro;

[System.Serializable]
public class BlockData
{
    public string prefabName;
    public Vector3 position;
    public float rotationZ;
    public Vector3 scale;
}

[System.Serializable]
public class LevelData
{
    public string levelName;
    public List<BlockData> blocks = new List<BlockData>();
}

public class JSONLevelEditor : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown levelDropdown;
    public Button loadButton;
    public Button saveButton;
    public TMP_Text statusText;

    [Header("Editor")]
    public Transform editRoot; // où on instancie les blocs
    public List<GameObject> blockPrefabs; // à remplir dans l’inspecteur (mêmes noms que dans JSON)

    private LevelData currentLevel;
    private string currentJsonPath;

    private void Start()
    {
        // Remplir la dropdown avec les JSON disponibles dans Resources/Levels
        var assets = Resources.LoadAll<TextAsset>("Levels");
        levelDropdown.options.Clear();
        foreach (var txt in assets)
            levelDropdown.options.Add(new TMP_Dropdown.OptionData(txt.name));
        levelDropdown.RefreshShownValue();

        loadButton.onClick.AddListener(OnLoadClicked);
        saveButton.onClick.AddListener(OnSaveClicked);
        UpdateStatus("Prêt.", Color.white);
    }

    private void OnLoadClicked()
    {
        string lvlName = levelDropdown.options[levelDropdown.value].text;
        TextAsset json = Resources.Load<TextAsset>($"Levels/{lvlName}");
        if (json == null)
        {
            UpdateStatus($"Niveau '{lvlName}' introuvable.", Color.red);
            return;
        }

        currentJsonPath = Path.Combine(Application.dataPath, "Resources/Levels", lvlName + ".json");
        currentLevel = JsonUtility.FromJson<LevelData>(json.text);
        if (currentLevel == null)
        {
            UpdateStatus("Impossible de parser le JSON.", Color.red);
            return;
        }

        ClearEditRoot();
        foreach (var bd in currentLevel.blocks)
        {
            var prefab = blockPrefabs.Find(p => p.name == bd.prefabName);
            if (prefab == null) continue;
            var go = Instantiate(prefab, editRoot);
            go.transform.localPosition = bd.position;
            go.transform.localEulerAngles = new Vector3(0, 0, bd.rotationZ);
            go.transform.localScale = bd.scale;
            // vous pouvez ajouter un script pour manipuler ce 'go' dans l’éditeur
        }

        UpdateStatus($"Niveau '{lvlName}' chargé.", Color.green);
    }

    private void OnSaveClicked()
    {
        if (currentLevel == null)
        {
            UpdateStatus("Aucun niveau chargé.", Color.red);
            return;
        }

        // Reconstruire le LevelData depuis les enfants de editRoot
        currentLevel.blocks.Clear();
        foreach (Transform child in editRoot)
        {
            var bd = new BlockData
            {
                prefabName = child.name.Replace("(Clone)", ""),
                position = child.localPosition,
                rotationZ = child.localEulerAngles.z,
                scale = child.localScale
            };
            currentLevel.blocks.Add(bd);
        }

        // Demande path de sauvegarde
        StartCoroutine(DoSaveDialog());
    }

    private IEnumerator DoSaveDialog()
    {
        yield return FileBrowser.WaitForSaveDialog(
            FileBrowser.PickMode.Files, false, null, "json",
            "Save JSON", "Save");

        if (!FileBrowser.Success)
        {
            UpdateStatus("Sauvegarde annulée.", Color.red);
            yield break;
        }

        string dest = FileBrowser.Result[0];
        if (!dest.EndsWith(".json")) dest += ".json";

        string jsonText = JsonUtility.ToJson(currentLevel, true);
        try
        {
            File.WriteAllText(dest, jsonText);
            UpdateStatus($"Sauvegardé : {dest}", Color.green);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            UpdateStatus("Erreur lors de la sauvegarde.", Color.red);
        }
    }

    private void ClearEditRoot()
    {
        for (int i = editRoot.childCount - 1; i >= 0; i--)
            DestroyImmediate(editRoot.GetChild(i).gameObject);
    }

    private void UpdateStatus(string msg, Color col)
    {
        if (statusText == null) return;
        statusText.text = msg;
        statusText.color = col;
    }
}
