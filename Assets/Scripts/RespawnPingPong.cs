using Oculus.Interaction.Samples;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlingShotController : MonoBehaviour
{
    [SerializeField]
    private RespawnOnDrop[] respawnComponents;

    [SerializeField]
    private GameObject parent;

    [SerializeField]
    private int initialBallCount = 7;

    [SerializeField]
    private TextMeshProUGUI counterText;

    [SerializeField]
    private float groundThreshold = 0.3f;

    [SerializeField]
    private bool hardCondition = false;

    private int currentBallCount;
    private int minBalls = 1;
    private int maxBalls;
    private List<GameObject> cups;
    private int score = 0;
    private int currentCupIndex = 0;
    private Color originalColor;
    private Color targetColor = Color.red;

    private void Start()
    {
        UpdateBallCount();
        maxBalls = currentBallCount;
        cups = GetCups();

        if (hardCondition)
        {
            SetNextCupColor(currentCupIndex);
        }
    }

    public void OnCanvasGroupChanged()
    {
        respawnComponents = parent.GetComponentsInChildren<RespawnOnDrop>();
    }

    public void RespawnAll()
    {
        int ballsToRespawn = currentBallCount;

        foreach (var respawnComponent in respawnComponents)
        {
            if (respawnComponent.gameObject.name == "PingPongBall")
            {
                if (ballsToRespawn > 0)
                {
                    respawnComponent.gameObject.SetActive(true);
                    respawnComponent.Respawn();
                    ballsToRespawn--;
                }
                else
                {
                    respawnComponent.gameObject.SetActive(false);
                }
            }
            else
            {
                respawnComponent.Respawn();
            }
        }

        if (hardCondition)
        {
            currentCupIndex = 0;
            SetNextCupColor(currentCupIndex);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RespawnAll();
        }

        if (hardCondition)
        {
            if (cups[currentCupIndex].transform.position.y < groundThreshold)
            {
                currentCupIndex++;

                if (currentCupIndex >= cups.Count)
                {
                    IncrementScore();
                    RespawnAll();
                }
                else
                {
                    SetNextCupColor(currentCupIndex);
                }
            }
            else if (AnyCupOutOfOrder())
            {
                //RespawnAll();
            }
        }
        else if (AreAllCupsBelowGround())
        {
            IncrementScore();
            RespawnAll();
        }
    }

    private void UpdateBallCount()
    {
        currentBallCount = initialBallCount;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (parent.transform.GetChild(i).name == "PingPongBall")
            {
                currentBallCount++;
            }
        }
    }

    private List<GameObject> GetCups()
    {
        var cups = new List<GameObject>();

        foreach (var respawnComponent in respawnComponents)
        {
            if (respawnComponent.gameObject.name == "PlasticCup")
            {
                cups.Add(respawnComponent.gameObject);
            }
        }

        return cups;
    }

    private bool AreAllCupsBelowGround()
    {
        foreach (var cup in cups)
        {
            if (cup.transform.position.y >= groundThreshold)
            {
                return false;
            }
        }

        return true;
    }

    private bool AnyCupOutOfOrder()
    {
        for (int i = 0; i < cups.Count; i++)
        {
            if (i != currentCupIndex && cups[i].transform.position.y < groundThreshold)
            {
                return true;
            }
        }
        return false;
    }

    private void IncrementScore()
    {
        score++;
        counterText.text = "Score: " + score;
    }

    private void SetNextCupColor(int index)
    {
        ResetCupColors();
        var meshRenderer = cups[index].GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
            meshRenderer.material.color = targetColor;
        }
    }

    private void ResetCupColors()
    {
        foreach (var cup in cups)
        {
            var meshRenderer = cup.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material.color = originalColor;
            }
        }
    }
}
