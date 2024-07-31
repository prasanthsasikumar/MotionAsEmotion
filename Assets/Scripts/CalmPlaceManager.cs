using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CalmPlaceManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip calmSound;

    [Header("Effect Settings")]
    public float duration = 20f; // Duration over which to change contrast
    public Color endColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Color to fade to

    [Header("UI Elements")]
    public GameObject dialogueBox;

    private ColorAdjustments colorAdjustments;
    private float originalContrast;
    private Color originalColorFilter;
    private float targetContrast = 100f; // Target contrast value for darkening effect
    private float elapsedTime = 0f;
    private AudioSource audioSource;

    private void Start()
    {
        if (TryGetComponent(out Volume volume) && volume.profile.TryGet(out colorAdjustments))
        {
            originalContrast = colorAdjustments.contrast.value;
            originalColorFilter = colorAdjustments.colorFilter.value;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = calmSound;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetEffect();
        }
    }

    private IEnumerator ChangeContrastOverTime()
    {
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > duration * 0.1f)
            {
                dialogueBox.SetActive(false);
            }

            float t = Mathf.Clamp01(elapsedTime / duration);
            colorAdjustments.contrast.value = Mathf.Lerp(originalContrast, targetContrast, t);
            colorAdjustments.colorFilter.value = Color.Lerp(originalColorFilter, endColor, t);
            yield return null;
        }
    }

    public void ResetEffect()
    {
        elapsedTime = 0f;
        colorAdjustments.contrast.value = originalContrast;
        colorAdjustments.colorFilter.value = originalColorFilter;
        dialogueBox.SetActive(true);

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.Play();
        StartCoroutine(ChangeContrastOverTime());
    }
}
