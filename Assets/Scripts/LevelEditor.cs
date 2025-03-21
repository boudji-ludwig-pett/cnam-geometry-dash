using UnityEngine;
using UnityEngine.UI;
public class LevelEditor : MonoBehaviour
{
    public Transform mapParent; // Parent pour organiser les objets
    private GameObject currentBlock; // Block en cours de placement
    public Image blockButtonImage; // Image du bouton pour récupérer le sprite

    private bool isPlacingBlock = false; // Indique si un block est en mode placement

    void Start()
    {
        if (blockButtonImage == null)
        {
            blockButtonImage = GameObject.Find("BlankSquare").GetComponent<Image>(); // Assurez-vous que le nom est correct

            if (blockButtonImage == null)
            {
                Debug.LogError("Impossible de trouver 'blockButtonImage' automatiquement !");
                return;
            }
        }

        // Charger l'image du bouton depuis le bon chemin relatif
        Sprite loadedSprite = Resources.Load<Sprite>("InGame/BlockSkin/RegularBlock01");
        if (loadedSprite != null)
        {
            blockButtonImage.sprite = loadedSprite;
        }
        else
        {
            Debug.LogError("Impossible de charger l'image RegularBlock01.png. Vérifiez qu'elle est bien placée dans Assets/Resources/InGame/BlockSkin");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Déplacement du block sous la souris
        if (isPlacingBlock && currentBlock != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentBlock.transform.position = new Vector2(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y)); // Aligne sur une grille

            // Placer définitivement le block quand on clique
            if (Input.GetMouseButtonDown(0))
            {
                PlaceBlock();
            }
        }
    }

    public void AddBlock()
    {
        if (isPlacingBlock) return; // Empêche d'ajouter plusieurs blocks à la fois

        // Création d'un nouvel objet Block
        GameObject newBlock = new GameObject("Block");

        // Ajout d'un Sprite Renderer
        SpriteRenderer spriteRenderer = newBlock.AddComponent<SpriteRenderer>();
        if (blockButtonImage != null)
        {
            spriteRenderer.sprite = blockButtonImage.sprite;
        }
        else
        {
            Debug.LogError("L'image du bouton de block n'est pas assignée !");
        }

        // Ajout d'un BoxCollider2D
        BoxCollider2D collider = newBlock.AddComponent<BoxCollider2D>();
        collider.offset = new Vector2(-0.0040f, 1.77481f);
        collider.size = new Vector2(5.12119f, 1.45697f);

        // Position par défaut
        newBlock.transform.position = new Vector2(0, 0);
        newBlock.transform.localScale = new Vector3(0.96055f, 0.2326f, 1);

        // Organisation sous un parent
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
        currentBlock = null; // Réinitialise le block en cours de placement
    }
    public void AddSpike()
    {
        // TODO
    }

    public void AddSmallSpike()
    {
        //TODO
    }

    public void AddPlateform()
    {
        //TODO
    }
    public void AddShipPortal()
    {
        //TODO
    }
    public void AddCubePortal()
    {
        //TODO
    }
    public void Save()
    {
        //TODO
    }
}
