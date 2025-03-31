using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipGameMode : IGameMode
{
    private const float HorizontalSpeed = 8.6f;
    private const float JumpForce = 26.6581f;
    private const KeyCode JumpKey = KeyCode.Space;

    private const float UpperAngle = 45f;
    private const float LowerAngle = -45f;
    private const float RotationLerpSpeed = 5f;

    public void Update(Player player)
    {
        player.RigidBody.linearVelocity = new Vector2(HorizontalSpeed, player.RigidBody.linearVelocity.y);

        if (player.HasStarted && Input.GetKey(JumpKey))
        {
            Jump(player);
        }

        float targetAngle = Input.GetKey(JumpKey) ? UpperAngle : LowerAngle;
        float currentAngle = player.Transform.rotation.eulerAngles.z;
        if (currentAngle > 180f)
            currentAngle -= 360f;
        float newAngle = Mathf.Lerp(currentAngle, targetAngle, RotationLerpSpeed * Time.deltaTime);
        player.Transform.rotation = Quaternion.Euler(0, 0, newAngle);

        UpdateParticlePositionAndRotation(player);
    }

    private void UpdateParticlePositionAndRotation(Player player)
    {
        player.Particle.transform.position = player.Transform.position + new Vector3(-0.19f, -0.64f, -10);
        player.Particle.transform.rotation = Quaternion.Euler(0, 0, 150.464f);
    }

    private void Jump(Player player)
    {
        player.RigidBody.linearVelocity = new Vector2(player.RigidBody.linearVelocity.x, 0);
        player.RigidBody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        player.LevelsLoader.IncreaseTotalJumps();
    }

    public void OnCollisionEnter(Player player, Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Kill"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }
        if (collision.gameObject.CompareTag("Win"))
        {
            SceneManager.LoadScene("HomeScene");
            return;
        }

        float currentAngle = player.Transform.rotation.eulerAngles.z;
        float shortestAngle = Mathf.DeltaAngle(currentAngle, 0);
        player.Transform.rotation = Quaternion.RotateTowards(player.Transform.rotation, Quaternion.Euler(0, 0, 0), Mathf.Abs(shortestAngle));
    }

    public void OnCollisionExit(Player player, Collision2D collision)
    {
    }
}
