using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject playerObject;
    public float normalMinYFollow = 2.0f;
    public float shipMinYFollow = 6.0f;
    public float smoothSpeed = 5.0f;
    private float initialY;

    [Header("References")]
    public bool isPlaying;

    private void Start()
    {
        initialY = transform.position.y;
    }

    private void Update()
    {
        if (!isPlaying) return;

        Player player = playerObject.GetComponent<Player>();

        // Choix du minY selon le mode de jeu
        float minYFollow = (player.CurrentGameMode is ShipGameMode)
            ? shipMinYFollow
            : normalMinYFollow;

        // Calcul de la cible Y
        float targetY = initialY;
        if (playerObject.transform.position.y > minYFollow)
            targetY = playerObject.transform.position.y;

        // Interpolation douce
        float newY = Mathf.Lerp(transform.position.y, targetY, smoothSpeed * Time.deltaTime);

        // Clamp pour éviter de descendre sous Y = 0
        newY = Mathf.Max(newY, 0f);

        // On suit aussi l'axe X du joueur
        float newX = playerObject.transform.position.x;

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}
