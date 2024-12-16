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

        var mainModule = particleSystem.main;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;

        particleSystem.transform.parent = null;
    }

    public void Update()
    {
        transform.position += Time.deltaTime * 8.6f * Vector3.right;

        if (!IsJumping())
        {
            Vector3 Rotation = transform.rotation.eulerAngles;
            Rotation.z = Mathf.Round(Rotation.z / 90) * 90;
            transform.rotation = Quaternion.Euler(Rotation);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                rigidBody.AddForce(Vector2.up * 26.6581f, ForceMode2D.Impulse);
            }
            particleSystem.gameObject.SetActive(true);
        }
        else
        {
            particleSystem.gameObject.SetActive(false);
            transform.Rotate(Vector3.back * 360 * Time.deltaTime);
        }

        particleSystem.transform.position = transform.position + new Vector3(-0.19f, -0.64f, 0);
        particleSystem.transform.rotation = Quaternion.Euler(0, 0, 150.464f);

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
