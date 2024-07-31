using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class GrabGame : MonoBehaviour
{
    [Header("Settings")]
    public int maxNumberOfStones = 5;
    public bool isTimed = false;
    public float countdownTime = 30f;
    public GameObject location1, location2;
    public GameObject stonePrefabLocation, stonePrefab;

    [Header("UI Elements")]
    public TextMeshProUGUI stonesRemainingText;
    public TextMeshProUGUI timerText;

    [Header("Audio Clips")]
    public AudioClip winSound;
    public AudioClip loseSound;

    [Header("Events")]
    public UnityEvent onAllStonesPlaced;
    public UnityEvent onTimeUp;

    private int stonesPlacedCount = 0;
    private float timeRemaining;
    private AudioSource audioSource;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        timeRemaining = countdownTime;
        audioSource = gameObject.AddComponent<AudioSource>();

        UpdateStonesRemainingText();

        if (isTimed)
        {
            StartCoroutine(CountdownRoutine());
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("stone"))
        {
            stonesPlacedCount++;
            UpdateStonesRemainingText();
            ShuffleStonePrefabLocation();

            if (stonesPlacedCount >= maxNumberOfStones)
            {
                OnAllStonesPlaced();
            }
        }
    }

    private void OnAllStonesPlaced()
    {
        onAllStonesPlaced.Invoke();
        PlayAudioClip(winSound);
    }

    private IEnumerator CountdownRoutine()
    {
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1);
            timeRemaining--;
            UpdateTimerText();
        }

        OnTimeUp();
    }

    private void OnTimeUp()
    {
        onTimeUp.Invoke();
        PlayAudioClip(loseSound);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
    }

    public void ResetGame()
    {
        stonesPlacedCount = 0;
        UpdateStonesRemainingText();

        if (isTimed)
        {
            timeRemaining = countdownTime;
            UpdateTimerText();
            StartCoroutine(CountdownRoutine());
        }
    }

    private void UpdateStonesRemainingText()
    {
        stonesRemainingText.text = $"Place {maxNumberOfStones - stonesPlacedCount} more stones";
    }

    private void UpdateTimerText()
    {
        timerText.text = $"Time: {timeRemaining}";
    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void ShuffleStonePrefabLocation()
    {
        if (Random.Range(0, 2) == 0)
        {
            stonePrefabLocation.transform.position = location1.transform.position;
            Debug.Log("Location 1");
        }
        else
        {
            stonePrefabLocation.transform.position = location2.transform.position;
            Debug.Log("Location 2");
        }
    }
}
