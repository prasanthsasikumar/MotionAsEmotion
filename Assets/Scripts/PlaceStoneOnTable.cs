using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlaceStoneOnTable : MonoBehaviour
{
    public UnityEvent onStonePlaced;
    public int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //check if the something is placed on the table
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something placed on the table");
        if (other.gameObject.tag == "stone")
        {
            count++;
            if (count > 5)
            {
                Debug.Log("All stones placed");
                onStonePlaced.Invoke();
            }
        }
    }
}
