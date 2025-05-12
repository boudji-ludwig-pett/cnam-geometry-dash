using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

    void Update()
    {
        if (IsPointerOverUI()) return;

        if (isPlacingBlock && currentBlock != null)
            HandleBlockPlacement();
        else
            HandleBlockSelection();

        HandleBlockResizing();
        HandleBlockDeletion();
    }

    #region UI

    void LoadPrefabs()
    {
        blockPrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs"));
    }

    void GenerateButtons()
    {
        ClearCurrentButtons();

        if (blockGroupContainer == null || buttonPrefabTemplate == null)
        {
            Debug.LogError("UI Container ou prefab de bouton manquant.");
            return;
        }

        int start = currentPage * buttonsPerPage;
        int end = Mathf.Min(start + buttonsPerPage, blockPrefabs.Count);

        for (int i = start; i < end; i++)
        {
            GameObject button = Instantiate(buttonPrefabTemplate, blockGroupContainer);
            button.SetActive(true);

            SetupButtonVisual(button.transform, blockPrefabs[i], i - start);

            GameObject prefab = blockPrefabs[i];
            button.GetComponent<Button>().onClick.AddListener(() => SelectPrefab(prefab));
            currentButtons.Add(button);
        }
    }

    void SetupButtonVisual(Transform buttonTransform, GameObject prefab, int index)
    {
        Transform canvas = buttonTransform.Find("Canvas");
        Transform bg = canvas?.Find("BlankSquare");
        Transform icon = canvas?.Find("PrefabIcon");

        if (bg == null || icon == null)
        {
            Destroy(buttonTransform.gameObject);
            return;
        }

        float xOffset = -375f + index * 125f;
        bg.GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffset, bg.GetComponent<RectTransform>().anchoredPosition.y);
        icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffset, icon.GetComponent<RectTransform>().anchoredPosition.y);

        bg.GetComponent<Image>().sprite = Resources.Load<Sprite>("InGame/ButtonSkin/BlankSquare");
        icon.GetComponent<Image>().sprite = prefab.GetComponent<SpriteRenderer>()?.sprite;

        icon.GetComponent<RectTransform>().sizeDelta = prefab.name.ToLower().Contains("small")
            ? new Vector2(50, 25)
            : new Vector2(50, 50);
    }

    void ClearCurrentButtons()
    {
        foreach (var button in currentButtons)
            Destroy(button);
        currentButtons.Clear();
    }

    public void NextPage()
    {
        int maxPage = Mathf.CeilToInt(blockPrefabs.Count / (float)buttonsPerPage);
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

    #endregion

    #region Placement

    void SelectPrefab(GameObject prefab)
    {
        if (isPlacingBlock) return;

        currentScale = DetermineScaleFromName(prefab.name);
        InstantiateAndPrepare(prefab, currentScale);
    }

    Vector3 DetermineScaleFromName(string name)
    {
        name = name.ToLower();

        if (name.Contains("portal")) return new Vector3(0.5f, 0.5f, 1);
        if (name.Contains("small")) return new Vector3(0.15f, 0.07f, 1);
        if (name.Contains("spike")) return new Vector3(0.15f, 0.15f, 1);
        if (name.Contains("block")) return new Vector3(0.2f, 0.2f, 1);
        if (name.Contains("bonus")) return new Vector3(0.3f, 0.3f, 1);

        return new Vector3(1f, 1f, 1);
    }

    void HandleBlockPlacement()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentBlock.transform.position = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), -1);

        if (Input.GetKeyDown(KeyCode.R))
            HandleBlockRotation();

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
            if (!IsPlacementValid())
            {
                Debug.Log("Placement annulé : collision.");
                return;
            }

            PlaceBlock();
        }
    }

    bool IsPlacementValid()
    {
        Collider2D[] overlaps = Physics2D.OverlapBoxAll(
            currentBlock.transform.position,
            currentBlock.GetComponent<Collider2D>().bounds.size,
            0f
        );

        foreach (var col in overlaps)
        {
            if (col.transform.root == currentBlock.transform || col.CompareTag("Ground"))
                continue;
            return false;
        }
        return true;
    }

    void HandleBlockSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null && !hit.CompareTag("Ground"))
            {
                GameObject selected = hit.gameObject;

                // ✅ Cas spécial : sélectionner le parent ObstacleBlock si clic sur enfant
                if (selected.name.Contains("ObstacleSafer") || selected.name.Contains("ObstacleKiller"))
                {
                    Transform parent = selected.transform.parent;
                    if (parent != null && parent.name.Contains("ObstacleBlock"))
                    {
                        selected = parent.gameObject;
                    }
                }

                currentBlock = selected;
                isPlacingBlock = true;
                currentScale = currentBlock.transform.localScale;
                Debug.Log($"🟢 Bloc sélectionné : {currentBlock.name}");
            }
        }
    }


    void PlaceBlock()
    {
        string name = currentBlock.name.ToLower();

        bool grounded = name.Contains("spike") || name.Contains("bonus") || name.Contains("kill");

        if (grounded)
        {
            StickBlockToGround();
        }
        else if (!ShouldSkipVerticalSnap(name))
        {
            SnapBlockVertically();
            TrySnapToNearbyBlock(); // ✅ seulement ici
        }
        else
        {
            TrySnapToNearbyBlock(); // ex: autres blocs
        }


        TrySnapToNearbyBlock();

        isPlacingBlock = false;
        currentBlock = null;
    }
    void StickBlockToGround()
    {
        Collider2D col = currentBlock.GetComponent<Collider2D>();
        Bounds bounds = col.bounds;

        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y);
        float rayLength = 100f; // on descend loin pour être sûr de toucher le sol

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength);

        if (hit.collider != null && hit.collider.gameObject != currentBlock)
        {
            float topY = hit.collider.bounds.max.y;
            float height = bounds.size.y;
            float newY = topY + height / 2f;

            currentBlock.transform.position = new Vector3(currentBlock.transform.position.x, newY, -1f);
            Debug.Log($"📌 Bloc descendu sur {hit.collider.name} à Y={newY}");
        }
        else
        {
            Debug.Log("❗ Aucun support trouvé en dessous pour aligner le bloc.");
        }
    }

    bool ShouldSkipVerticalSnap(string name)
    {
        name = name.ToLower();
        return name.Contains("smallobstacle") || name.Contains("portal");
    }

    void SnapBlockVertically()
    {
        Collider2D col = currentBlock.GetComponent<Collider2D>();
        Bounds bounds = col.bounds;

        float snapThreshold = 0.1f; // ➜ 0.1 unité = environ 2 pixels

        Vector2 checkStart = new Vector2(bounds.min.x, bounds.min.y - snapThreshold);
        Vector2 checkEnd = new Vector2(bounds.max.x, bounds.min.y);

        Collider2D[] hitsBelow = Physics2D.OverlapAreaAll(checkStart, checkEnd);

        float highestY = -Mathf.Infinity;
        GameObject bestTarget = null;

        foreach (var hit in hitsBelow)
        {
            if (hit == null || hit.gameObject == currentBlock || hit.transform.IsChildOf(currentBlock.transform))
                continue;

            float top = hit.bounds.max.y;
            if (top > highestY)
            {
                highestY = top;
                bestTarget = hit.gameObject;
            }
        }

        if (bestTarget != null)
        {
            float blockHeight = bounds.size.y;
            float snapY = highestY + blockHeight / 2f;

            currentBlock.transform.position = new Vector3(currentBlock.transform.position.x, snapY, -1f);
            Debug.Log($"✅ Snap vertical à {snapY} sur {bestTarget.name}");
        }
        else
        {
            Debug.Log("❌ Aucun bloc trouvé assez proche en dessous pour snap.");
        }
    }


    void InstantiateAndPrepare(GameObject prefab, Vector3? scaleOverride = null)
    {
        GameObject obj = Instantiate(prefab);
        obj.transform.position = new Vector3(0, 0, -1);
        obj.transform.localScale = scaleOverride ?? currentScale;

        try { obj.tag = prefab.name; }
        catch { Debug.LogWarning($"Le tag '{prefab.name}' est manquant."); }

        currentBlock = obj;
        isPlacingBlock = true;
    }

    void HandleBlockRotation()
    {
        currentBlock.transform.Rotate(0f, 0f, -90f);
        Debug.Log("🔄 Bloc pivoté de 90° !");
    }

    #endregion

    #region Resizing & Deletion

    void HandleBlockResizing()
    {
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift) && !isPlacingBlock)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null && !hit.CompareTag("Ground"))
            {
                BeginResizing(hit.gameObject, mousePos);
            }
        }

        if (isResizing && resizingTarget != null)
        {
            PerformResizing();
        }
    }

    void BeginResizing(GameObject target, Vector2 mousePos)
    {
        resizingTarget = target;
        originalMousePos = mousePos;
        originalScale = target.transform.localScale;

        Vector2 localClick = mousePos - (Vector2)target.transform.position;
        float ratio = target.GetComponent<Collider2D>().bounds.size.x / target.GetComponent<Collider2D>().bounds.size.y;

        currentResizeAxis = Mathf.Abs(localClick.x) > Mathf.Abs(localClick.y * ratio)
            ? ResizeAxis.Horizontal
            : ResizeAxis.Vertical;

        isResizing = true;
        Debug.Log($"🧰 Début du redimensionnement : {target.name} (axe : {currentResizeAxis})");
    }

    void PerformResizing()
    {
        Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 delta = currentMousePos - originalMousePos;

        Vector3 newScale = originalScale;
        if (currentResizeAxis == ResizeAxis.Horizontal)
            newScale.x = Mathf.Max(0.1f, originalScale.x + delta.x);
        else if (currentResizeAxis == ResizeAxis.Vertical)
            newScale.y = Mathf.Max(0.1f, originalScale.y + delta.y);

        resizingTarget.transform.localScale = newScale;

        if (IsOverlapping(resizingTarget))
        {
            resizingTarget.transform.localScale = originalScale;
            Debug.Log("❌ Redimensionnement annulé : collision.");
        }

        if (Input.GetMouseButtonUp(0))
        {
            isResizing = false;
            resizingTarget = null;
            currentResizeAxis = ResizeAxis.None;
            Debug.Log("✅ Fin du redimensionnement");
        }
    }

    bool IsOverlapping(GameObject obj)
    {
        Bounds bounds = obj.GetComponent<Collider2D>().bounds;
        Collider2D[] overlaps = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0f);

        foreach (var col in overlaps)
        {
            if (col.gameObject != obj)
                return true;
        }
        return false;
    }

    void HandleBlockDeletion()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null && !hit.CompareTag("Ground"))
            {
                GameObject toDestroy = hit.gameObject;

                // ✅ Cas spécial : ObstacleBlock ou ses enfants
                if (toDestroy.name.Contains("ObstacleSafer") || toDestroy.name.Contains("ObstacleKiller"))
                {
                    Transform parent = toDestroy.transform.parent;
                    if (parent != null && parent.name.Contains("ObstacleBlock"))
                    {
                        toDestroy = parent.gameObject;
                    }
                }

                if (toDestroy == currentBlock)
                {
                    currentBlock = null;
                    isPlacingBlock = false;
                }

                Destroy(toDestroy);
                Debug.Log($"🗑️ Supprimé : {toDestroy.name}");
            }
        }
    }
    #endregion

    #region Utility

    bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    void TrySnapToNearbyBlock()
    {
        if (currentBlock == null) return;

        Collider2D blockCollider = currentBlock.GetComponent<Collider2D>();
        Bounds bounds = blockCollider.bounds;
        float snapDistance = 1f;

        Vector2[] directions =
        {
            Vector2.right, Vector2.left, Vector2.down, Vector2.up
        };

        foreach (var dir in directions)
        {
            Vector2 extent2D = new Vector2(bounds.extents.x, bounds.extents.y);
            Vector2 start = (Vector2)bounds.center + dir * (extent2D + Vector2.one * (snapDistance / 2f));
            Collider2D[] hits = Physics2D.OverlapCircleAll(start, snapDistance);

            foreach (var hit in hits)
            {
                if (hit != null && hit.gameObject != currentBlock)
                {
                    SnapToTarget(hit, dir);
                    return;
                }
            }
        }
    }

    void SnapToTarget(Collider2D hit, Vector2 dir)
    {
        Bounds hitBounds = hit.bounds;
        Bounds ourBounds = currentBlock.GetComponent<Collider2D>().bounds;

        Vector3 newPos = currentBlock.transform.position;

        if (dir == Vector2.right)
            newPos.x = hitBounds.min.x - ourBounds.size.x / 2f;
        else if (dir == Vector2.left)
            newPos.x = hitBounds.max.x + ourBounds.size.x / 2f;
        else if (dir == Vector2.down)
            newPos.y = hitBounds.max.y + ourBounds.size.y / 2f;
        else if (dir == Vector2.up)
            newPos.y = hitBounds.min.y - ourBounds.size.y / 2f;

        currentBlock.transform.position = new Vector3(newPos.x, newPos.y, -1);
        Debug.Log("✅ Snap à " + dir);
    }

    public void Save()
    {
        // TODO : Sauvegarde du niveau
    }

    #endregion
}
