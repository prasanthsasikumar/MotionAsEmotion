using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SequenceGameManager : MonoBehaviour
{
    [Header("Images")]
    public List<Sprite> images;
    public Sprite backImage, highLightedImage;

    [Header("Game Settings")]
    public Transform cardParent;
    public float flipDuration = 0.5f;
    public int maxCards = 16;
    public int difficulty = 5;
    public float minDelay = 0.5f;
    public float maxDelay = 1.5f;

    [Header("UI Elements")]
    public TextMeshProUGUI timeTaken;
    public TextMeshProUGUI message;

    [Header("Audio")]
    public AudioClip winSound, loseSound;

    private List<Transform> cards = new List<Transform>();
    private List<Transform> sequence = new List<Transform>();
    private int currentSequenceIndex = 0;
    private float timer = 0f;
    private bool gameEnded = false;

    private AudioSource audioSource;

    /// <summary>
    /// Initializes the game.
    /// </summary>
    public void StartGame()
    {
        InitializeCards();
        ShuffleCards();
        AssignImagesToCards();
        StartCoroutine(ShowSequence());
        StartCoroutine(Timer());
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// Initializes card objects and adds them to the cards list.
    /// </summary>
    private void InitializeCards()
    {
        foreach (Transform child in cardParent)
        {
            if (cards.Count >= maxCards) break;
            Transform cardTransform = child.GetChild(0).GetChild(0);
            cards.Add(cardTransform);
            cardTransform.gameObject.AddComponent<Card>();
        }
    }

    /// <summary>
    /// Shuffles the cards list.
    /// </summary>
    private void ShuffleCards()
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Transform temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    /// <summary>
    /// Assigns images to cards.
    /// </summary>
    private void AssignImagesToCards()
    {
        List<Sprite> selectedImages = new List<Sprite>();
        for (int i = 0; i < cards.Count / 2; i++)
        {
            Sprite image = images[i];
            selectedImages.Add(image);
            selectedImages.Add(image);
        }

        selectedImages.Shuffle(); // Ensure you have a Shuffle extension method

        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i].GetComponent<Card>();
            card.SetImage(selectedImages[i]);
        }
    }

    /// <summary>
    /// Shows the sequence to the player.
    /// </summary>
    private IEnumerator ShowSequence()
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

        ResetCardsToBackImage();

        foreach (Transform card in sequence)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            StartCoroutine(HighlightCard(card));
            yield return new WaitForSeconds(flipDuration);
        }

        SetCardsInteractable(true);
        currentSequenceIndex = 0;
        message.text = "Repeat the sequence!";
    }

    /// <summary>
    /// Handles card selection logic.
    /// </summary>
    public void OnCardSelected(Transform selectedCard)
    {
        if (selectedCard.GetComponent<Card>().solved) return;

        if (sequence[currentSequenceIndex] == selectedCard)
        {
            currentSequenceIndex++;
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

    /// <summary>
    /// Highlights a card.
    /// </summary>
    private IEnumerator HighlightCard(Transform card)
    {
        float elapsedTime = 0f;
        while (elapsedTime < flipDuration)
        {
            card.GetComponent<Image>().sprite = highLightedImage;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        card.GetComponent<Image>().sprite = backImage;
    }

    /// <summary>
    /// Timer coroutine.
    /// </summary>
    private IEnumerator Timer()
    {
        while (!gameEnded)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        timeTaken.text = $"Time taken: {timer:F2} seconds";
        Debug.Log($"Time taken: {timer:F2} seconds");
    }

    /// <summary>
    /// Ends the game with a win or loss.
    /// </summary>
    private void EndGame(bool won)
    {
        gameEnded = true;
        message.text = won ? "You win!" : "You lose!";
        audioSource.PlayOneShot(won ? winSound : loseSound);
        StartCoroutine(DelayAndRestart(2f));
    }

    /// <summary>
    /// Restarts the game.
    /// </summary>
    public void Restart()
    {
        timer = 0f;
        gameEnded = false;
        timeTaken.text = "";
        message.text = "";
        sequence.Clear();
        ResetCardsToBackImage();
        SetCardsInteractable(true);
        ShuffleCards();
        AssignImagesToCards();
        StartCoroutine(ShowSequence());
        StartCoroutine(Timer());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    private IEnumerator DelayAndRestart(float delay)
    {
        yield return new WaitForSeconds(delay);
        Restart();
    }

    /// <summary>
    /// Resets cards to back image and makes them non-interactable.
    /// </summary>
    private void ResetCardsToBackImage()
    {
        foreach (Transform card in cards)
        {
            card.GetComponent<Image>().sprite = backImage;
            card.GetComponent<Card>().SetInteractable(false);
        }
    }

    /// <summary>
    /// Sets cards interactable state.
    /// </summary>
    /// <param name="state">State to set</param>
    private void SetCardsInteractable(bool state)
    {
        foreach (Transform card in cards)
        {
            card.GetComponent<Card>().SetInteractable(state);
        }
    }
}
