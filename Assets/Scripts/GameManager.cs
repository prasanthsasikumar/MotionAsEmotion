using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Stage
    {
        public string stageName;
        public List<GameObject> componentsToEnable;
        public List<GameObject> componentsToDisable;

        public List<UnityEvent> eventsToInvoke;
    }

    public List<Stage> stages;
    private int currentStageIndex = 0;

    void Start()
    {
        if (stages.Count > 0)
        {
            ProceedToNextStage();
        }
        else
        {
            Debug.LogError("No stages defined in the GameManager.");
        }
    }

    public void ProceedToNextStage()
    {
        if (currentStageIndex < stages.Count)
        {
            
            foreach (var component in stages[currentStageIndex].componentsToDisable)
            {
                component.SetActive(false);
            }

            // Enable current stage components
            foreach (var component in stages[currentStageIndex].componentsToEnable)
            {
                component.SetActive(true);
            }

            Debug.Log("Proceeding to stage: " + stages[currentStageIndex].stageName);

            // Invoke events
            foreach (var unityEvent in stages[currentStageIndex].eventsToInvoke)
            {
                unityEvent.Invoke();
            }

            currentStageIndex++;
        }
        else
        {
            Debug.Log("All stages completed.");
        }
    }

    //reload scene
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
