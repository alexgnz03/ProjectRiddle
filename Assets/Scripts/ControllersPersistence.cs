using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllersPersistence : MonoBehaviour
{
    private static bool controllersExists;

    void Awake()
    {
        if (!controllersExists)
        {
            DontDestroyOnLoad(gameObject);
            controllersExists = true;
        }
        else
        {
            Destroy(gameObject); // Ya hay uno
        }
    }
}
