using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerManually : MonoBehaviour
{ 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
            transform.Rotate(0, -10, 0);
        else if (Input.GetKeyDown(KeyCode.Alpha0))
            transform.Rotate(0, 10, 0);
    }
}
