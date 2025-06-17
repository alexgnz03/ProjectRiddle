using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuIdentifier : MonoBehaviour
{
    public static MenuIdentifier Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
