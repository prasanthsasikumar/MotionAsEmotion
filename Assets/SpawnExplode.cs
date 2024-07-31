using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnExplode : MonoBehaviour
{
    public GameObject explodePrefab;
    public void Explode()
    {
        // Spawn explode prefab
        GameObject explosion = Instantiate(explodePrefab);
        explosion.transform.position = transform.position;
        //BroadcastMessage("Explode");
    }
}
