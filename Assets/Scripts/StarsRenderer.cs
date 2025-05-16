using UnityEngine;
using UnityEngine.UI;

public class StarsRenderer : MonoBehaviour
{
    public Image starTemplate;
    public RectTransform starsContainer;
    public LevelsLoader levelsLoader;

    public float extraPadding = 10f;

    private float starSpacing;
    private int lastRenderedDifficulty = -1;

    void Start()
    {
        if (starTemplate == null || starsContainer == null)
        {
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
        if (levelsLoader == null || levelsLoader.levelCurrent == null)
        {
            enabled = false;
            return;
        }

        float templateWidth = starTemplate.rectTransform.sizeDelta.x;
        starSpacing = templateWidth + extraPadding;

        RenderStars();
    }

    void Update()
    {
        int currentDifficulty = Mathf.Clamp(levelsLoader.levelCurrent.difficulty, 1, 5);
        if (currentDifficulty != lastRenderedDifficulty)
        {
            RenderStars();
        }
    }

    private void RenderStars()
    {
        string lvlName = levelsLoader.levelCurrent.name;
        int difficulty = Mathf.Clamp(levelsLoader.levelCurrent.difficulty, 1, 5);

        for (int i = starsContainer.childCount - 1; i >= 0; i--)
        {
            var child = starsContainer.GetChild(i);
            if (child.gameObject != starTemplate.gameObject)
            {
                Destroy(child.gameObject);
            }
        }

        lastRenderedDifficulty = difficulty;

        float totalWidth = difficulty * starSpacing - extraPadding;
        float startX = -totalWidth / 2 + starTemplate.rectTransform.sizeDelta.x / 2;

        for (int i = 0; i < difficulty; i++)
        {
            var star = Instantiate(starTemplate, starsContainer);
            star.gameObject.SetActive(true);

            float posX = startX + i * starSpacing;
            Vector2 anchoredPos = new Vector2(posX, 0f);
            star.rectTransform.anchoredPosition = anchoredPos;
            star.rectTransform.SetAsLastSibling();
        }
    }
}
