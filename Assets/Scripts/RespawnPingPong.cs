using Oculus.Interaction.Samples;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class SlingShotController : MonoBehaviour
{
    public RespawnOnDrop[] respawnComponents;
    public GameObject parent;
    public int numberOfBalls = 7; 

    private int minBalls, maxBalls;

    private void Start()
    {
        minBalls = 1;
        maxBalls = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (parent.transform.GetChild(i).name == "PingPongBall")
            {
                numberOfBalls++;
            }
        }
        maxBalls = numberOfBalls;
    }
    public void OnCanvasGroupChanged()
    {
        respawnComponents = parent.transform.GetComponentsInChildren<RespawnOnDrop>();
    }

    public void RespawnAll()
    {
        int tempNumberOfBalls = numberOfBalls;
        for (int i = 0; i < respawnComponents.Length; i++)
        {
            if (respawnComponents[i].gameObject.name == "PingPongBall")
            {
                if (tempNumberOfBalls > 0)
                {
                    respawnComponents[i].gameObject.SetActive(true);
                    respawnComponents[i].Respawn();
                    tempNumberOfBalls--;
                    print("PingPongBalRespawned");
                }
                else
                {
                    respawnComponents[i].gameObject.SetActive(false);
                }

            }
            else
            {
                respawnComponents[i].Respawn();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RespawnAll();
        }
    }
}
