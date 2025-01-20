using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    public GameObject playerObject;

    public ParticleSystem particle;

    private bool wantsToJump = false;
    public bool isColliding = true;

    public AudioSource audioSource;

    public void Start()
    {
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
        return !isColliding;
    }

    private void AlignRotation()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.z = Mathf.Round(rotation.z / 90) * 90;
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void UpdateParticlePositionAndRotation()
    {
        particle.transform.position = transform.position + new Vector3(-0.19f, -0.64f, -10);
        particle.transform.rotation = Quaternion.Euler(0, 0, 150.464f);
    }

    private void UpdateParticleSystemSpeed()
    {
        var velocityOverLifetime = particle.velocityOverLifetime;
        velocityOverLifetime.x = rigidBody.linearVelocity.x;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        isColliding = true;

        if (collision.gameObject.tag == "Kill")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false;
    }
}
