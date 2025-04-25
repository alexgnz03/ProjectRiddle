using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InvInputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnMenuActions onMenu;

    private InventoryCursor cursor;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new PlayerInput();
        onMenu = playerInput.OnMenu;
        cursor = GetComponent<InventoryCursor>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //tell the playermotor to move using the value from our movement action.
        cursor.ProcessMove(onMenu.CursorMovement.ReadValue<Vector2>());
    }
    private void OnEnable()
    {
        onMenu.Enable();
    }
    private void OnDisable()
    {
        onMenu.Disable();
    }
}
