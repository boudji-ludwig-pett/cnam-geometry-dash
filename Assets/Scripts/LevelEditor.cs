using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelEditor : MonoBehaviour
{
    [Header("Placement")]
    public Transform mapParent;
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
        ClearCurrentButtons();

        Transform container = blockGroupContainer;

        if (container == null || buttonPrefabTemplate == null)
        {
            Debug.LogError("UI Container ou prefab de bouton manquant.");
            return;
        }

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
                icon.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 25);
            else
                icon.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);

            GameObject prefab = blockPrefabs[i];
            button.GetComponent<Button>().onClick.AddListener(() => SelectPrefab(prefab));
            currentButtons.Add(button);
        }
    }

    void ClearCurrentButtons()
    {
        foreach (var button in currentButtons)
            Destroy(button);

        currentButtons.Clear();
    }

    public void NextPage()
    {
        int maxPage = 3;
        Debug.Log(currentPage);
        if (currentPage < maxPage - 1)
        {
            currentPage++;
            GenerateButtons();
        }
    }

    public void PreviousPage()
    {
        Debug.Log(currentPage);
        if (currentPage > 0)
        {
            currentPage--;
            GenerateButtons();
        }
    }

    void SelectPrefab(GameObject prefab)
    {
        if (isPlacingBlock) return;

        string name = prefab.name.ToLower();

        if (name.Contains("portal"))
            currentScale = new Vector3(0.5f, 0.5f, 1);
        else if (name.Contains("small"))
            currentScale = new Vector3(0.15f, 0.07f, 1);
        else if (name.Contains("spike"))
            currentScale = new Vector3(0.15f, 0.15f, 1);
        else if (name.Contains("block"))
            currentScale = new Vector3(0.2f, 0.2f, 1);
        else if (name.Contains("bonus"))
            currentScale = new Vector3(0.3f, 0.3f, 1);
        else
            currentScale = new Vector3(1f, 1f, 1);

        InstantiateAndPrepare(prefab, currentScale);
    }
    void Update()
    {
        // Déplacement de l'objet en cours de placement
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
                    Debug.Log("Placement annulé : un objet est déjà présent à cet endroit.");
                    return;
                }

                PlaceBlock();
            }
        }
        else if (Input.GetMouseButtonDown(0)) // Clic gauche pour reprendre un objet déjà placé
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null && hit.transform != null && hit.transform.parent == mapParent)
            {
                if (hit.CompareTag("Ground"))
                {
                    Debug.Log("Impossible de déplacer le sol (tag Ground).");
                    return;
                }
                currentBlock = hit.gameObject;
                isPlacingBlock = true;
                currentScale = currentBlock.transform.localScale;
                Debug.Log($"Déplacement de l'objet : {currentBlock.name}");
                return;
            }
        }

        // Redimensionnement d'un objet déjà placé
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift) && !isPlacingBlock)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null && hit.transform != null && hit.transform.parent == mapParent && !hit.CompareTag("Ground"))
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
                Debug.Log($"Début de redimensionnement : {resizingTarget.name}, axe = {currentResizeAxis}");
            }
        }

        if (isResizing && resizingTarget != null)
        {
            Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 delta = currentMousePos - originalMousePos;

            Vector3 newScale = originalScale;

            if (currentResizeAxis == ResizeAxis.Horizontal)
                newScale.x = Mathf.Max(0.1f, originalScale.x + delta.x);
            else if (currentResizeAxis == ResizeAxis.Vertical)
                newScale.y = Mathf.Max(0.1f, originalScale.y + delta.y);

            // Temporarily apply the new scale for collision testing
            Vector3 originalPos = resizingTarget.transform.position;
            resizingTarget.transform.localScale = newScale;

            Bounds bounds = resizingTarget.GetComponent<Collider2D>().bounds;
            Collider2D[] overlaps = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0f);

            bool hasCollision = false;
            foreach (var col in overlaps)
            {
                if (col.gameObject != resizingTarget && col.transform.parent == mapParent)
                {
                    hasCollision = true;
                    break;
                }
            }

            if (hasCollision)
            {
                resizingTarget.transform.localScale = originalScale; // revert
                Debug.Log("Étirement annulé : collision détectée.");
            }

            if (Input.GetMouseButtonUp(0))
            {
                isResizing = false;
                resizingTarget = null;
                currentResizeAxis = ResizeAxis.None;
            }
        }


        // Clic droit pour supprimer un objet déjà placé (sauf le sol)
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null && hit.transform != null && hit.transform.parent == mapParent)
            {
                if (hit.CompareTag("Ground"))
                {
                    Debug.Log("Impossible de supprimer le sol (tag Ground).");
                    return;
                }

                Destroy(hit.gameObject);
                Debug.Log($"Objet supprimé : {hit.name}");
            }
        }

    }

    void PlaceBlock()
    {
        if (currentBlock.name.ToLower().Contains("smallobstacle") || currentBlock.name.ToLower().Contains("portal"))
        {
            // Pas de snap pour les small blocks
            isPlacingBlock = false;
            currentBlock = null;
            return;
        }

        // Calcul de la position au sol ou sur le bloc le plus bas
        Vector2 origin = currentBlock.transform.position;
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, Vector2.down, 100f);

        float highestY = -Mathf.Infinity;
        GameObject bestTarget = null;

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != currentBlock)
            {
                float topOfObject = hit.collider.bounds.max.y;
                if (topOfObject > highestY)
                {
                    highestY = topOfObject;
                    bestTarget = hit.collider.gameObject;
                }
            }
        }

        // Si on a trouvé un bloc en dessous, on s'aligne sur le dessus
        if (bestTarget != null)
        {
            float height = currentBlock.GetComponent<Collider2D>().bounds.size.y;
            currentBlock.transform.position = new Vector3(currentBlock.transform.position.x, highestY + height / 2f, -1);
        }
        else
        {
            // Sinon on pose au sol (y = 0)
            float height = currentBlock.GetComponent<Collider2D>().bounds.size.y;
            currentBlock.transform.position = new Vector3(currentBlock.transform.position.x, height / 2f, -1);
        }

        isPlacingBlock = false;
        currentBlock = null;
    }

    void InstantiateAndPrepare(GameObject prefab, Vector3? scaleOverride = null)
    {
        GameObject obj = Instantiate(prefab);
        obj.transform.position = new Vector3(0, 0, -1);
        obj.transform.localScale = scaleOverride ?? currentScale;

        try { obj.tag = prefab.name; }
        catch { Debug.LogWarning($"Le tag '{prefab.name}' n'existe pas. Ajoutez-le dans Project Settings > Tags."); }

        if (mapParent != null)
            obj.transform.SetParent(mapParent);

        currentBlock = obj;
        isPlacingBlock = true;
    }

    public void Save()
    {
        // TODO : Implémenter la sauvegarde du niveau
    }
}
