using UnityEngine;

public class PipeScoreTrigger : MonoBehaviour
{
    private bool hasScored = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasScored) return;
        
        if (other.CompareTag("Player"))
        {
            hasScored = true;
            ScoreManager.Instance.AddScore();
        }
    }

    private void OnEnable()
    {
        hasScored = false;
    }
}
