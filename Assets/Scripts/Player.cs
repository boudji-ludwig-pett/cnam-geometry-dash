using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class Player : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    public GameObject playerObject;
    public ParticleSystem particle;
    public LevelsLoader levelsLoader;

    public bool isColliding = true;
    private bool hasStarted = false;

    private bool canJump = true;

    public void Start()
    {
        levelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();

        var mainModule = particle.main;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
        particle.transform.parent = null;

        Invoke(nameof(EnableInput), 0.1f);
    }

    private void EnableInput()
    {
        hasStarted = true;
    }

    public void Update()
    {
        rigidBody.linearVelocity = new Vector2(8.6f, rigidBody.linearVelocity.y);

        if (hasStarted && isColliding && Input.GetKey(KeyCode.Space) && canJump)
        {
            Jump();
        }

        if (!IsJumping())
        {
            AlignRotation();
            particle.gameObject.SetActive(true);
        }
        else
        {
            particle.gameObject.SetActive(false);
            transform.Rotate(Vector3.back * 360 * Time.deltaTime);
        }

        UpdateParticlePositionAndRotation();
    }

    private void Jump()
    {
        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0);
        rigidBody.AddForce(Vector2.up * 26.6581f, ForceMode2D.Impulse);
        levelsLoader.IncreaseTotalJumps();
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

    public void OnCollisionEnter2D(Collision2D collision)
    {
        isColliding = true;
        canJump = true;

        if (collision.gameObject.CompareTag("Kill"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (collision.gameObject.CompareTag("Win"))
        {
            SceneManager.LoadScene("HomeScene");
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false;
    }
}
