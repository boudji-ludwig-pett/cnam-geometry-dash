using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class LevelEditor : MonoBehaviour
{
    [Header("Placement")]
    private GameObject currentBlock;
    private bool isPlacingBlock = false;
    private Vector3 currentScale = Vector3.one;
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
    private Transform persistentBlockContainer;

    void Start()
    {
        persistentBlockContainer = new GameObject("PlacedBlocks").transform;
        DontDestroyOnLoad(persistentBlockContainer.gameObject);
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

    void OnDestroy()
    {
        ClearEditor();
    }
    #region UI

    void LoadPrefabs()
    {
        var all = Resources.LoadAll<GameObject>("Prefabs");
        blockPrefabs.Clear();
        foreach (var prefab in all)
        {
            var name = prefab.name.ToLower();
            if (name == "ground" || name == "winnerwall") continue;
            blockPrefabs.Add(prefab);
        }
    }

    void GenerateButtons()
    {
        ClearCurrentButtons();
        if (blockGroupContainer == null || buttonPrefabTemplate == null)
        {
            Debug.LogError("UI Container ou prefab manquant.");
            return;
        }
        int start = currentPage * buttonsPerPage;
        int end = Mathf.Min(start + buttonsPerPage, blockPrefabs.Count);
        for (int i = start; i < end; i++)
        {
            var btn = Instantiate(buttonPrefabTemplate, blockGroupContainer);
            btn.SetActive(true);
            SetupButtonVisual(btn.transform, blockPrefabs[i], i - start);
            var prefab = blockPrefabs[i];
            btn.GetComponent<Button>().onClick.AddListener(() => SelectPrefab(prefab));
            currentButtons.Add(btn);
        }
    }

    void SetupButtonVisual(Transform t, GameObject prefab, int idx)
    {
        var canvas = t.Find("Canvas");
        var bg = canvas?.Find("BlankSquare");
        var icon = canvas?.Find("PrefabIcon");
        if (bg == null || icon == null) { Destroy(t.gameObject); return; }
        float xOff = -325f + idx * 125f;
        var bgRt = bg.GetComponent<RectTransform>();
        var icRt = icon.GetComponent<RectTransform>();
        bgRt.anchoredPosition = new Vector2(xOff, bgRt.anchoredPosition.y - 70);
        icRt.anchoredPosition = new Vector2(xOff, icRt.anchoredPosition.y - 70);
        bg.GetComponent<Image>().sprite = Resources.Load<Sprite>("InGame/ButtonSkin/BlankSquare");
        icon.GetComponent<Image>().sprite = prefab.GetComponent<SpriteRenderer>()?.sprite;
        icRt.sizeDelta = prefab.name.ToLower().Contains("small")
            ? new Vector2(50, 25)
            : new Vector2(50, 50);
    }

    void ClearCurrentButtons()
    {
        foreach (var b in currentButtons) Destroy(b);
        currentButtons.Clear();
    }

    public void NextPage()
    {
        int max = Mathf.CeilToInt(blockPrefabs.Count / (float)buttonsPerPage);
        if (currentPage < max - 1) { currentPage++; GenerateButtons(); }
    }

    public void PreviousPage()
    {
        if (currentPage > 0) { currentPage--; GenerateButtons(); }
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
        return Vector3.one;
    }

    void HandleBlockPlacement()
    {
        Vector2 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentBlock.transform.position = new Vector3(Mathf.Round(m.x), Mathf.Round(m.y), -1);
        if (Input.GetKeyDown(KeyCode.R)) HandleBlockRotation();
        if (!currentBlock.name.ToLower().Contains("portal"))
        {
            float s = Input.GetAxis("Mouse ScrollWheel");
            if (s != 0)
            {
                float ns = Mathf.Max(0.1f, currentScale.x + s * scaleStep);
                currentScale = Vector3.one * ns;
                currentBlock.transform.localScale = currentScale;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPlacementValid())
            {
                Debug.Log("Placement invalide : collision.");
                return;
            }
            PlaceBlock();
        }
    }

    bool IsPlacementValid()
    {
        var col = currentBlock.GetComponent<Collider2D>();
        var hits = Physics2D.OverlapBoxAll(col.bounds.center, col.bounds.size, 0f);
        foreach (var h in hits)
        {
            if (h == col) continue;
            if (h.transform.IsChildOf(currentBlock.transform)) continue;
            return false;
        }
        return true;
    }

    void HandleBlockSelection()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        Vector2 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.OverlapPoint(m);
        if (hit != null && !hit.CompareTag("Ground"))
        {
            var sel = hit.gameObject;
            if ((sel.name.Contains("ObstacleSafer") || sel.name.Contains("ObstacleKiller"))
                && sel.transform.parent != null
                && sel.transform.parent.name.Contains("ObstacleBlock"))
                sel = sel.transform.parent.gameObject;
            currentBlock = sel;
            isPlacingBlock = true;
            currentScale = currentBlock.transform.localScale;
            Debug.Log($"Sélection : {sel.name}");
        }
    }
    void PlaceBlock()
    {
        string name = currentBlock.name.ToLower();
        bool isSpikeType = name.Contains("spike") || name.Contains("smallspike") || name.Contains("killzone");

        if (isSpikeType)
        {
            // 1) Bloquer si on perçoit un spike de même type dans la direction de snap
            if (IsBlockedBySameTypeInSnapDirection())
            {
                Debug.LogError("Impossible de poser un spike sur un autre spike !");
                Destroy(currentBlock);
            }
            else
            {
                // 2) On snap dans la direction (down/left/up/right), et on détruit si aucun support
                if (!SnapSpikeByRotation())
                {
                    Debug.LogError("Impossible de poser un spike dans le vide !");
                    Destroy(currentBlock);
                }
                else
                {
                    // 3) On fait l’ajustement fin (si besoin)
                    TrySnapToNearbyBlock();
                }
            }
        }
        else
        {
            // tous les autres blocs
            TrySnapToNearbyBlock();
        }

        isPlacingBlock = false;
        currentBlock = null;
    }

    /// <summary>
    /// Vérifie qu’il n’y ait pas déjà un spike/smallspike/killzone
    /// juste devant le spike selon sa rotation.
    /// </summary>
    bool IsBlockedBySameTypeInSnapDirection()
    {
        var col = currentBlock.GetComponent<Collider2D>();
        var b = col.bounds;

        // 1) Détermine direction de snap (0→down,1→left,2→up,3→right)
        int rot = (Mathf.RoundToInt(currentBlock.transform.eulerAngles.z / 90) % 4 + 4) % 4;
        Vector2 dir = rot switch
        {
            1 => Vector2.right,
            2 => Vector2.up,
            3 => Vector2.left,
            _ => Vector2.down
        };

        // 2) Origine : on place la « boîte » juste en bordure du sprite
        float offset = 0.01f;
        Vector2 origin = rot switch
        {
            1 => new Vector2(b.min.x - offset, b.center.y),    // gauche
            3 => new Vector2(b.max.x + offset, b.center.y),    // droite
            2 => new Vector2(b.center.x, b.max.y + offset), // haut
            _ => new Vector2(b.center.x, b.min.y - offset)  // bas
        };

        // 3) On box‐cast exactement la taille du sprite pour 100 unités
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            origin,
            b.size,
            0f,
            dir,
            100f
        );

        foreach (var h in hits)
        {
            if (h.collider == null || h.collider.gameObject == currentBlock) continue;
            if (h.collider.isTrigger) continue;

            string me = currentBlock.name.ToLower();
            string other = h.collider.gameObject.name.ToLower();

            bool meIsSpikeFamily = me.Contains("spike") || me.Contains("killzone");
            bool otherIsSpikeFamily = other.Contains("spike") || other.Contains("killzone");

            if (meIsSpikeFamily && otherIsSpikeFamily)
            {
                // on bloque absolument tout chevauchement entre ces trois types
                return true;
            }

            // si on tape autre chose (sol, block, bonus…), on arrête le scan
            break;
        }

        return false;
    }

    bool SnapSpikeByRotation()
    {
        // Récupère bounds et demi-tailles
        var col = currentBlock.GetComponent<Collider2D>();
        var b = col.bounds;
        float hw = b.extents.x;
        float hh = b.extents.y;

        // 1) Détermine la rotation en quarts de tour : 0→down, 1→left, 2→up, 3→right
        int rot = ((Mathf.RoundToInt(currentBlock.transform.eulerAngles.z / 90f) % 4) + 4) % 4;
        Vector2 dir;
        switch (rot)
        {
            case 1: dir = Vector2.right; break;
            case 2: dir = Vector2.up; break;
            case 3: dir = Vector2.left; break;
            default: dir = Vector2.down; break;
        }

        // 2) Calcule 3 origines le long de la face « avant » du spike
        const float eps = 0.01f;
        List<Vector2> origins = new List<Vector2>();
        if (dir == Vector2.down || dir == Vector2.up)
        {
            // face inférieure ou supérieure → balaye l’axe X
            float y0 = (dir == Vector2.down) ? b.min.y - eps : b.max.y + eps;
            origins.Add(new Vector2(b.min.x + 0.1f * b.size.x, y0));
            origins.Add(new Vector2(b.center.x, y0));
            origins.Add(new Vector2(b.max.x - 0.1f * b.size.x, y0));
        }
        else
        {
            // face gauche ou droite → balaye l’axe Y
            float x0 = (dir == Vector2.left) ? b.min.x - eps : b.max.x + eps;
            origins.Add(new Vector2(x0, b.min.y + 0.1f * b.size.y));
            origins.Add(new Vector2(x0, b.center.y));
            origins.Add(new Vector2(x0, b.max.y - 0.1f * b.size.y));
        }

        // 3) Pour chaque origine, on lance un RaycastAll et on garde le hit le plus proche
        float bestDist = float.PositiveInfinity;
        RaycastHit2D bestHit = default;
        foreach (var o in origins)
        {
            var hits = Physics2D.RaycastAll(o, dir, 100f);
            foreach (var h in hits)
            {
                if (h.collider == null || h.collider.gameObject == currentBlock) continue;
                if (h.collider.isTrigger) continue;
                if (h.distance < bestDist)
                {
                    bestDist = h.distance;
                    bestHit = h;
                }
            }
        }

        // 4) Aucun support trouvé → échec
        if (bestHit.collider == null)
            return false;

        // 5) Sinon, colle bord à bord
        Vector3 p = currentBlock.transform.position;
        if (dir == Vector2.down) p.y = bestHit.point.y + hh;
        else if (dir == Vector2.up) p.y = bestHit.point.y - hh;
        else if (dir == Vector2.left) p.x = bestHit.point.x + hw;
        else if (dir == Vector2.right) p.x = bestHit.point.x - hw;

        currentBlock.transform.position = new Vector3(p.x, p.y, -1f);
        Debug.Log($"Spike snapé {dir} sur « {bestHit.collider.name} » à {currentBlock.transform.position}");
        return true;
    }


    #endregion

    #region Resizing & Deletion

    void HandleBlockResizing()
    {
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift) && !isPlacingBlock)
        {
            Vector2 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.OverlapPoint(m);
            if (hit != null && !hit.CompareTag("Ground"))
                BeginResizing(hit.gameObject, m);
        }
        if (isResizing && resizingTarget != null)
            PerformResizing();
    }

    void BeginResizing(GameObject tgt, Vector2 mPos)
    {
        resizingTarget = tgt;
        originalMousePos = mPos;
        originalScale = tgt.transform.localScale;
        Vector2 local = mPos - (Vector2)tgt.transform.position;
        float ratio = tgt.GetComponent<Collider2D>().bounds.size.x / tgt.GetComponent<Collider2D>().bounds.size.y;
        currentResizeAxis = Mathf.Abs(local.x) > Mathf.Abs(local.y * ratio)
            ? ResizeAxis.Horizontal
            : ResizeAxis.Vertical;
        isResizing = true;
        Debug.Log($"Début redim {tgt.name} (axe {currentResizeAxis})");
    }

    void PerformResizing()
    {
        Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 delta = m - originalMousePos;
        Vector3 ns = originalScale;
        if (currentResizeAxis == ResizeAxis.Horizontal)
            ns.x = Mathf.Max(0.1f, originalScale.x + delta.x);
        else
            ns.y = Mathf.Max(0.1f, originalScale.y + delta.y);
        resizingTarget.transform.localScale = ns;
        if (IsOverlapping(resizingTarget))
        {
            resizingTarget.transform.localScale = originalScale;
            Debug.Log("Redim annulé : collision");
        }
        if (Input.GetMouseButtonUp(0))
        {
            isResizing = false;
            resizingTarget = null;
            currentResizeAxis = ResizeAxis.None;
            Debug.Log("Fin redim");
        }
    }

    bool IsOverlapping(GameObject obj)
    {
        var b = obj.GetComponent<Collider2D>().bounds;
        foreach (var h in Physics2D.OverlapBoxAll(b.center, b.size, 0f))
            if (h.gameObject != obj) return true;
        return false;
    }

    void HandleBlockDeletion()
    {
        if (!Input.GetMouseButtonDown(1)) return;
        Vector2 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.OverlapPoint(m);
        if (hit != null && !hit.CompareTag("Ground"))
        {
            var toD = hit.gameObject;
            if ((toD.name.Contains("ObstacleSafer") || toD.name.Contains("ObstacleKiller"))
                && toD.transform.parent != null
                && toD.transform.parent.name.Contains("ObstacleBlock"))
                toD = toD.transform.parent.gameObject;
            if (toD == currentBlock) { currentBlock = null; isPlacingBlock = false; }
            Destroy(toD);
            Debug.Log($"Supprimé {toD.name}");
        }
    }

    #endregion

    #region Utility

    bool IsPointerOverUI()
        => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

    void TrySnapToNearbyBlock()
    {
        if (currentBlock == null) return;

        var col = currentBlock.GetComponent<Collider2D>();
        var b = col.bounds;
        float snapDistance = 1f;
        float verticalEps = 0.1f;

        // === SNAP HORIZONTAL (droite)
        Vector2 hBoxSize = new Vector2(snapDistance, b.size.y - verticalEps * 2f);
        Vector2 rightCenter = new Vector2(b.max.x + snapDistance / 2f, b.center.y);
        var hits = Physics2D.OverlapBoxAll(rightCenter, hBoxSize, 0f);
        foreach (var h in hits)
        {
            if (IsInvalidSnapTarget(h)) continue;
            float newX = h.bounds.min.x - b.extents.x;
            currentBlock.transform.position = new Vector3(newX, currentBlock.transform.position.y, -1f);
            Debug.Log($"Snap horizontal à droite contre {h.name}");
            return;
        }

        // === SNAP HORIZONTAL (gauche)
        Vector2 leftCenter = new Vector2(b.min.x - snapDistance / 2f, b.center.y);
        hits = Physics2D.OverlapBoxAll(leftCenter, hBoxSize, 0f);
        foreach (var h in hits)
        {
            if (IsInvalidSnapTarget(h)) continue;
            float newX = h.bounds.max.x + b.extents.x;
            currentBlock.transform.position = new Vector3(newX, currentBlock.transform.position.y, -1f);
            Debug.Log($"Snap horizontal à gauche contre {h.name}");
            return;
        }

        // === SNAP VERTICAL (dessous)
        Vector2 downBoxSize = new Vector2(b.size.x - 0.1f, snapDistance);
        Vector2 downCenter = new Vector2(b.center.x, b.min.y - snapDistance / 2f);
        hits = Physics2D.OverlapBoxAll(downCenter, downBoxSize, 0f);
        foreach (var h in hits)
        {
            if (IsInvalidSnapTarget(h)) continue;
            float newY = h.bounds.max.y + b.extents.y;
            currentBlock.transform.position = new Vector3(currentBlock.transform.position.x, newY, -1f);
            Debug.Log($"Snap vertical (bas) contre {h.name}");
            return;
        }

        // === SNAP VERTICAL (au-dessus)
        Vector2 upCenter = new Vector2(b.center.x, b.max.y + snapDistance / 2f);
        hits = Physics2D.OverlapBoxAll(upCenter, downBoxSize, 0f);
        foreach (var h in hits)
        {
            if (IsInvalidSnapTarget(h)) continue;
            float newY = h.bounds.min.y - b.extents.y;
            currentBlock.transform.position = new Vector3(currentBlock.transform.position.x, newY, -1f);
            Debug.Log($"Snap vertical (haut) contre {h.name}");
            return;
        }
    }

    bool IsInvalidSnapTarget(Collider2D h)
    {
        if (h == null || h.gameObject == currentBlock || h.isTrigger) return true;

        var t = h.transform;
        if (t.parent != null && t.parent.name.Contains("ObstacleBlock"))
        {
            if (t.name.Contains("ObstacleKiller") || t.name.Contains("ObstacleSafer"))
                return true;
        }

        return false;
    }

    // Filtrage des enfants parasites
    bool IsChildOfObstacleBlock(Collider2D col)
    {
        var t = col.transform;
        if (t.parent == null) return false;

        bool isNamedObstacleChild = t.name.Contains("ObstacleSafer") || t.name.Contains("ObstacleKiller");
        bool parentIsBlock = t.parent.name.Contains("ObstacleBlock");
        return isNamedObstacleChild && parentIsBlock;
    }

    void HandleBlockRotation()
    {
        currentBlock.transform.Rotate(0, 0, -90f);
        Debug.Log("Rotation 90°!");
    }

    void InstantiateAndPrepare(GameObject prefab, Vector3? scaleOverride = null)
    {
        var obj = Instantiate(prefab, persistentBlockContainer);
        obj.transform.position = new Vector3(0, 0, -1);
        obj.transform.localScale = scaleOverride ?? currentScale;
        currentBlock = obj;
        currentBlock.tag = prefab.tag;
        isPlacingBlock = true;
    }

    public void Save()
    {
    }

    #endregion

    public void ClearEditor()
    {
        if (persistentBlockContainer == null) return;

        foreach (Transform child in persistentBlockContainer)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("Éditeur vidé.");

        currentBlock = null;
        isPlacingBlock = false;
        currentPage = 0;
        ClearCurrentButtons();
        GenerateButtons();
    }
}
