using UnityEngine;

public class PipeMoveScript : MonoBehaviour
{
    public float moveSpeed = 2;
    public float deadZone = -45;
    public BirdScript bird;

    public void Start()
    {
        bird = GameObject.FindGameObjectWithTag("Player").GetComponent<BirdScript>();
    }

    public void Update()
    {
        if (!bird.isAlive)
        {
            return;
        }
        transform.position = transform.position + (Time.deltaTime * moveSpeed * Vector3.left);

        if (transform.position.x < deadZone)
        {
            Destroy(gameObject);
        }
    }
}
