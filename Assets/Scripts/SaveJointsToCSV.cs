using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine.UIElements;

public class SaveJointsToCSV : MonoBehaviour
{
    // List of joint transforms to be saved
    [SerializeField]
    private List<Transform> _jointTransforms;

    public bool pauseStreaming = false;
    private bool setUpCompleted = false;
    private HandVisual handVisual;

    public enum HandType
    {
        Left,
        Right
    }
    public HandType handType;

    // Path to save the CSV file
    private string csvFilePath;
    private StreamWriter writer;

    // Start is called before the first frame update
    public void Setup()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string timeString = System.DateTime.Now.ToString("HH-mm-ss");
        string filename = timeString + "-" + sceneName;
        string fileName = handType == HandType.Left ? filename + "-left_hand.csv" : filename + "-right_hand.csv";
        csvFilePath = Path.Combine(Application.persistentDataPath, fileName);
        handVisual = GetComponent<HandVisual>();
        if (handVisual != null)
        {
            _jointTransforms = new List<Transform>(handVisual.Joints);
        }
        else
        {
            Debug.LogError("HandVisual component not found.");
            return;
        }

        writer = new StreamWriter(csvFilePath);
        // Write the header
        writer.WriteLine("Timestamp,JointName,PositionX,PositionY,PositionZ,RotationX,RotationY,RotationZ,RotationW");
        setUpCompleted = true;
    }

    private void SaveJointsDataToCSV()
    {
        string timestamp = DateTime.Now.ToString("o");

        
        float pinchStrength = handVisual.Hand.GetFingerPinchStrength(HandFinger.Index);
        string pinchLine = $"{timestamp},pinchStrength,{pinchStrength},0,0,0,0,0,0";
        writer.WriteLine(pinchLine);

        // Write each joint's data with timestamp
        foreach (Transform joint in _jointTransforms)
        {
            string jointName = joint.name;
            Vector3 position = joint.position;
            Quaternion rotation = joint.rotation;

            string line = $"{timestamp},{jointName},{position.x},{position.y},{position.z},{rotation.x},{rotation.y},{rotation.z},{rotation.w}";
            writer.WriteLine(line);
        }
    }

    private void Update()
    {
        if (pauseStreaming || !setUpCompleted)
        {
            return;
        }
        // Call the following function 10 times per second
        if (Time.frameCount % 6 == 0)
        {
            SaveJointsDataToCSV();
        }
    }

    private void OnDestroy()
    {
        // Ensure the writer is properly closed when the object is destroyed
        if (writer != null)
        {
            writer.Close();
            writer = null;
            Debug.Log($"CSV file closed: {csvFilePath}");
        }
    }
}
