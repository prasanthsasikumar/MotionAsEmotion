using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckCups : MonoBehaviour
{
    public List<GameObject> cups;
    public UnityEvent onCupsFell;

    private void Update()
    {
        //Check if all cups are below 0.5f on the y-axis
        foreach (GameObject cup in cups)
        {
            if (cup.transform.position.y > 0.5f)
            {
                onCupsFell.Invoke();
                break;
            }
        }

    }
}
