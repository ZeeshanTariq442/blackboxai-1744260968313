using UnityEngine;

public class VisualEffects : MonoBehaviour
{
    public static VisualEffects Instance { get; private set; }

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem scoreEffect;
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private ParticleSystem trailEffect;

    [Header("Animation Settings")]
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float pulseDuration = 0.3f;
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeDuration = 0.2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayScoreEffect(Vector3 position)
    {
        if (scoreEffect != null)
        {
            ParticleSystem effect = Instantiate(scoreEffect, position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
        }
    }

    public void PlayDeathEffect(Vector3 position)
    {
        if (deathEffect != null)
        {
            ParticleSystem effect = Instantiate(deathEffect, position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
        }
    }

    public void StartTrailEffect(Transform target)
    {
        if (trailEffect != null)
        {
            ParticleSystem trail = Instantiate(trailEffect, target);
            trail.Play();
        }
    }

    public void StopTrailEffect(Transform target)
    {
        ParticleSystem[] trails = target.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem trail in trails)
        {
            trail.Stop();
            Destroy(trail.gameObject, trail.main.duration);
        }
    }

    public void PulseObject(Transform target)
    {
        if (target != null)
        {
            LeanTween.scale(target.gameObject, Vector3.one * pulseScale, pulseDuration)
                .setEasePunch()
                .setOnComplete(() => {
                    LeanTween.scale(target.gameObject, Vector3.one, pulseDuration);
                });
        }
    }

    public void ShakeCamera(Transform cameraTransform)
    {
        if (cameraTransform != null)
        {
            Vector3 originalPosition = cameraTransform.localPosition;
            LeanTween.value(gameObject, 0, 1, shakeDuration)
                .setOnUpdate((float value) => {
                    float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
                    float offsetY = Random.Range(-shakeIntensity, shakeIntensity);
                    cameraTransform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);
                })
                .setOnComplete(() => {
                    cameraTransform.localPosition = originalPosition;
                });
        }
    }

    public void FadeInObject(GameObject target, float duration = 1f)
    {
        if (target != null)
        {
            CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = target.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0f;
            LeanTween.alphaCanvas(canvasGroup, 1f, duration)
                .setEaseInOutSine();
        }
    }

    public void FadeOutObject(GameObject target, float duration = 1f)
    {
        if (target != null)
        {
            CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = target.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 1f;
            LeanTween.alphaCanvas(canvasGroup, 0f, duration)
                .setEaseInOutSine();
        }
    }

    public void PopupText(Transform target, string text, Color color)
    {
        GameObject popupTextObj = new GameObject("PopupText");
        popupTextObj.transform.position = target.position;
        
        TMPro.TextMeshPro tmpText = popupTextObj.AddComponent<TMPro.TextMeshPro>();
        tmpText.text = text;
        tmpText.color = color;
        tmpText.alignment = TMPro.TextAlignmentOptions.Center;
        tmpText.fontSize = 8;

        // Animate the popup text
        LeanTween.moveY(popupTextObj, popupTextObj.transform.position.y + 2f, 1f)
            .setEaseOutQuad();
        LeanTween.alphaText(tmpText.rectTransform, 0f, 1f)
            .setEaseInQuad()
            .setOnComplete(() => {
                Destroy(popupTextObj);
            });
    }
}
