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

            if (currentBlock != null && Input.GetKeyDown(KeyCode.R))
            {
                HandleBlockRotation(); // ✅ Nouvelle rotation
            }

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

            if (hit != null && hit.transform != null)
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

            if (hit != null && hit.transform != null && !hit.CompareTag("Ground"))
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
                if (col.gameObject != resizingTarget)
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

            if (hit != null && hit.transform != null)
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
        bool skipVerticalSnap = false;

        if (currentBlock.name.ToLower().Contains("smallobstacle") || currentBlock.name.ToLower().Contains("portal"))
        {
            skipVerticalSnap = true; // On saute l'alignement vertical pour ces cas-là
        }

        if (!skipVerticalSnap)
        {
            Vector2 origin = currentBlock.transform.position;
            RaycastHit2D[] hitsBelow = Physics2D.RaycastAll(origin, Vector2.down, 100f);

            float highestY = -Mathf.Infinity;
            GameObject bestTargetBelow = null;

            foreach (var hit in hitsBelow)
            {
                if (hit.collider != null && hit.collider.gameObject != currentBlock)
                {
                    float topOfObject = hit.collider.bounds.max.y;
                    if (topOfObject > highestY)
                    {
                        highestY = topOfObject;
                        bestTargetBelow = hit.collider.gameObject;
                    }
                }
            }

            if (bestTargetBelow != null)
            {
                float height = currentBlock.GetComponent<Collider2D>().bounds.size.y;
                currentBlock.transform.position = new Vector3(currentBlock.transform.position.x, highestY + height / 2f, -1);
            }
            else
            {
                float height = currentBlock.GetComponent<Collider2D>().bounds.size.y;
                currentBlock.transform.position = new Vector3(currentBlock.transform.position.x, height / 2f, -1);
            }
        }
        // ➔ Toujours essayer de snap sur la droite et en bas même pour Portal et SmallObstacle
        TrySnapToNearbyBlock();

        isPlacingBlock = false;
        currentBlock = null;
    }


    private void TrySnapToNearbyBlock()
    {
        if (currentBlock == null)
            return;

        Collider2D blockCollider = currentBlock.GetComponent<Collider2D>();
        Bounds bounds = blockCollider.bounds;

        float snapDistance = 1f; // Distance de snap (en Unity units)

        // Zone de scan à droite
        Vector2 rightAreaStart = new Vector2(bounds.max.x, bounds.min.y);
        Vector2 rightAreaEnd = new Vector2(bounds.max.x + snapDistance, bounds.max.y);

        // Zone de scan à gauche
        Vector2 leftAreaStart = new Vector2(bounds.min.x - snapDistance, bounds.min.y);
        Vector2 leftAreaEnd = new Vector2(bounds.min.x, bounds.max.y);

        // Zone de scan en dessous
        Vector2 bottomAreaStart = new Vector2(bounds.min.x, bounds.min.y - snapDistance);
        Vector2 bottomAreaEnd = new Vector2(bounds.max.x, bounds.min.y);

        // Zone de scan au dessus
        Vector2 topAreaStart = new Vector2(bounds.min.x, bounds.max.y);
        Vector2 topAreaEnd = new Vector2(bounds.max.x, bounds.max.y + snapDistance);

        Collider2D[] hitsRight = Physics2D.OverlapAreaAll(rightAreaStart, rightAreaEnd);
        Collider2D[] hitsLeft = Physics2D.OverlapAreaAll(leftAreaStart, leftAreaEnd);
        Collider2D[] hitsBelow = Physics2D.OverlapAreaAll(bottomAreaStart, bottomAreaEnd);
        Collider2D[] hitsAbove = Physics2D.OverlapAreaAll(topAreaStart, topAreaEnd);

        // ➔ Priorité : droite > gauche > bas > haut

        foreach (var hit in hitsRight)
        {
            if (hit != null && hit.gameObject != currentBlock)
            {
                float theirLeft = hit.bounds.min.x;
                float ourWidth = bounds.size.x;
                currentBlock.transform.position = new Vector3(theirLeft - ourWidth / 2f, currentBlock.transform.position.y, -1);
                Debug.Log("✅ Snap automatique à droite !");
                return;
            }
        }

        foreach (var hit in hitsLeft)
        {
            if (hit != null && hit.gameObject != currentBlock)
            {
                float theirRight = hit.bounds.max.x;
                float ourWidth = bounds.size.x;
                currentBlock.transform.position = new Vector3(theirRight + ourWidth / 2f, currentBlock.transform.position.y, -1);
                Debug.Log("✅ Snap automatique à gauche !");
                return;
            }
        }

        foreach (var hit in hitsBelow)
        {
            if (hit != null && hit.gameObject != currentBlock)
            {
                float theirTop = hit.bounds.max.y;
                float ourHeight = bounds.size.y;
                currentBlock.transform.position = new Vector3(currentBlock.transform.position.x, theirTop + ourHeight / 2f, -1);
                Debug.Log("✅ Snap automatique en bas !");
                return;
            }
        }

        foreach (var hit in hitsAbove)
        {
            if (hit != null && hit.gameObject != currentBlock)
            {
                float theirBottom = hit.bounds.min.y;
                float ourHeight = bounds.size.y;
                currentBlock.transform.position = new Vector3(currentBlock.transform.position.x, theirBottom - ourHeight / 2f, -1);
                Debug.Log("✅ Snap automatique en haut !");
                return;
            }
        }
    }
    void InstantiateAndPrepare(GameObject prefab, Vector3? scaleOverride = null)
    {
        GameObject obj = Instantiate(prefab);
        obj.transform.position = new Vector3(0, 0, -1);
        obj.transform.localScale = scaleOverride ?? currentScale;

        try { obj.tag = prefab.name; }
        catch { Debug.LogWarning($"Le tag '{prefab.name}' n'existe pas. Ajoutez-le dans Project Settings > Tags."); }

        currentBlock = obj;
        isPlacingBlock = true;
    }

    private void HandleBlockRotation()
    {
        currentBlock.transform.Rotate(0f, 0f, -90f); // ➔ Rotation de 90° dans le sens horaire
        Debug.Log("🔄 Bloc pivoté de 90° !");
    }
}
