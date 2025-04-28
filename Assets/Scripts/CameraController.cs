using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // ← → ou A D
        float verticalInput = Input.GetAxisRaw("Vertical");     // ↑ ↓ ou W S

        Vector3 movement = new Vector3(
            horizontalInput * moveSpeed * Time.deltaTime,
            verticalInput * moveSpeed * Time.deltaTime,
            0f
        );

        Camera.main.transform.position += movement;
    }
}
