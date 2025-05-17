using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipGameMode : IGameMode
{
    private const float HorizontalSpeed = 8.6f;
    private const float JumpForce = 26.6581f;
    private const KeyCode JumpKey = KeyCode.Space;
    private const float MaxAscentAngle = 45f;
    private const float MaxDescentAngle = -45f;
    private const float RotationSpeed = 360f;

    public void Update(Player player)
    {
        player.RigidBody.linearVelocity = new Vector2(HorizontalSpeed * player.SpeedMultiplier, player.RigidBody.linearVelocity.y);

        bool jumpPressed = Input.GetKey(JumpKey);

        if (jumpPressed)
        {
            Jump(player);
        }

        float targetAngle;
        if (player.RigidBody.linearVelocity.y > 0.1f)
        {
            float velocityLerp = Mathf.Clamp01(player.RigidBody.linearVelocity.y / JumpForce);
            targetAngle = Mathf.Lerp(0f, MaxAscentAngle, velocityLerp);
        }
        else if (player.RigidBody.linearVelocity.y < -0.1f)
        {
            float velocityLerp = Mathf.Clamp01(Mathf.Abs(player.RigidBody.linearVelocity.y) / 20f);
            targetAngle = Mathf.Lerp(0f, MaxDescentAngle, velocityLerp);
        }
        else
        {
            targetAngle = 0f;
        }

        float currentAngle = GetCurrentZAngle(player);
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, RotationSpeed * Time.deltaTime);
        player.Transform.rotation = Quaternion.Euler(0, 0, newAngle);

        if (player.Particle.gameObject.activeSelf)
        {
            player.Particle.gameObject.SetActive(false);
        }
    }

    private float GetCurrentZAngle(Player player)
    {
        float angle = player.Transform.rotation.eulerAngles.z;
        if (angle > 180f)
        {
            angle -= 360f;
        }
        return angle;
    }

    public void Jump(Player player)
    {
        if (player.RigidBody.linearVelocity.y <= 0.1f)
        {
            player.RigidBody.linearVelocity = new Vector2(player.RigidBody.linearVelocity.x, 0);
            player.RigidBody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            if (player.LevelsLoader != null)
            {
                player.LevelsLoader.IncreaseTotalJumps();
            }
        }
    }

    public void OnCollisionEnter(Player player, Collision2D collision)
    {
        float snappedAngle = 0f;
        player.Transform.rotation = Quaternion.Euler(0, 0, snappedAngle);
    }

    public void OnCollisionExit(Player player, Collision2D collision)
    {
    }
}
