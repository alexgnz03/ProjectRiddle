using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterHouseTrigger : MonoBehaviour
{
    public string houseSceneName = "HouseScene";  // Cambia al nombre de tu escena
    public string entryPointName = "DoorSpawnPoint";  // Nombre del empty en la otra escena
    public bool useEntryPoint = true;

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(useEntryPoint == true)
            {
                SceneLoader.LoadSceneWithEntry(houseSceneName, entryPointName);
            }

            else
            { 
                FightUIController.Instance.FightElements.SetActive(true);
                FightStateController.Instance.onFight = true;
                SceneLoader.LoadSceneNoEntry(houseSceneName); 
            }
            
        }
    }
}
