using UnityEngine;
using UnityEngine.UI;
public class LevelEditor : MonoBehaviour
{
    public Transform mapParent; // Parent pour organiser les objets
    private GameObject currentBlock; // Block en cours de placement
    public Image blockButtonImage; // Image du bouton pour récupérer le sprite

    private bool isPlacingBlock = false; // Indique si un block est en mode placement
    private float scaleStep = 0.1f; // Incrément de redimensionnement avec la molette
    private Vector3 currentScale = new Vector3(1f, 1f, 1); // Échelle actuelle appliquée au block

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Déplacement du block sous la souris
        if (isPlacingBlock && currentBlock != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentBlock.transform.position = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), -1); // Aligne sur une grille

            // Redimensionnement avec la molette sauf pour les portails
            if (currentBlock.name != "ShipPortal" && currentBlock.name != "CubePortal")
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll != 0)
                {
                    float newScale = Mathf.Max(0.1f, currentScale.x + scroll * scaleStep);
                    currentScale = new Vector3(newScale, newScale, 1);
                    currentBlock.transform.localScale = currentScale;
                }
            }

            // Placer définitivement le block quand on clique
            if (Input.GetMouseButtonDown(0))
            {
                // Vérifie les collisions avec d'autres objets
                Collider2D[] overlaps = Physics2D.OverlapBoxAll(currentBlock.transform.position, currentBlock.GetComponent<Collider2D>().bounds.size, 0f);

                if (overlaps.Length > 1) // >1 car le bloc en cours a déjà un collider
                {
                    Debug.Log("Placement annulé : un objet est déjà présent à cet endroit.");
                    return;
                }

                PlaceBlock();
            }
        }
    }
    public void AddBlock()
    {
        if (isPlacingBlock) return;

        GameObject newBlock = new GameObject("Block");
        SpriteRenderer spriteRenderer = newBlock.AddComponent<SpriteRenderer>();

        if (blockButtonImage != null && blockButtonImage.sprite != null)
        {
            spriteRenderer.sprite = blockButtonImage.sprite;
        }
        else
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("InGame/BlockSkin/RegularBlock01");
            Debug.LogError("L'image du bouton de block n'est pas assignée !");
        }

        BoxCollider2D collider = newBlock.AddComponent<BoxCollider2D>();
        collider.offset = Vector2.zero;
        collider.size = spriteRenderer.sprite.bounds.size;

        newBlock.transform.position = new Vector2(0, 0);
        currentScale = new Vector3(1f, 1f, 1);
        newBlock.transform.localScale = currentScale;

        if (mapParent != null)
        {
            newBlock.transform.SetParent(mapParent);
        }

        currentBlock = newBlock;
        isPlacingBlock = true;
    }

    private void PlaceBlock()
    {
        isPlacingBlock = false;
        currentBlock = null;
    }

    public void AddSpike()
    {
        if (isPlacingBlock) return;

        GameObject newSpike = new GameObject("Spike");
        SpriteRenderer spriteRenderer = newSpike.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("InGame/SpikeSkin/BlueSpike");
        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("Le sprite de la plateforme est introuvable. Vérifiez le chemin Resources/InGame/SpikeSkin/BlueSpike");
        }

        BoxCollider2D collider = newSpike.AddComponent<BoxCollider2D>();
        collider.offset = Vector2.zero;
        collider.size = spriteRenderer.sprite.bounds.size;

        newSpike.transform.position = new Vector2(0, 0);
        currentScale = new Vector3(1f, 1f, 1);
        newSpike.transform.localScale = currentScale;

        if (mapParent != null)
        {
            newSpike.transform.SetParent(mapParent);
        }

        currentBlock = newSpike;
        isPlacingBlock = true;
    }

    public void AddSmallSpike()
    {
        if (isPlacingBlock) return;

        GameObject newSmallSpike = new GameObject("SmallSpike");
        SpriteRenderer spriteRenderer = newSmallSpike.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("InGame/SmallSpikeSkin/BaseSmallSpike");
        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("Le sprite de la plateforme est introuvable. Vérifiez le chemin Resources/InGame/SpikeSkin/BlueSpike");
        }

        BoxCollider2D collider = newSmallSpike.AddComponent<BoxCollider2D>();
        collider.offset = Vector2.zero;
        collider.size = spriteRenderer.sprite.bounds.size;

        newSmallSpike.transform.position = new Vector2(0, 0);
        currentScale = new Vector3(0.25f, 0.25f, 1);
        newSmallSpike.transform.localScale = currentScale;

        if (mapParent != null)
        {
            newSmallSpike.transform.SetParent(mapParent);
        }

        currentBlock = newSmallSpike;
        isPlacingBlock = true;
    }

    public void AddPlatform()
    {
        if (isPlacingBlock) return;

        GameObject newPlatform = new GameObject("Platform");
        SpriteRenderer spriteRenderer = newPlatform.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("InGame/PlateformSkin/RegularPlatform01");
        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("Le sprite de la plateforme est introuvable. Vérifiez le chemin Resources/InGame/Platform/Platform01.png");
        }

        BoxCollider2D collider = newPlatform.AddComponent<BoxCollider2D>();
        collider.offset = Vector2.zero;
        collider.size = spriteRenderer.sprite.bounds.size;

        newPlatform.transform.position = new Vector2(0, 0);
        currentScale = new Vector3(1f, 1f, 1);
        newPlatform.transform.localScale = currentScale;

        if (mapParent != null)
        {
            newPlatform.transform.SetParent(mapParent);
        }

        currentBlock = newPlatform;
        isPlacingBlock = true;
    }

    public void AddShipPortal()
    {
        if (isPlacingBlock) return;

        GameObject newShipPortal = new GameObject("ShipPortal");
        SpriteRenderer spriteRenderer = newShipPortal.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("InGame/PortalsSkin/ShipPortalLabelled");
        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("Le sprite de la plateforme est introuvable. Vérifiez le chemin Resources/InGame/PortalSkin/ShipPortalLabelled");
        }

        BoxCollider2D collider = newShipPortal.AddComponent<BoxCollider2D>();
        collider.offset = Vector2.zero;
        collider.size = spriteRenderer.sprite.bounds.size;

        newShipPortal.transform.position = new Vector2(0, 0);
        newShipPortal.transform.localScale = new Vector3(0.5f, 0.5f, 1);

        if (mapParent != null)
        {
            newShipPortal.transform.SetParent(mapParent);
        }

        currentBlock = newShipPortal;
        isPlacingBlock = true;
    }

    public void AddCubePortal()
    {
        if (isPlacingBlock) return;

        GameObject newCubePortal = new GameObject("CubePortal");
        SpriteRenderer spriteRenderer = newCubePortal.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("InGame/PortalsSkin/CubePortalLabelled");
        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("Le sprite de la plateforme est introuvable. Vérifiez le chemin Resources/InGame/PortalSkin/ShipPortalLabelled");
        }

        BoxCollider2D collider = newCubePortal.AddComponent<BoxCollider2D>();
        collider.offset = Vector2.zero;
        collider.size = spriteRenderer.sprite.bounds.size;

        newCubePortal.transform.position = new Vector2(0, 0);
        newCubePortal.transform.localScale = new Vector3(0.5f, 0.5f, 1);

        if (mapParent != null)
        {
            newCubePortal.transform.SetParent(mapParent);
        }

        currentBlock = newCubePortal;
        isPlacingBlock = true;
    }

    public void Save()
    {
        //TODO
    }
}
