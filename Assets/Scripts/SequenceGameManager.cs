using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SequenceGameManager : MonoBehaviour
{
    public List<Sprite> images;
    public Sprite backImage;
    public Transform cardParent;
    public float flipDuration = 0.5f;
    public int maxCards = 16;
    public int difficulty = 5;
    public float minDelay = 0.5f;
    public float maxDelay = 1.5f;
    public TextMeshProUGUI timeTaken;
    public TextMeshProUGUI message;

    private List<Transform> cards = new List<Transform>();
    private List<Transform> sequence = new List<Transform>();
    private int currentSequenceIndex = 0;
    private float timer = 0f;
    private bool gameEnded = false;

    void Start()
    {
        InitializeCards();
        ShuffleCards();
        AssignImagesToCards();
        StartCoroutine(ShowSequence());
        StartCoroutine(Timer());
    }

    void InitializeCards()
    {
        foreach (Transform child in cardParent)
        {
            if (cards.Count >= maxCards)
                break;
            cards.Add(child.GetChild(0).GetChild(0));
            cards[cards.Count - 1].AddComponent<Card>();
        }
    }

    void ShuffleCards()
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Transform temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    void AssignImagesToCards()
    {
        List<Sprite> selectedImages = new List<Sprite>();
        for (int i = 0; i < cards.Count / 2; i++)
        {
            Sprite image = images[i];
            selectedImages.Add(image);
            selectedImages.Add(image);
        }

        selectedImages.Shuffle(); // Shuffle the selected images list

        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i].GetComponent<Card>();
            card.SetImage(selectedImages[i]);
        }
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
                index = Random.Range(0, cards.Count);
            } while (indices.Contains(index));

            indices.Add(index);
            sequence.Add(cards[index]);
        }

        foreach (Transform card in cards)
        {
            card.GetComponent<Image>().sprite = backImage;
            card.GetComponent<Card>().SetInteractable(false);
        }

        foreach (Transform card in sequence)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            StartCoroutine(FlipCard(card, true));
            yield return new WaitForSeconds(flipDuration);
            StartCoroutine(FlipCard(card, false));
        }

        foreach (Transform card in cards)
        {
            card.GetComponent<Card>().SetInteractable(true);
        }

        currentSequenceIndex = 0;
        message.text = "Repeat the sequence!";
    }

    public void OnCardSelected(Transform selectedCard)
    {
        if (selectedCard.GetComponent<Card>().solved)
            return;

        if (sequence[currentSequenceIndex] == selectedCard)
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

    IEnumerator FlipCard(Transform card, bool flip)
    {
        float elapsedTime = 0f;
        Vector3 startRotation = card.localEulerAngles;
        Vector3 endRotation = flip ? new Vector3(0, 180, 0) : Vector3.zero;

        while (elapsedTime < flipDuration)
        {
            card.localEulerAngles = Vector3.Lerp(startRotation, endRotation, (elapsedTime / flipDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (flip)
            card.GetComponent<Image>().sprite = card.GetComponent<Card>().frontImage;
        else
            card.GetComponent<Image>().sprite = backImage;

        card.localEulerAngles = endRotation;
    }

    IEnumerator Timer()
    {
        while (!gameEnded)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        timeTaken.text = "Time taken: " + timer;
        Debug.Log("Time taken: " + timer);
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

        // Reset the card states
        foreach (Transform card in cards)
        {
            card.GetComponent<Card>().SetInteractable(true);
            card.GetComponent<Card>().solved = false;
            card.GetComponent<Image>().sprite = backImage;
        }

        // Reinitialize cards, shuffle, assign images, and show the sequence
        ShuffleCards();
        AssignImagesToCards();
        StartCoroutine(ShowSequence());
        StartCoroutine(Timer());
    }
}

