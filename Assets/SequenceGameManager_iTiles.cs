using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SequenceGameManager_iTiles : MonoBehaviour
{
    public Transform buttonParent;
    public int difficulty = 5;
    public float minDelay = 0.5f;
    public float maxDelay = 1.5f;
    public TextMeshProUGUI timeTaken;
    public TextMeshProUGUI message;
    public BoxCollider tableCollider;
    public GameObject buttonPrefab;

    public List<GameObject> buttons = new List<GameObject>();
    private List<GameObject> sequence = new List<GameObject>();
    private int currentSequenceIndex = 0;
    private float timer = 0f;
    private bool gameEnded = false;

    void Start()
    {
        PlaceButtonsOnTable();
        //GetButtonList();
        //StartCoroutine(ShowSequence());
        //StartCoroutine(Timer());
    }

    public void PlaceButtonsOnTable()
    {
        // Use the table collider to determine the table's size and place buttons accordingly under buttonParent
        Vector3 tableSize = tableCollider.size;
        Vector3 tableCenter = tableCollider.center;
        float buttonDiameter = 0.3f;
        float buffer = 0.1f; // Buffer distance around edges

        // Calculate the starting positions with buffer
        Vector3 tablePosition = tableCollider.transform.position;
        float minX = tablePosition.x + tableCenter.x - tableSize.x / 2 + buffer;
        float maxX = tablePosition.x + tableCenter.x + tableSize.x / 2 - buffer;
        float minZ = tablePosition.z + tableCenter.z - tableSize.z / 2 + buffer;
        float maxZ = tablePosition.z + tableCenter.z + tableSize.z / 2 - buffer;
        float y = tablePosition.y + tableCenter.y + tableSize.y / 2;

        // Print all values
        Debug.Log("tableSize: " + tableSize);
        Debug.Log("tableCenter: " + tableCenter);
        Debug.Log("tablePosition: " + tablePosition);
        Debug.Log("minX: " + minX);
        Debug.Log("maxX: " + maxX);
        Debug.Log("minZ: " + minZ);
        Debug.Log("maxZ: " + maxZ);
        Debug.Log("y: " + y);

        int numberOfCubes = 10; // Adjust this number as needed
        HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();

        for (int i = 0; i < numberOfCubes; i++)
        {
            Vector3 buttonPosition = new Vector3();
            bool validPosition = false;

            // Ensure unique positions
            while (!validPosition)
            {
                float randomX = Random.Range(minX, maxX);
                float randomZ = Random.Range(minZ, maxZ);
                buttonPosition = new Vector3(randomX, y, randomZ);

                // Check if position overlaps with existing cubes
                bool overlaps = false;
                foreach (Vector3 pos in occupiedPositions)
                {
                    if (Vector3.Distance(pos, buttonPosition) < buttonDiameter)
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (!overlaps)
                {
                    validPosition = true;
                }
            }

            // Place a small cube at the buttonPosition
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            cube.transform.position = buttonPosition;
            cube.transform.parent = buttonParent;

            occupiedPositions.Add(buttonPosition);
        }
    }



    public void GetButtonList()
    {
        foreach (Transform child in buttonParent)
        {
            if (child.gameObject.name.Contains("BigRedButton"))
            {
                buttons.Add(child.gameObject);
            }
        }
        //SetButtonColor(buttons[2], 0.5f);
    }


    IEnumerator ShowSequence()
    {
        sequence.Clear();
        List<int> indices = new List<int>();

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

        foreach (GameObject button in buttons)
        {
            SetButtonColor(button, 0.5f);
            //button.GetComponent<Button>().interactable = false;
        }

        foreach (GameObject button in sequence)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            SetButtonColor(button, 1);
            yield return new WaitForSeconds(minDelay);
            SetButtonColor(button, 0);
        }

        foreach (GameObject button in buttons)
        {
           // button.GetComponent<Button>().interactable = true;
        }

        currentSequenceIndex = 0;
        message.text = "Repeat the sequence!";
    }

    void SetButtonColor(GameObject button, float value)
    {
        Transform buttonMesh = button.transform.Find("Button/Visuals/ButtonVisual/Button");
        if (buttonMesh != null)
        {
            MeshRenderer renderer = buttonMesh.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.material != null)
            {
                Material materialInstance = renderer.material;
                materialInstance.SetFloat("_highlight", value);
            } else
            {
                Debug.LogError("Could not find the renderer or material for the button!");
            }
        }
    }

    public void OnButtonSelected(GameObject selectedButton)
    {
        if (sequence[currentSequenceIndex] == selectedButton)
        {
            currentSequenceIndex++;
            if (currentSequenceIndex >= sequence.Count)
            {
                gameEnded = true;
                message.text = "You win!";
            }
        }
        else
        {
            gameEnded = true;
            message.text = "You lose!";
        }
    }

    IEnumerator Timer()
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
        // Reset the timer and gameEnded flag
        timer = 0f;
        gameEnded = false;
        timeTaken.text = "";
        message.text = "";

        // Clear the current sequence
        sequence.Clear();

        // Reset the button states
        foreach (GameObject button in buttons)
        {
           // button.GetComponent<Button>().interactable = true;
            SetButtonColor(button, 0);
        }

        // Show the sequence again
        StartCoroutine(ShowSequence());
        StartCoroutine(Timer());
    }
}
