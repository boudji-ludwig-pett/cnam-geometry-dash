using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    public GameObject groundObject;

    public Vector3 initialPosition;
    public Quaternion initialRotation;

    public ParticleSystem particle;

    private bool wantsToJump = false;

    public AudioSource audioSource;

    public void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        var mainModule = particle.main;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;

        particle.transform.parent = null;
    }

    public void Update()
    {
        transform.position += Time.deltaTime * 8.6f * Vector3.right;

        if (Input.GetKey(KeyCode.Space))
        {
            wantsToJump = true;
        }
        else
        {
            wantsToJump = false;
        }

        if (!IsJumping())
        {
            AlignRotation();

            if (wantsToJump)
            {
                Jump();
                wantsToJump = false;
            }

            particle.gameObject.SetActive(true);
        }
        else
        {
            particle.gameObject.SetActive(false);
            transform.Rotate(Vector3.back * 360 * Time.deltaTime);
        }

        UpdateParticlePositionAndRotation();
        UpdateParticleSystemSpeed();
    }

    private void Jump()
    {
        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0);
        rigidBody.AddForce(Vector2.up * 26.6581f, ForceMode2D.Impulse);
    }

    private bool IsJumping()
    {
        return Mathf.Abs(initialPosition.y - transform.position.y) > 0.05f;
    }

    private void AlignRotation()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.z = Mathf.Round(rotation.z / 90) * 90;
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void UpdateParticlePositionAndRotation()
    {
        particle.transform.position = transform.position + new Vector3(-0.19f, -0.64f, 0);
        particle.transform.rotation = Quaternion.Euler(0, 0, 150.464f);
    }

    private void UpdateParticleSystemSpeed()
    {
        var velocityOverLifetime = particle.velocityOverLifetime;
        velocityOverLifetime.x = rigidBody.linearVelocity.x;
    }
}
