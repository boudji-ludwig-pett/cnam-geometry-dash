using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject playerObject;
    public const float MIN_Y_FOLLOW = 2.0f;
    private float initialY;

    private void Start()
    {
        initialY = transform.position.y;
    }

    private void Update()
    {
        float targetY = initialY;

        if (playerObject.transform.position.y > MIN_Y_FOLLOW)
        {
            targetY = playerObject.transform.position.y;
        }

        transform.position = new Vector3(playerObject.transform.position.x, targetY, transform.position.z);
    }
}
