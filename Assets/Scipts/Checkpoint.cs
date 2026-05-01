using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    public static Vector2 savePosition;
    public static string saveScene; // ✔ เพิ่ม

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            savePosition = transform.position;
            saveScene = SceneManager.GetActiveScene().name; // ✔ จำด่าน

            Debug.Log("Checkpoint Saved: " + savePosition);
        }
    }
}