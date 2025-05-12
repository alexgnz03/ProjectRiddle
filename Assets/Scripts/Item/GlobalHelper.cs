using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalHelper
{
    public static string GenerateUniqueID(GameObject obj)
    {
        return $"{obj.scene.name}_{obj.transform.position.x}_{obj.transform.position.y}"; //Chest_3_4
    }
}
