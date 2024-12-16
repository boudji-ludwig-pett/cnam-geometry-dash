using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    public GameObject groundObject;

    public Vector3 initialPosition;

    public ParticleSystem particleSystem;

    public void Start()
    {
        initialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    public void Update()
    {
        transform.position += Time.deltaTime * 8.6f * Vector3.right;

        if (!IsJumping())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rigidBody.AddForce(Vector2.up * 26.6581f, ForceMode2D.Impulse);
            }
            particleSystem.gameObject.SetActive(true);
        }
        else
        {
            particleSystem.gameObject.SetActive(false);
        }
        ParticleSystemSpeed();
    }

    private bool IsJumping()
    {
        return Mathf.Abs(initialPosition.y - transform.position.y) > 0.05f;
    }

    private void ParticleSystemSpeed()
    {
        var velocityOverLifetime = particleSystem.velocityOverLifetime;
        velocityOverLifetime.x = rigidBody.linearVelocity.x;
    }
}
