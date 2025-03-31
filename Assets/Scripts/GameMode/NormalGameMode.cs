using UnityEngine;
using UnityEngine.SceneManagement;

public class NormalGameMode : IGameMode
{
    private const float HorizontalSpeed = 8.6f;
    private const float JumpForce = 26.6581f;
    private const KeyCode JumpKey = KeyCode.Space;

    public void Update(Player player)
    {
        player.RigidBody.linearVelocity = new Vector2(HorizontalSpeed * player.SpeedMultiplier, player.RigidBody.linearVelocity.y);

        if (player.HasStarted && player.IsColliding && Input.GetKey(JumpKey) && player.CanJump)
        {
            Jump(player);
        }

        if (!IsJumping(player))
        {
            AlignRotation(player);
            player.Particle.gameObject.SetActive(true);
        }
        else
        {
            player.Particle.gameObject.SetActive(false);
            player.Transform.Rotate(Vector3.back * 360 * Time.deltaTime);
        }

        UpdateParticlePositionAndRotation(player);
    }

    private void Jump(Player player)
    {
        player.RigidBody.linearVelocity = new Vector2(player.RigidBody.linearVelocity.x, 0);
        player.RigidBody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        player.LevelsLoader.IncreaseTotalJumps();
    }

    private bool IsJumping(Player player)
    {
        return !player.IsColliding;
    }

    private void AlignRotation(Player player)
    {
        Vector3 rotation = player.Transform.rotation.eulerAngles;
        rotation.z = Mathf.Round(rotation.z / 90) * 90;
        player.Transform.rotation = Quaternion.Euler(rotation);
    }

    private void UpdateParticlePositionAndRotation(Player player)
    {
        player.Particle.transform.position = player.Transform.position + new Vector3(-0.19f, -0.64f, -10);
        player.Particle.transform.rotation = Quaternion.Euler(0, 0, 150.464f);
    }

    public void OnCollisionEnter(Player player, Collision2D collision)
    {
        player.IsColliding = true;
        player.CanJump = true;

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
