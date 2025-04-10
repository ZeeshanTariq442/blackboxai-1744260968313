using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utilities
{
    #region Extension Methods
    
    public static void ResetTransform(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void SetActiveChildren(this Transform transform, bool active)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(active);
        }
    }

    public static void DestroyChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            Object.Destroy(child.gameObject);
        }
    }

    #endregion

    #region Math Utilities

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static Vector2 GetRandomPointInCircle(float radius)
    {
        float angle = Random.Range(0f, 360f);
        float r = radius * Mathf.Sqrt(Random.Range(0f, 1f));
        return new Vector2(r * Mathf.Cos(angle), r * Mathf.Sin(angle));
    }

    #endregion

    #region Animation Helpers

    public static IEnumerator LerpPosition(Transform target, Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            target.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
        target.position = end;
    }

    public static IEnumerator LerpRotation(Transform target, Quaternion start, Quaternion end, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            target.rotation = Quaternion.Lerp(start, end, t);
            yield return null;
        }
        target.rotation = end;
    }

    #endregion

    #region UI Helpers

    public static void SetAlpha(this CanvasGroup canvasGroup, float alpha)
    {
        canvasGroup.alpha = alpha;
        canvasGroup.blocksRaycasts = alpha > 0;
        canvasGroup.interactable = alpha > 0;
    }

    public static string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public static string FormatScore(int score)
    {
        return score.ToString("N0");
    }

    #endregion

    #region Game State Helpers

    public static bool IsGamePaused()
    {
        return Time.timeScale == 0;
    }

    public static void PauseGame()
    {
        Time.timeScale = 0;
    }

    public static void ResumeGame()
    {
        Time.timeScale = 1;
    }

    #endregion

    #region Object Pooling

    private static Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>();

    public static GameObject GetPooledObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string key = prefab.name;
        
        if (!objectPools.ContainsKey(key))
        {
            objectPools[key] = new Queue<GameObject>();
        }

        GameObject obj;
        if (objectPools[key].Count > 0)
        {
            obj = objectPools[key].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }
        else
        {
            obj = Object.Instantiate(prefab, position, rotation);
            obj.name = key;
        }

        return obj;
    }

    public static void ReturnToPool(GameObject obj)
    {
        string key = obj.name;
        obj.SetActive(false);
        
        if (!objectPools.ContainsKey(key))
        {
            objectPools[key] = new Queue<GameObject>();
        }
        
        objectPools[key].Enqueue(obj);
    }

    public static void ClearPool(string key)
    {
        if (objectPools.ContainsKey(key))
        {
            while (objectPools[key].Count > 0)
            {
                GameObject obj = objectPools[key].Dequeue();
                Object.Destroy(obj);
            }
            objectPools.Remove(key);
        }
    }

    public static void ClearAllPools()
    {
        foreach (var pool in objectPools.Values)
        {
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                Object.Destroy(obj);
            }
        }
        objectPools.Clear();
    }

    #endregion

    #region Debug Helpers

    public static void DrawCircle(Vector3 center, float radius, Color color, float duration = 0)
    {
        const int segments = 36;
        float angle = 0f;
        
        for (int i = 0; i < segments; i++)
        {
            float nextAngle = ((float)(i + 1) / segments) * 360f * Mathf.Deg2Rad;
            angle = ((float)i / segments) * 360f * Mathf.Deg2Rad;
            
            Vector3 currentPos = center + new Vector3(Mathf.Sin(angle) * radius, Mathf.Cos(angle) * radius, 0);
            Vector3 nextPos = center + new Vector3(Mathf.Sin(nextAngle) * radius, Mathf.Cos(nextAngle) * radius, 0);
            
            Debug.DrawLine(currentPos, nextPos, color, duration);
        }
    }

    public static string GetStackTrace()
    {
        System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
        return stackTrace.ToString();
    }

    #endregion
}

// Custom attributes
public class ReadOnlyAttribute : PropertyAttribute { }

// Custom event system
public class GameEvent
{
    private readonly List<System.Action> listeners = new List<System.Action>();

    public void AddListener(System.Action listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(System.Action listener)
    {
        listeners.Remove(listener);
    }

    public void Invoke()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i]?.Invoke();
        }
    }
}

// Singleton base class
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static readonly object Lock = new object();
    private static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }

            lock (Lock)
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError($"[Singleton] Multiple instances of {typeof(T)}!");
                        return instance;
                    }

                    if (instance == null)
                    {
                        GameObject singleton = new GameObject();
                        instance = singleton.AddComponent<T>();
                        singleton.name = $"(singleton) {typeof(T)}";
                        DontDestroyOnLoad(singleton);
                    }
                }

                return instance;
            }
        }
    }

    protected virtual void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}
