using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BirdController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    
    private Rigidbody2D rb;
    private bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on bird!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (isDead || !GameManager.Instance.IsGameStarted) return;

        // Handle input (mouse click, touch, or spacebar)
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Rotate bird based on velocity
        if (rb.velocity.y > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 30);
        }
        else
        {
            float rotZ = Mathf.Lerp(30, -90, (-rb.velocity.y / 10f));
            transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }
    }

    private void Jump()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDead)
        {
            isDead = true;
            GameManager.Instance.TriggerGameOver();
        }
    }
}
