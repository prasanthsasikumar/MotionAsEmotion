using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [Header("Card Settings")]
    public List<Sprite> images;
    public Sprite backImage;
    public Transform cardParent;
    public float flipDuration = 0.5f;
    public int maxCards = 16;

    [Header("UI Settings")]
    public TextMeshPro timeTaken;
    public TextMeshPro countdownText; // UI Text to display countdown timer

    [Header("Game Mode")]
    public bool isTimedMode = false; // Boolean to switch between timed and standard modes
    public float countdown = 30f; // Countdown timer in seconds

    [Header("Audio")]
    public AudioClip winSound;
    public AudioClip loseSound;

    private List<Transform> cards = new List<Transform>();
    private Transform firstSelectedCard = null;
    private Transform secondSelectedCard = null;
    private float timer = 0f;
    private bool gameEnded = false;

    private void Start()
    {
        InitializeCards();
        ShuffleCards();
        AssignImagesToCards();
        SetAllCardsToBackImage();

        if (isTimedMode)
        {
            StartCoroutine(Timer());
        }
        else
        {
            countdownText.gameObject.SetActive(false); // Hide countdown text in standard mode
        }
        gameObject.AddComponent<AudioSource>();
    }

    private void InitializeCards()
    {
        foreach (Transform child in cardParent)
        {
            if (cards.Count >= maxCards)
                break;

            Transform card = child.GetChild(0).GetChild(0);
            cards.Add(card);
            card.gameObject.AddComponent<Card>();
        }
    }

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

    private void AssignImagesToCards()
    {
        List<Sprite> selectedImages = new List<Sprite>();
        for (int i = 0; i < cards.Count / 2; i++)
        {
            Sprite image = images[i];
            selectedImages.Add(image);
            selectedImages.Add(image);
        }

        selectedImages.Shuffle();

        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i].GetComponent<Card>();
            card.SetImage(selectedImages[i]);
        }
    }

    public void OnCardSelected(Transform selectedCard)
    {
        Card selectedCardComponent = selectedCard.GetComponent<Card>();
        if (selectedCardComponent.solved)
            return;

        if (firstSelectedCard == null)
        {
            firstSelectedCard = selectedCard;
            StartCoroutine(FlipCard(firstSelectedCard, true));
        }
        else if (secondSelectedCard == null)
        {
            secondSelectedCard = selectedCard;
            StartCoroutine(FlipCard(secondSelectedCard, true));
            CheckForMatch();
        }
    }

    private void CheckForMatch()
    {
        Card firstCardComponent = firstSelectedCard.GetComponent<Card>();
        Card secondCardComponent = secondSelectedCard.GetComponent<Card>();

        if (firstCardComponent.frontImage == secondCardComponent.frontImage)
        {
            MarkCardsAsSolved(firstCardComponent, secondCardComponent);
            if (AllCardsMatched())
            {
                gameEnded = true;
                AudioSource audioSource = GetComponent<AudioSource>();
                audioSource.PlayOneShot(winSound);
            }
        }
        else
        {
            StartCoroutine(ResetCards());
        }
    }

    private void MarkCardsAsSolved(Card firstCard, Card secondCard)
    {
        firstCard.SetInteractable(false);
        secondCard.SetInteractable(false);
        firstCard.SetGrayedOut();
        secondCard.SetGrayedOut();
        firstSelectedCard = null;
        secondSelectedCard = null;
    }

    private IEnumerator FlipCard(Transform card, bool flip)
    {
        float elapsedTime = 0f;
        Vector3 startRotation = card.localEulerAngles;
        Vector3 endRotation = flip ? new Vector3(0, 180, 0) : Vector3.zero;

        while (elapsedTime < flipDuration)
        {
            card.localEulerAngles = Vector3.Lerp(startRotation, endRotation, elapsedTime / flipDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Image cardImage = card.GetComponent<Image>();
        Card cardComponent = card.GetComponent<Card>();

        cardImage.sprite = flip ? cardComponent.frontImage : backImage;
        card.localEulerAngles = endRotation;
    }

    private IEnumerator ResetCards()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FlipCard(firstSelectedCard, false));
        StartCoroutine(FlipCard(secondSelectedCard, false));
        firstSelectedCard.GetComponent<Card>().SetInteractable(true);
        secondSelectedCard.GetComponent<Card>().SetInteractable(true);
        firstSelectedCard = null;
        secondSelectedCard = null;
    }

    private IEnumerator Timer()
    {
        while (!gameEnded && countdown > 0)
        {
            countdown -= Time.deltaTime;
            countdownText.text = $"Time left: {Mathf.Ceil(countdown)}s";
            yield return null;
        }

        if (countdown <= 0 && !gameEnded)
        {
            countdownText.text = "Time's up!";
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(loseSound);
            gameEnded = true;
        }

        timeTaken.text = $"Time taken: {timer:F2} seconds";
        Debug.Log($"Time taken: {timer:F2} seconds");
    }

    private bool AllCardsMatched()
    {
        foreach (Transform card in cards)
        {
            if (card.GetComponent<Card>().IsInteractable())
                return false;
        }
        return true;
    }

    public void ResetGame()
    {
        timer = 0f;
        countdown = 30f; // Reset countdown for the next game
        gameEnded = false;
        timeTaken.text = string.Empty;
        countdownText.text = $"Time left: {countdown}s";
        countdownText.gameObject.SetActive(isTimedMode); // Show/hide countdown text based on mode
        ShuffleCards();
        AssignImagesToCards();
        SetAllCardsToBackImage();
        if (isTimedMode)
            StartCoroutine(Timer());
        else
            countdownText.gameObject.SetActive(false); 
    }

    private void SetAllCardsToBackImage()
    {
        foreach (Transform card in cards)
        {
            card.GetComponent<Image>().sprite = backImage;
        }
    }
}

public static class ListExtensions
{
    private static readonly System.Random random = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
