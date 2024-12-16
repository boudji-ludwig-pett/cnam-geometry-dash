using UnityEngine;

public class ObstacleSafe : MonoBehaviour
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

    public void OnCollisionEnter()
    {
        playerScript.isColliding = true;
    }

    public void OnCollisionExit()
    {
        playerScript.isColliding = false;
    }
}
