using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SkyBoxFetcher : MonoBehaviour
{ 
    // Replace with your API endpoint URL
    string url = "https://backend.blockadelabs.com/api/v1/imagine/myRequests";
    // Replace with your API key
    string apiKey = "yFT96DxVwAE8YvyUuBanWD7Q6ntAsx3TLifd0GqMv005ieuYNEQBCBzyqTWl";

    void Start()
    {
        StartCoroutine(GetRequests());
    }

    IEnumerator GetRequests()
    {
        // Create a UnityWebRequest
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Set headers
        request.SetRequestHeader("Accept", "*/*");
        request.SetRequestHeader("Accept-Encoding", "gzip, deflate, br");
        request.SetRequestHeader("x-api-key", apiKey);

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            // Parse JSON response
            string jsonText = request.downloadHandler.text;
            Debug.Log("Response: " + jsonText);

            // Deserialize JSON to object
            response = JsonUtility.FromJson<Response>(jsonText);

            // Check if data field exists
            if (response.data != null)
            {
                // Create an array to store the data
                List<Data> dataArray = new List<Data>();

                // Loop through each item in the data array
                foreach (var item in response.data)
                {
                    dataArray.Add(item);
                    // Access individual fields like item.title, item.prompt, etc.
                    Debug.Log("Title: " + item.title);
                    Debug.Log("Prompt: " + item.prompt);
                    Debug.Log("File URL: " + item.file_url);
                    Debug.Log("-------------------");
                }

                // Now 'dataArray' contains all the data objects from the response
                // You can use 'dataArray' further as needed
            }
        }
    }

    // Classes for JSON deserialization
    [System.Serializable]
    public class Response
    {
        public List<Data> data;
        public int totalCount;
        public bool has_more;
    }
    public Response response;

    [System.Serializable]
    public class Data
    {
        public int id;
        public string obfuscated_id;
        public string title;
        public string prompt;
        public string file_url;
        // Add more fields as needed
    }
}
