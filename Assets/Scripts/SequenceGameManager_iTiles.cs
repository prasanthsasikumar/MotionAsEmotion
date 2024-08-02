using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SequenceGameManager_iTiles : MonoBehaviour
{
    [Header("Game Settings")]
    public Transform buttonParent;
    public int numberOfButtons = 10;
    public int difficulty = 5;
    public float minDelay = 0.5f;
    public float maxDelay = 1.5f;
    public float buttonYOffset = 0.07f;
    public float buttonDiameter = 0.3f;
    public bool pilotStepUp = false;

    [Header("UI Elements")]
    public TextMeshProUGUI timeTaken;
    public TextMeshProUGUI infoFrame;

    [Header("Game Objects")]
    public BoxCollider tableCollider;
    public GameObject buttonPrefab;
    public AudioClip winSound, loseSound;

    private List<GameObject> buttons = new List<GameObject>();
    private List<GameObject> sequence = new List<GameObject>();
    private int currentSequenceIndex = 0;
    private float timer = 0f;
    private bool gameEnded = false;

    private void Start()
    {
        PlaceButtonsOnTable();
        StartCoroutine(ShowSequence());
        StartCoroutine(Timer());
        gameObject.AddComponent<AudioSource>();
    }

    private void PlaceButtonsOnTable()
    {
        var potentialPositions = GenerateRandomPositions();
        ShuffleList(potentialPositions);

        int numberOfCubes = Mathf.Min(numberOfButtons, potentialPositions.Count);
        for (int i = 0; i < numberOfCubes; i++)
        {
            Vector3 buttonPosition = potentialPositions[i];
            GameObject button = Instantiate(buttonPrefab, buttonPosition, Quaternion.identity, buttonParent);
            buttons.Add(button);
            button.transform.localPosition = buttonPosition;
            button.transform.Find("Button").GetComponent<InteractableUnityEventWrapper>().WhenSelect.AddListener(() => OnButtonSelected(button));
        }
    }

    private List<Vector3> GenerateRandomPositions()
    {
        var tableSize = tableCollider.bounds.size;
        var tablePosition = tableCollider.transform.position;

        float buffer = 0.1f;
        float minX = tablePosition.x - tableSize.x / 2 + buffer;
        float maxX = tablePosition.x + tableSize.x / 2 - buffer;
        float minZ = tablePosition.z - tableSize.z / 2 + buffer;
        float maxZ = tablePosition.z + tableSize.z / 2 - buffer;
        float y = tablePosition.y + tableSize.y / 2 - buttonYOffset;

        DrawTableBorder(minX, maxX, minZ, maxZ, y);

        var potentialPositions = new List<Vector3>();
        var positionSet = new HashSet<Vector3>();
        int maxAttempts = numberOfButtons * 100;

        while (potentialPositions.Count < numberOfButtons && maxAttempts > 0)
        {
            maxAttempts--;

            Vector3 newPosition;
            bool isValid;
            int maxIterations = 1000;

            do
            {
                maxIterations--;
                float x = Random.Range(minX, maxX - buttonDiameter);
                float z = Random.Range(minZ, maxZ - buttonDiameter);
                newPosition = new Vector3(x, y, z);

                isValid = true;
                foreach (var existingPosition in positionSet)
                {
                    if (Vector3.Distance(newPosition, existingPosition) < buttonDiameter)
                    {
                        isValid = false;
                        break;
                    }
                }
            } while (!isValid && maxIterations > 0);

            if (isValid)
            {
                positionSet.Add(newPosition);
                potentialPositions.Add(newPosition);
            }
        }

        if (maxAttempts <= 0)
        {
            Debug.LogWarning("Reached maximum number of attempts for placing buttons.");
        }

        return potentialPositions;
    }

    private void DrawTableBorder(float minX, float maxX, float minZ, float maxZ, float y)
    {
        Debug.DrawLine(new Vector3(minX, y, minZ), new Vector3(minX, y, maxZ), Color.red, Mathf.Infinity);
        Debug.DrawLine(new Vector3(minX, y, maxZ), new Vector3(maxX, y, maxZ), Color.red, Mathf.Infinity);
        Debug.DrawLine(new Vector3(maxX, y, maxZ), new Vector3(maxX, y, minZ), Color.red, Mathf.Infinity);
        Debug.DrawLine(new Vector3(maxX, y, minZ), new Vector3(minX, y, minZ), Color.red, Mathf.Infinity);
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    private IEnumerator ShowSequence()
    {
        yield return new WaitForSeconds(1f);
        infoFrame.text = "Ready?";
        yield return new WaitForSeconds(2f);
        infoFrame.text = "Memorize the sequence!";
        yield return new WaitForSeconds(0.5f);

        sequence.Clear();
        var indices = new List<int>();
        difficulty = Mathf.Min(difficulty, buttons.Count);

        for (int i = 0; i < difficulty; i++)
        {
            int index;
            do
            {
                index = Random.Range(0, buttons.Count);
            } while (indices.Contains(index));

            indices.Add(index);
            sequence.Add(buttons[index]);
        }

        foreach (var button in buttons)
        {
            SetButtonColor(button, 0.5f);
        }

        foreach (var button in sequence)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            SetButtonColor(button, 1);
            yield return new WaitForSeconds(minDelay);
            SetButtonColor(button, 0);
        }

        currentSequenceIndex = 0;
        infoFrame.text = "Repeat the sequence!";
    }

    private void SetButtonColor(GameObject button, float value)
    {
        var buttonMesh = button.transform.Find("Button/Visuals/ButtonVisual/Button");
        if (buttonMesh != null)
        {
            var renderer = buttonMesh.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.SetFloat("_highlight", value);
            }
            else
            {
                Debug.LogError("Could not find the renderer or material for the button!");
            }
        }
    }

    public void OnButtonSelected(GameObject selectedButton)
    {
        Debug.Log("Button selected: " + selectedButton.name);
        if (sequence[currentSequenceIndex] == selectedButton)
        {
            currentSequenceIndex++;
            SetButtonColor(selectedButton, 0.75f);
            if (currentSequenceIndex >= sequence.Count)
            {
                EndGame(true);
            }
        }
        else
        {
            EndGame(false);
        }
    }

    private void EndGame(bool won)
    {
        gameEnded = true;
        infoFrame.text = won ? "You win!" : "You lose!";
        if (pilotStepUp)
            difficulty++;
        var audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(won ? winSound : loseSound);

        foreach (var button in buttons)
        {
            SetButtonColor(button, won ? 1 : 0);
        }

        StartCoroutine(DelayFunction(2f));
        Restart();
    }

    private IEnumerator Timer()
    {
        while (!gameEnded)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        timeTaken.text = "Time taken: " + timer.ToString("F2") + " seconds";
    }

    public void Restart()
    {
        timer = 0f;
        gameEnded = false;
        timeTaken.text = "";
        infoFrame.text = "Ready..?";

        sequence.Clear();

        foreach (var button in buttons)
        {
            SetButtonColor(button, 0);
        }

        StartCoroutine(ShowSequence());
        StartCoroutine(Timer());
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    private IEnumerator DelayFunction(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
}
