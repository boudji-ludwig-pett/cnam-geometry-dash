using UnityEngine;
using System.IO;
using System.Collections;
using SimpleFileBrowser;
using UnityEngine.SceneManagement;
using TMPro;

public class JSONImporter : MonoBehaviour
{
    public TMP_Text statusText;

    private void Awake()
    {
        if (statusText == null)
        {
            GameObject statusObj = GameObject.Find("StatusText");
            if (statusObj != null)
            {
                statusText = statusObj.GetComponent<TMP_Text>();
                if (statusText != null)
                {
                    Debug.Log("✅ StatusText found and assigned automatically!");
                }
                else
                {
                    Debug.LogError("⚠️ 'StatusText' was found but does not have a TMP_Text component!");
                }
            }
            else
            {
                Debug.LogError("⚠️ No GameObject named 'StatusText' found in the scene. Please create a TextMeshPro element and name it 'StatusText'.");
            }
        }
    }

    private void Start()
    {
        if (statusText != null)
        {
            statusText.text = "Ready to import...";
            statusText.color = Color.white;
        }
        else
        {
            Debug.LogError("statusText is not assigned!");
        }
    }

    public void ImportJSON()
    {
        Debug.Log("Button clicked, starting import...");
        if (statusText != null)
        {
            statusText.text = "Importing...";
            statusText.color = Color.yellow;
        }
        StartCoroutine(ShowFileBrowser());
    }

    private IEnumerator ShowFileBrowser()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Select JSON File", "Load");

        if (FileBrowser.Success)
        {
            string sourcePath = FileBrowser.Result[0];

            if (Path.GetExtension(sourcePath).ToLower() != ".json")
            {
                UpdateStatus("Invalid file. Please select a JSON file.", Color.red);
                yield break;
            }

            string fileName = Path.GetFileName(sourcePath);
            string destinationPath = Path.Combine(Application.dataPath, "Resources/Levels", fileName);

            bool success = false;
            try
            {
                File.Copy(sourcePath, destinationPath, true);
                success = true;
            }
            catch (IOException e)
            {
                Debug.LogError("Error copying file: " + e.Message);
            }

            if (success)
            {
                UpdateStatus("Import successful!", Color.green);
            }
            else
            {
                UpdateStatus("Import error.", Color.red);
            }

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            LevelsLoader loader = Object.FindAnyObjectByType<LevelsLoader>();
            if (loader != null)
            {
                loader.RefreshLevels();
            }
        }
        else
        {
            UpdateStatus("No file selected.", Color.red);
        }
        yield return null;
    }

    private void UpdateStatus(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color;
            statusText.gameObject.SetActive(false);
            statusText.gameObject.SetActive(true);
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            Debug.LogError("statusText is NULL!");
        }
    }
}
