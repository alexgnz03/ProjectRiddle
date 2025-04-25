using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;

    public static float defaultSpeed = -5f;

    public float speed = defaultSpeed;
    public float gravity = -9.8f;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update() //Intentar optimizar esto
    {
     if (PauseController.IsGamePaused)
        {
            speed = 0f; //Parar movimiento
            return;
        }
     else
        {
            speed = defaultSpeed;
        }
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.y = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        //Debug.Log(playerVelocity.y);
    }


    //Shiftear
    public void GoSlow() 
    {
            speed = speed / 2;
    }

    public void StopSlow()
    {
        speed = speed * 2;
    }
}
