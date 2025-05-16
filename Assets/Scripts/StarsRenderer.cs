using UnityEngine;
using UnityEngine.UI;

public class StarsRenderer : MonoBehaviour
{
    public Image starTemplate;
    public RectTransform starsContainer;
    public LevelsLoader levelsLoader;

    public float extraPadding = 10f;

    private bool useManualMode = false;
    private int manualDifficulty = 1;

    private float starSpacing;
    private int lastRenderedDifficulty = -1;

    void Start()
    {
        if (starTemplate == null || starsContainer == null)
        {
            Debug.LogError("Star template ou container manquant");
            enabled = false;
            return;
        }
        starTemplate.gameObject.SetActive(false);

        if (levelsLoader == null)
        {
            var loaderObj = GameObject.FindGameObjectWithTag("LevelsLoader");
            if (loaderObj != null)
                levelsLoader = loaderObj.GetComponent<LevelsLoader>();
        }

        starSpacing = starTemplate.rectTransform.sizeDelta.x + extraPadding;
        RenderStarsInternal(GetCurrentDifficulty());
    }

    void Update()
    {
        int diff = GetCurrentDifficulty();
        if (diff != lastRenderedDifficulty)
            RenderStarsInternal(diff);
    }

    public void SetManualDifficulty(int difficulty)
    {
        useManualMode = true;
        manualDifficulty = Mathf.Clamp(difficulty, 1, 5);
        RenderStarsInternal(manualDifficulty);
    }

    public void UseAutomaticMode()
    {
        useManualMode = false;
        RenderStarsInternal(GetCurrentDifficulty());
    }

    public int GetCurrentDifficulty()
    {
        if (useManualMode)
            return manualDifficulty;
        if (levelsLoader != null && levelsLoader.levelCurrent != null)
            return Mathf.Clamp(levelsLoader.levelCurrent.difficulty, 1, 5);
        return 1;
    }


    private void RenderStarsInternal(int difficulty)
    {
        for (int i = starsContainer.childCount - 1; i >= 0; i--)
        {
            var child = starsContainer.GetChild(i);
            if (child.gameObject != starTemplate.gameObject)
                Destroy(child.gameObject);
        }

        lastRenderedDifficulty = difficulty;

        float totalWidth = difficulty * starSpacing - extraPadding;
        float startX = -totalWidth / 2 + starTemplate.rectTransform.sizeDelta.x / 2;

        for (int i = 0; i < difficulty; i++)
        {
            var star = Instantiate(starTemplate, starsContainer);
            star.gameObject.SetActive(true);
            star.rectTransform.anchoredPosition = new Vector2(startX + i * starSpacing, 0f);
            star.rectTransform.SetAsLastSibling();
        }
    }
}
