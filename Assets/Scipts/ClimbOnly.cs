using UnityEngine;

public class ClimbOnly : MonoBehaviour
{
    public float climbSpeed = 4f;

    private Rigidbody2D rb;
    private bool isOnLadder;
    private float vertical;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        vertical = Input.GetAxis("Vertical");

        if (isOnLadder)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(0, vertical * climbSpeed);
        }
        else
        {
            rb.gravityScale = 3;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Ladder"))
            isOnLadder = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Ladder"))
            isOnLadder = false;
    }
}