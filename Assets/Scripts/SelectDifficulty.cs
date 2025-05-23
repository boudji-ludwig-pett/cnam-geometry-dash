using UnityEngine;
using UnityEngine.UI;

public class SelectDifficulty : MonoBehaviour
{
    public GameObject selectDifficultyPanel;
    public GameObject mainSaveButton;
    public StarsRenderer starsRenderer;
    public JSONExporter jsonExporter;

    private int currentDifficulty;
    private const int MinDiff = 1;
    private const int MaxDiff = 5;

    void Awake()
    {
        if (selectDifficultyPanel == null)
        {
            selectDifficultyPanel = GameObject.Find("SelectDifficultyPanel");
        }
        if (mainSaveButton == null)
        {
            mainSaveButton = GameObject.Find("MainSaveButton");
        }
    }

    void Start()
    {
        currentDifficulty = starsRenderer != null
            ? starsRenderer.GetCurrentDifficulty()
            : MinDiff;
        currentDifficulty = Mathf.Clamp(currentDifficulty, MinDiff, MaxDiff);

        UpdateUI();
    }

    private void UpdateUI()
    {
        starsRenderer.UseAutomaticMode();
        starsRenderer?.SetManualDifficulty(currentDifficulty);
    }

    public void OpenSelectDifficulty()
    {
        selectDifficultyPanel.SetActive(true);
        mainSaveButton.SetActive(false);
        UpdateUI();
    }


    public void PreviousDifficulty()
    {
        if (currentDifficulty > MinDiff)
        {
            currentDifficulty--;
            UpdateUI();
        }
    }

    public void NextDifficulty()
    {
        if (currentDifficulty < MaxDiff)
        {
            currentDifficulty++;
            UpdateUI();
        }
    }

    public void Cancel()
    {
        selectDifficultyPanel.SetActive(false);
        mainSaveButton.SetActive(true);
    }
}
