using UnityEngine;
using System.Collections;

public class PipeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pipePrefab;
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private float minHeight = -2f;
    [SerializeField] private float maxHeight = 2f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float pipeGap = 3.5f;

    private void Start()
    {
        if (pipePrefab == null)
        {
            Debug.LogError("Pipe prefab not assigned to spawner!");
            enabled = false;
            return;
        }

        StartCoroutine(SpawnPipes());
    }

    private IEnumerator SpawnPipes()
    {
        while (!GameManager.Instance.IsGameOver)
        {
            if (GameManager.Instance.IsGameStarted)
            {
                SpawnPipePair();
            }
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private void SpawnPipePair()
    {
        float randomHeight = Random.Range(minHeight, maxHeight);
        
        // Spawn top pipe
        GameObject topPipe = Instantiate(pipePrefab, 
            new Vector3(transform.position.x, randomHeight + pipeGap/2, 0), 
            Quaternion.Euler(0, 0, 180));
            
        // Spawn bottom pipe
        GameObject bottomPipe = Instantiate(pipePrefab, 
            new Vector3(transform.position.x, randomHeight - pipeGap/2, 0), 
            Quaternion.identity);

        // Add movement to pipes
        StartCoroutine(MovePipe(topPipe));
        StartCoroutine(MovePipe(bottomPipe));
    }

    private IEnumerator MovePipe(GameObject pipe)
    {
        while (pipe != null && pipe.transform.position.x > -12f)
        {
            pipe.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            yield return null;
        }

        if (pipe != null)
        {
            Destroy(pipe);
        }
    }
}
