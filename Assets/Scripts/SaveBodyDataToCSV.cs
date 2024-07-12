using System;
using System.IO;
using UnityEngine;

public class SaveBodyDataToCSV : MonoBehaviour
{
    // Game objects to track
    private GameObject centerEyeAnchor;
    private GameObject oculusHandL;
    private GameObject oculusHandR;

    // Path to save the CSV file
    private string csvFilePath;
    private StreamWriter writer;

    public bool pauseStreaming= false;

    // Start is called before the first frame update
    void Start()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string filename = Time.time.ToString() + "-" + sceneName + "-body.csv";
        csvFilePath = Path.Combine(Application.persistentDataPath, "body.csv");

        // Find the game objects
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor");
        oculusHandL = GameObject.Find("OculusHand_L");
        oculusHandR = GameObject.Find("OculusHand_R");

        if (centerEyeAnchor == null || oculusHandL == null || oculusHandR == null)
        {
            Debug.LogError("One or more required game objects not found.");
            return;
        }

        // Open or create the file
        writer = new StreamWriter(csvFilePath);
        // Write the header
        writer.WriteLine("Timestamp,ObjectName,PositionX,PositionY,PositionZ,RotationX,RotationY,RotationZ,RotationW");
    }

    private void WriteBodyDataToCSV()
    {
        string timestamp = DateTime.Now.ToString("o");

        // Write data for each tracked object
        WriteTransformData(timestamp, centerEyeAnchor.transform, "CenterEyeAnchor");
        WriteTransformData(timestamp, oculusHandL.transform, "OculusHand_L");
        WriteTransformData(timestamp, oculusHandR.transform, "OculusHand_R");
    }

    private void WriteTransformData(string timestamp, Transform transform, string objectName)
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        string line = $"{timestamp},{objectName},{position.x},{position.y},{position.z},{rotation.x},{rotation.y},{rotation.z},{rotation.w}";
        writer.WriteLine(line);
    }

    private void Update()
    {
        if (pauseStreaming)
        {
            SaveJointsToCSV[] instances = FindObjectsOfType<SaveJointsToCSV>();
            foreach (SaveJointsToCSV instance in instances)
            {
                instance.pauseStreaming = true;
            }
            return;
        }
        // Call the following function 10 times per second
        if (Time.frameCount % 6 == 0)
        {
            WriteBodyDataToCSV();
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
