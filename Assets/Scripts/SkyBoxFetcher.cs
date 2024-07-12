using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SkyBoxFetcher : MonoBehaviour
{ 
    // Replace with your API endpoint URL
    string url = "https://backend.blockadelabs.com/api/v1/imagine/myRequests";
    // Replace with your API key
    public string apiKey = "";

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
                // Get the first item
                var firstItem = response.data[0];

                // Get the file URL of the first item
                string fileUrl = firstItem.file_url;

                // Download the texture
                UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(fileUrl);
                yield return textureRequest.SendWebRequest();

                if (textureRequest.result == UnityWebRequest.Result.ConnectionError ||
                    textureRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error downloading texture: " + textureRequest.error);
                }
                else
                {
                    // Get downloaded texture
                    Texture2D texture = DownloadHandlerTexture.GetContent(textureRequest);

                    // Create a material for the skybox
                    Material skyboxMaterial = new Material(Shader.Find("Skybox/Panoramic"));

                    // Set the texture to the skybox material
                    skyboxMaterial.SetTexture("_MainTex", texture);

                    // Apply the skybox material to the scene's skybox
                    RenderSettings.skybox = skyboxMaterial;
                }
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
