using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    private static bool playerExists;

    void Awake()
    {
        if (!playerExists)
        {
            DontDestroyOnLoad(gameObject);
            playerExists = true;
        }
        else
        {
            Destroy(gameObject); // Ya hay uno
        }
    }
}
