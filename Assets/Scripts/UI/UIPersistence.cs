using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPersistence : MonoBehaviour
{
    private static bool uiExists;

    void Awake()
    {
        if (!uiExists)
        {
            DontDestroyOnLoad(gameObject);
            uiExists = true;
        }
        else
        {
            Destroy(gameObject); // Ya hay uno
        }
    }
}
