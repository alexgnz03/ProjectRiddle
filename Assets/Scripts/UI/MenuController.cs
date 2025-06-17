using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    // Start is called before the first frame update
    void Start()
    {
        CloseMenu();
        menuCanvas = MenuIdentifier.Instance.gameObject;
    }

    public void CloseMenu()
    {
        menuCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            OpenInventory();
        }
    }

    public void OpenInventory()
    {
        //TODO Revisar si al implementar el menu de pausa puedo intactuar, abrir el inventario, y pausar a la vez
        if (!menuCanvas.activeSelf && PauseController.IsGamePaused && !InventoryController.playerIsInteracting)
        {
            return;
        }
        menuCanvas.SetActive(!menuCanvas.activeSelf);
        PauseController.SetPause(menuCanvas.activeSelf);
    }
}
