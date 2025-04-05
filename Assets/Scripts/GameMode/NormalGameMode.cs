using UnityEngine;
using UnityEngine.SceneManagement;


public class NormalGameMode : IGameMode
{
    private const float HorizontalSpeed = 8.6f;
    private const float JumpForce = 26.6581f;
    private const KeyCode JumpKey = KeyCode.Space;
    private bool isRotating = false;
    private float targetRotationAngle = 0f;
    private readonly float rotationSpeed = 360f;


    public void Update(Player player)
    {
        player.RigidBody.linearVelocity = new Vector2(HorizontalSpeed * player.SpeedMultiplier, player.RigidBody.linearVelocity.y);

        if (player.HasStarted && player.IsColliding && Input.GetKey(JumpKey) && !isRotating)
        {
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

    private void Jump(Player player)
    {
        player.RigidBody.linearVelocity = new Vector2(player.RigidBody.linearVelocity.x, 0);
        player.RigidBody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        player.LevelsLoader.IncreaseTotalJumps();
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

        if (collision.gameObject.CompareTag("Kill"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (collision.gameObject.CompareTag("Win"))
        {
            SceneManager.LoadScene("SelectLevelScene");
        }
    }

    public void OnCollisionExit(Player player, Collision2D collision)
    {
        player.IsColliding = false;
    }
}
