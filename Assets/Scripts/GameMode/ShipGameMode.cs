using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipGameMode : IGameMode
{
    private const float HorizontalSpeed = 8.6f;
    private const float JumpForce = 26.6581f;
    private const KeyCode JumpKey = KeyCode.Space;
    private const float UpperAngle = 45f;
    private const float LowerAngle = -45f;
    private const float RotationTransitionDuration = 0.5f;

    public void Update(Player player)
    {
        player.RigidBody.linearVelocity = new Vector2(HorizontalSpeed * player.SpeedMultiplier, player.RigidBody.linearVelocity.y);

        bool jumpPressed = Input.GetKey(JumpKey);

        if (player.HasStarted && jumpPressed)
        {
            Jump(player);

            if (Input.GetKeyDown(JumpKey))
            {
                player.Transform.rotation = Quaternion.Euler(0, 0, UpperAngle);
            }
            else
            {
                player.Transform.rotation = Quaternion.Euler(0, 0, UpperAngle);
            }
        }
        else
        {
            float currentAngle = GetCurrentZAngle(player);

            float t = Mathf.Clamp01(Time.deltaTime / RotationTransitionDuration);
            float interpolationFactor = Mathf.Sin(t * (Mathf.PI / 2));
            float newAngle = Mathf.Lerp(currentAngle, LowerAngle, interpolationFactor);
            player.Transform.rotation = Quaternion.Euler(0, 0, newAngle);
        }

        if (player.Particle.gameObject.activeSelf)
        {
            player.Particle.gameObject.SetActive(false);
        }
    }

    private float GetCurrentZAngle(Player player)
    {
        float angle = player.Transform.rotation.eulerAngles.z;
        if (angle > 180f)
            angle -= 360f;
        return angle;
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

        float currentAngle = GetCurrentZAngle(player);
        float shortestAngle = Mathf.DeltaAngle(currentAngle, 0);
        player.Transform.rotation = Quaternion.RotateTowards(player.Transform.rotation, Quaternion.Euler(0, 0, 0), Mathf.Abs(shortestAngle));
    }

    public void OnCollisionExit(Player player, Collision2D collision)
    {
    }
}
