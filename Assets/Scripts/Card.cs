using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private Toggle toggle;
    private bool isInteractable = true;
    public Sprite frontImage;
    public bool solved = false;

    void Start()
    {
        toggle = this.transform.parent.transform.parent.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnCardClicked);
    }

    public void SetImage(Sprite image)
    {
        GetComponent<Image>().sprite = image;  
        frontImage = image;
        print("SetImage: " + image.name);
    }

    public void OnCardClicked(bool isOn)
    {
        if (!isOn)
            return;

        if (isInteractable)
        {
            isInteractable = false;

            if (FindObjectOfType<CardManager>())
                FindObjectOfType<CardManager>().OnCardSelected(transform);
            else
                FindObjectOfType<SequenceGameManager>().OnCardSelected(transform);

            Debug.Log("Card clicked: " + transform.GetComponent<Card>().frontImage);  
        }
    }

    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
        //button.interactable = interactable;
    }

    public bool IsInteractable()
    {
        return isInteractable;
    }

    public void SetGrayedOut()
    {
        GetComponent<Image>().color = Color.grey;
        this.solved = true;
        toggle.onValueChanged.RemoveAllListeners();
    }
}
