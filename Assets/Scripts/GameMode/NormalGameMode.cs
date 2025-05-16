using UnityEngine;
using UnityEngine.SceneManagement;


public class NormalGameMode : IGameMode
{
    public bool editMode { get; set; } = false;
    private const float HorizontalSpeed = 8.6f;
    private const float JumpForce = 26.6581f;
    private const KeyCode JumpKey = KeyCode.Space;
    private bool isRotating = false;
    private float targetRotationAngle = 0f;
    private readonly float rotationSpeed = 360f;

    public void Update(Player player)
    {
        player.RigidBody.linearVelocity = new Vector2(HorizontalSpeed * player.SpeedMultiplier, player.RigidBody.linearVelocity.y);


        if (player.IsColliding && Input.GetKey(JumpKey) && !isRotating)
        {
            Debug.Log("Player is Jumping");
            Jump(player);
        }

        if (isRotating)
        {
            PerformRotation(player);
        }

        if (!IsJumping(player))
        {
            player.Particle.gameObject.SetActive(true);
        }
        else
        {
            player.Particle.gameObject.SetActive(false);
        }

        UpdateParticlePositionAndRotation(player);
    }

    public void Jump(Player player)
    {
        player.RigidBody.linearVelocity = new Vector2(player.RigidBody.linearVelocity.x, 0);
        player.RigidBody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        if (player.LevelsLoader != null)
        {
            player.LevelsLoader.IncreaseTotalJumps();
        }
        isRotating = true;
        targetRotationAngle = player.transform.eulerAngles.z - 90f;
    }

    private void PerformRotation(Player player)
    {
        float rotationThisFrame = rotationSpeed * Time.deltaTime;
        float newRotation = Mathf.MoveTowardsAngle(player.transform.eulerAngles.z, targetRotationAngle, rotationThisFrame);
        player.transform.rotation = Quaternion.Euler(0, 0, newRotation);

        if (Mathf.Abs(Mathf.DeltaAngle(newRotation, targetRotationAngle)) < 0.1f)
        {
            player.transform.rotation = Quaternion.Euler(0, 0, targetRotationAngle);
            isRotating = false;
            AlignRotation(player);
        }
    }

    private bool IsJumping(Player player)
    {
        return !player.IsColliding;
    }

    private void AlignRotation(Player player)
    {
        Vector3 rotation = player.transform.eulerAngles;
        rotation.z = Mathf.Round(rotation.z / 90) * 90;
        player.transform.rotation = Quaternion.Euler(rotation);
    }

    private void UpdateParticlePositionAndRotation(Player player)
    {
        player.Particle.transform.position =
        player.transform.position + new Vector3(-0.19f, -0.64f, -10);
        player.Particle.transform.rotation = Quaternion.Euler(0, 0, 150.464f);
    }

    public void OnCollisionEnter(Player player, Collision2D collision)
    {
        player.IsColliding = true;

        Debug.Log("Nom de l'objet : " + collision.gameObject.name.ToString());
        Debug.Log("Nom du tag : " + collision.gameObject.tag.ToString());
        if (collision.gameObject.CompareTag("Kill"))
        {
            if (editMode)
            {
                player.transform.position = new Vector3(-16, -3, 0f);
                player.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                player.RigidBody.freezeRotation = true;
                player.RigidBody.linearVelocity = Vector2.zero;
                player.SpeedMultiplier = 1f;
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        if (collision.gameObject.CompareTag("Win"))
        {
            if (editMode)
            {
                player.transform.position = new Vector3(-16, -3, 0f);
                player.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                player.RigidBody.freezeRotation = true;
                player.RigidBody.linearVelocity = Vector2.zero;
                player.SpeedMultiplier = 1f;
            }
            else
            {
                SceneManager.LoadScene("SelectLevelScene");
            }
        }
    }

    public void OnCollisionExit(Player player, Collision2D collision)
    {
        player.IsColliding = false;
    }
}
