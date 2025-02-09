using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject playerObject;

    public void Update()
    {
        transform.position = new Vector3(playerObject.transform.position.x, transform.position.y, transform.position.z);
        // transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }
}
