using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelEditor : MonoBehaviour
{
    [Header("Placement")]
    private GameObject currentBlock;
    private bool isPlacingBlock = false;
    private Vector3 currentScale = new Vector3(1f, 1f, 1);
    private float scaleStep = 0.1f;

    [Header("UI")]
    public Transform blockGroupContainer;
    public GameObject buttonPrefabTemplate;

    private int currentPage = 0;
    private const int buttonsPerPage = 4;

    private List<GameObject> blockPrefabs = new();
    private List<GameObject> currentButtons = new();

    private GameObject resizingTarget = null;
    private bool isResizing = false;
    private Vector3 originalMousePos;
    private Vector3 originalScale;

    private enum ResizeAxis { None, Horizontal, Vertical }
    private ResizeAxis currentResizeAxis = ResizeAxis.None;

    void Start()
    {
        LoadPrefabs();
        GenerateButtons();
    }

    void LoadPrefabs()
    {
        blockPrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs"));
    }

    void GenerateButtons()
    {
        if (buttonPrefabTemplate == null)
        {
            Debug.LogError("LevelEditor.GenerateButtons(): buttonPrefabTemplate n’est pas assigné !");
            return;
        }
        if (blockGroupContainer == null)
        {
            Debug.LogError("LevelEditor.GenerateButtons(): blockGroupContainer n’est pas assigné !");
            return;
        }
        ClearCurrentButtons();

        Transform container = blockGroupContainer;

        int start = currentPage * buttonsPerPage;
        int end = Mathf.Min(start + buttonsPerPage, blockPrefabs.Count);

        for (int i = start; i < end; i++)
        {
            GameObject button = Instantiate(buttonPrefabTemplate, container);
            button.SetActive(true);

            Transform canvas = button.transform.Find("Canvas");
            Transform bg = canvas?.Find("BlankSquare");
            Transform icon = canvas?.Find("PrefabIcon");

            if (bg == null || icon == null)
            {
                Destroy(button);
                continue;
            }

            float xOffset = -375f + (i - start) * 125f;
            bg.GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffset, bg.GetComponent<RectTransform>().anchoredPosition.y);
            icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffset, icon.GetComponent<RectTransform>().anchoredPosition.y);

            Image bgImage = bg.GetComponent<Image>();
            Image iconImage = icon.GetComponent<Image>();

            bgImage.sprite = Resources.Load<Sprite>("InGame/ButtonSkin/BlankSquare");
            iconImage.sprite = blockPrefabs[i].GetComponent<SpriteRenderer>()?.sprite;

            string prefabName = blockPrefabs[i].name.ToLower();
            if (prefabName.Contains("smallspike") || prefabName.Contains("smallobstacle"))
            {
                icon.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 25);
            }
            else
            {
                icon.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            }

            GameObject prefab = blockPrefabs[i];
            button.GetComponent<Button>().onClick.AddListener(() => SelectPrefab(prefab));
            currentButtons.Add(button);
        }
    }

    void ClearCurrentButtons()
    {
        foreach (var button in currentButtons)
        {
            Destroy(button);
        }

        currentButtons.Clear();
    }

    public void NextPage()
    {
        int maxPage = 3;
        if (currentPage < maxPage - 1)
        {
            currentPage++;
            GenerateButtons();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            GenerateButtons();
        }
    }

    void SelectPrefab(GameObject prefab)
    {
        if (isPlacingBlock)
        {
            return;
        }

        string name = prefab.name.ToLower();

        if (name.Contains("portal"))
        {
            currentScale = new Vector3(0.5f, 0.5f, 1);
        }
        else if (name.Contains("small"))
        {
            currentScale = new Vector3(0.15f, 0.07f, 1);
        }
        else if (name.Contains("spike"))
        {
            currentScale = new Vector3(0.15f, 0.15f, 1);
        }
        else if (name.Contains("block"))
        {

            currentScale = new Vector3(0.2f, 0.2f, 1);
        }
        else if (name.Contains("bonus"))
        {
            currentScale = new Vector3(0.3f, 0.3f, 1);
        }
        else
        {
            currentScale = new Vector3(1f, 1f, 1);
        }

        InstantiateAndPrepare(prefab, currentScale);
    }

    void Update()
    {
        if (isPlacingBlock && currentBlock != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentBlock.transform.position = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), -1);

            if (!currentBlock.name.ToLower().Contains("portal"))
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll != 0)
                {
                    float newScale = Mathf.Max(0.1f, currentScale.x + scroll * scaleStep);
                    currentScale = new Vector3(newScale, newScale, 1);
                    currentBlock.transform.localScale = currentScale;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                Collider2D[] overlaps = Physics2D.OverlapBoxAll(
                    currentBlock.transform.position,
                    currentBlock.GetComponent<Collider2D>().bounds.size,
                    0f
                );

                if (overlaps.Length > 1)
                {
                    return;
                }

                PlaceBlock();
            }
        }

        if (Input.GetMouseButtonDown(0) && !isPlacingBlock)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null)
            {
                resizingTarget = hit.gameObject;
                originalMousePos = mousePos;
                originalScale = resizingTarget.transform.localScale;

                Vector2 localClick = mousePos - (Vector2)resizingTarget.transform.position;
                float ratio = resizingTarget.GetComponent<Collider2D>().bounds.size.x /
                resizingTarget.GetComponent<Collider2D>().bounds.size.y;

                currentResizeAxis = Mathf.Abs(localClick.x) > Mathf.Abs(localClick.y * ratio)
                    ? ResizeAxis.Horizontal
                    : ResizeAxis.Vertical;

                isResizing = true;
            }
        }

        if (isResizing && resizingTarget != null)
        {
            Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 delta = currentMousePos - originalMousePos;

            if (currentResizeAxis == ResizeAxis.Horizontal)
            {
                float newScaleX = Mathf.Max(0.1f, originalScale.x + delta.x);
                resizingTarget.transform.localScale = new Vector3(newScaleX, originalScale.y, 1);
            }
            else if (currentResizeAxis == ResizeAxis.Vertical)
            {
                float newScaleY = Mathf.Max(0.1f, originalScale.y + delta.y);
                resizingTarget.transform.localScale = new Vector3(originalScale.x, newScaleY, 1);
            }

            if (Input.GetMouseButtonUp(0))
            {
                isResizing = false;
                resizingTarget = null;
                currentResizeAxis = ResizeAxis.None;
            }
        }
    }

    void PlaceBlock()
    {
        isPlacingBlock = false;
        currentBlock = null;
    }

    void InstantiateAndPrepare(GameObject prefab, Vector3? scaleOverride = null)
    {
        GameObject obj = Instantiate(prefab);
        obj.transform.position = new Vector3(0, 0, -1);
        obj.transform.localScale = scaleOverride ?? currentScale;

        currentBlock = obj;
        isPlacingBlock = true;
    }

    public void Save()
    {
    }
}
