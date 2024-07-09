using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private Toggle toggle;
    private bool isInteractable = true;
    public Sprite frontImage, backImage;
    public bool solved = false;

    void Start()
    {
        toggle = this.transform.parent.transform.parent.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnCardClicked);
        //Load the back image from the resources/Cards folder
        backImage = Resources.Load<Sprite>("Cards/cardback");
    }

    public void SetImage(Sprite image)
    {
        GetComponent<Image>().sprite = image;  
        frontImage = image;
        print("SetImage: " + image.name);
    }

    public void OnCardClicked(bool isOn)
    {
        if (isInteractable)
        {
            isInteractable = false;
            FindObjectOfType<CardManager>().OnCardSelected(transform);
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
