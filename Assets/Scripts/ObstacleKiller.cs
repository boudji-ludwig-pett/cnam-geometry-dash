using UnityEngine;

public class ObstacleKiller : MonoBehaviour
{
    public PlayerScript playerScript;
    public GameObject playerObject;

    public void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

    public void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        playerObject.transform.rotation = playerScript.initialRotation;
        playerObject.transform.position = playerScript.initialPosition;
        playerScript.isColliding = false;
    }
}
