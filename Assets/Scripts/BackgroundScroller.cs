using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.5f;
    [SerializeField] private float resetPosition = -18f;
    [SerializeField] private float startPosition = 18f;

    private void Update()
    {
        if (!GameManager.Instance.IsGameStarted || GameManager.Instance.IsGameOver)
            return;

        // Move background
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        // Check if background needs to be reset
        if (transform.position.x <= resetPosition)
        {
            Vector3 newPos = transform.position;
            newPos.x = startPosition;
            transform.position = newPos;
        }
    }
}
