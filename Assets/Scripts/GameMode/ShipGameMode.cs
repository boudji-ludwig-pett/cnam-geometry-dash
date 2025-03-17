using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipGameMode : IGameMode
{
    private const float HorizontalSpeed = 8.6f;
    private const float JumpForce = 26.6581f;
    private const KeyCode JumpKey = KeyCode.Space;

    public void Update(Player player)
    {
        player.RigidBody.linearVelocity = new Vector2(HorizontalSpeed, player.RigidBody.linearVelocity.y);

        if (player.HasStarted && Input.GetKey(JumpKey))
        {
            Jump(player);
        }
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
        }
        if (collision.gameObject.CompareTag("Win"))
        {
            SceneManager.LoadScene("HomeScene");
        }
    }

    public void OnCollisionExit(Player player, Collision2D collision)
    {
        // rien pour l'instant
    }
}
