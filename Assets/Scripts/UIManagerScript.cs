using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour
{
    public int numberOfTimes = 3;
    public int countdownTime = 10;
    public GameObject frontPageObject, lastPageObject;
    public bool timed = false;
    public Color pageObjectColor = Color.red;
    public AudioClip winSound, loseSound;
    public TMPro.TextMeshProUGUI countdownText;

    private int currentTry = 0;
    private Toggle frontToggle;
    private Toggle backToggle;
    private Coroutine countdownCoroutine;
    private Color originalPageObjectColor;

    void Start()
    {
        frontToggle = frontPageObject.GetComponent<Toggle>();
        backToggle = lastPageObject.GetComponent<Toggle>();

        frontToggle.onValueChanged.AddListener(OnFrontToggleChanged);
        backToggle.onValueChanged.AddListener(OnBackToggleChanged);

        originalPageObjectColor = frontToggle.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;

        // Initially enable the back toggle and disable the front toggle
        frontToggle.interactable = false;
        backToggle.interactable = true;
        backToggle.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = pageObjectColor;
        backToggle.isOn = false;

        if (timed)
        {
            countdownCoroutine = StartCoroutine(CountdownRoutine());
        }
        this.gameObject.AddComponent<AudioSource>();
    }

    void OnFrontToggleChanged(bool isOn)
    {
        if (isOn && currentTry < numberOfTimes)
        {
            frontToggle.interactable = false;
            frontToggle.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = originalPageObjectColor;

            // Remove listener, change state, and add listener back
            RemoveListener(frontToggle, OnFrontToggleChanged);
            frontToggle.isOn = false;
            AddListener(frontToggle, OnFrontToggleChanged);

            backToggle.interactable = true;
            backToggle.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = pageObjectColor;
            currentTry++;
        }
        CheckCompletion();
    }

    void OnBackToggleChanged(bool isOn)
    {
        if (isOn && currentTry < numberOfTimes)
        {
            backToggle.interactable = false;
            backToggle.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = originalPageObjectColor;

            // Remove listener, change state, and add listener back
            RemoveListener(backToggle, OnBackToggleChanged);
            backToggle.isOn = false;
            AddListener(backToggle, OnBackToggleChanged);

            frontToggle.interactable = true;
            frontToggle.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = pageObjectColor;
        }
    }

    private void CheckCompletion()
    {
        if (currentTry >= numberOfTimes)
        {
            frontToggle.interactable = false;
            backToggle.interactable = false;

            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
            }
            // Optionally, you can trigger an event or method call here to indicate completion.
            Debug.Log("Sequence completed!");
            frontToggle.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = originalPageObjectColor;
            frontToggle.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = originalPageObjectColor;
            this.gameObject.GetComponent<AudioSource>().PlayOneShot(winSound);
        }
    }

    private IEnumerator CountdownRoutine()
    {
        float elapsedTime = 0;

        while (elapsedTime < countdownTime)
        {
            elapsedTime += Time.deltaTime;
            countdownText.text = "Time Remaining: " + Mathf.Ceil(countdownTime - elapsedTime).ToString();
            yield return null;
        }

        if (currentTry < numberOfTimes)
        {
            Debug.Log("Countdown finished before sequence completion.");
            countdownText.text = "Time's up!";
            this.gameObject.GetComponent<AudioSource>().PlayOneShot(loseSound);
            // Optionally, handle the timeout scenario here.
        }
    }

    private void RemoveListener(Toggle toggle, UnityEngine.Events.UnityAction<bool> listener)
    {
        toggle.onValueChanged.RemoveListener(listener);
    }

    private void AddListener(Toggle toggle, UnityEngine.Events.UnityAction<bool> listener)
    {
        toggle.onValueChanged.AddListener(listener);
    }
}
