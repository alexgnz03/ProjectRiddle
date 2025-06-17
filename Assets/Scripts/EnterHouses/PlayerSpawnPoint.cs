using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    void Start()
    {
        if (!string.IsNullOrEmpty(SceneLoader.entryPointName))
        {
            GameObject entryPoint = GameObject.Find(SceneLoader.entryPointName);
            if (entryPoint != null)
            {
                transform.position = entryPoint.transform.position;
                transform.rotation = entryPoint.transform.rotation;
            }
            else
            {
                Debug.LogWarning("Spawn point not found: " + SceneLoader.entryPointName);
            }
        }
    }
}
