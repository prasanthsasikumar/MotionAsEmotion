using UnityEngine;

public class AlignWorld : MonoBehaviour
{
    public GameObject cubePrefab; // Assign the cube prefab in the inspector
    public Transform userCamera;  // Assign the user's camera in the inspector
    public Transform worldCenter;

    public float distance = 1.2f; // Distance in front of the user to place the cube
    public float yOffset = 0.2f;   // Vertical offset from the user's head level

    void Start()
    {
        PlaceCube();
    }

    void PlaceCube()
    {
        // Calculate the position 2 meters in front of the user
        Vector3 targetPosition = userCamera.position + userCamera.forward * distance;

        // Adjust the position to be 0.3 meters below the user's head level
        targetPosition.y -= 0.3f;

        // Instantiate the cube at the target position with no rotation
        Instantiate(cubePrefab, targetPosition, Quaternion.identity);
    }

    public void AdjustCameraToWorld()
    {
        userCamera.position = worldCenter.position + worldCenter.forward * distance;
        

    }

    void Update()
    {
        // Check if the user presses the space key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Place a new cube in front of the user
            AdjustCameraToWorld();
        }
    }
}
