using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public float flapStrength = 7;
    public bool isAlive = true;
    public Rigidbody2D rigidBody;
    public LogicManagerScript logicManager;

    public void Update()
    {
        if (!isAlive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidBody.linearVelocity = Vector2.up * flapStrength;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        logicManager.GameOver();
        isAlive = false;
    }
}
