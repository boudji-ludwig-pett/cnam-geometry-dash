using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject playerObject;
    public float normalMinYFollow = 2.0f;
    public float shipMinYFollow = 6.0f;
    private float initialY;

    [Header("References")]
    public bool isPlaying;

    private void Start()
    {
        initialY = transform.position.y;
    }

    private void Update()
    {
        if (isPlaying)
        {
            Player player = playerObject.GetComponent<Player>();

            float minYFollow = normalMinYFollow;
            if (player.CurrentGameMode is ShipGameMode)
            {
                minYFollow = shipMinYFollow;
            }

            float targetY = initialY;
            if (playerObject.transform.position.y > minYFollow)
            {
                targetY = playerObject.transform.position.y;
            }

            transform.position = new Vector3(playerObject.transform.position.x, targetY, transform.position.z);
        }
    }
}
