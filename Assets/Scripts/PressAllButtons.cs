using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressAllButtons : MonoBehaviour
{
    public int count = 0;
    public UnityEvent onAllButtonsPressed;  
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void IncrementCount()
    {

       count++;
        if (count > 10)
        {
            Debug.Log("All buttons pressed");
            onAllButtonsPressed.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
