using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public List<Sprite> images;
    public Transform cardParent;
    public float flipDuration = 0.5f;
    public int maxCards = 16;

    private List<Transform> cards = new List<Transform>();
    private Transform firstSelectedCard = null;
    private Transform secondSelectedCard = null;
    private float timer = 0f;
    private bool gameEnded = false;

    void Start()
    {
        InitializeCards();
        ShuffleCards();
        AssignImagesToCards();
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

    public void OnCardSelected(Transform selectedCard)
    {
        if(selectedCard.GetComponent<Card>().solved)
            return;

        if (firstSelectedCard != null)
        print(firstSelectedCard.GetComponent<Card>().frontImage);
        if (secondSelectedCard != null)
            print(secondSelectedCard.GetComponent<Card>().frontImage);

        if (firstSelectedCard == null)
        {
            firstSelectedCard = selectedCard;
            StartCoroutine(FlipCard(firstSelectedCard, true));
        }
        else if (secondSelectedCard == null)
        {
            secondSelectedCard = selectedCard;
            StartCoroutine(FlipCard(secondSelectedCard, true));

            if (firstSelectedCard.GetComponent<Card>().frontImage == secondSelectedCard.GetComponent<Card>().frontImage)
            {
                firstSelectedCard.GetComponent<Card>().SetInteractable(false);
                secondSelectedCard.GetComponent<Card>().SetInteractable(false);
                firstSelectedCard.GetComponent<Card>().SetGrayedOut();
                secondSelectedCard.GetComponent<Card>().SetGrayedOut();
                firstSelectedCard = null;
                secondSelectedCard = null;

                if (AllCardsMatched())
                {
                    gameEnded = true;
                }
            }
            else
            {
                StartCoroutine(ResetCards());
            }
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
        if(flip)
            card.GetComponent<Image>().sprite = card.GetComponent<Card>().backImage;
        else
            card.GetComponent<Image>().sprite = card.GetComponent<Card>().frontImage;

        card.localEulerAngles = endRotation;
    }

    IEnumerator ResetCards()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FlipCard(firstSelectedCard, false));
        StartCoroutine(FlipCard(secondSelectedCard, false));
        firstSelectedCard.GetComponent<Card>().SetInteractable(true);
        secondSelectedCard.GetComponent<Card>().SetInteractable(true);
        firstSelectedCard = null;
        secondSelectedCard = null;
    }

    IEnumerator Timer()
    {
        while (!gameEnded)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Time taken: " + timer);
    }

    bool AllCardsMatched()
    {
        foreach (Transform card in cards)
        {
            if (card.GetComponent<Card>().IsInteractable())
            {
                return false;
            }
        }
        return true;
    }
}

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        System.Random rnd = new System.Random();
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
