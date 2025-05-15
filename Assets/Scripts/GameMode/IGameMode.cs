using UnityEngine;

public interface IGameMode
{
    void Update(Player player);
    void OnCollisionEnter(Player player, Collision2D collision);
    void OnCollisionExit(Player player, Collision2D collision);

    void Jump(Player player);
}
