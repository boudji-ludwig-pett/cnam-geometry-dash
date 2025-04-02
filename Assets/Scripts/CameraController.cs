using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;

    void Update()
    {
        // Déplacement horizontal uniquement
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // ← → ou A D

        if (horizontalInput != 0)
        {
            Vector3 movement = new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0f, 0f);
            Camera.main.transform.position += movement;
        }
    }
}
