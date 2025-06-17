using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{

    public RectTransform backgroundImage; // asignar en inspector
    public RectTransform character;       // el personaje UI que se mueve

    private float minX, maxX;

    // Start is called before the first frame update
    void Start()
    {
        // Calculamos los bordes izquierdo y derecho del área limitada
        // En espacio local relativo al Canvas o parent

        // backgroundImage.rect es el tamaño en local space
        float halfWidth = backgroundImage.rect.width / 2f;

        // minX y maxX serán en localPosition para el personaje
        minX = backgroundImage.localPosition.x - halfWidth;
        maxX = backgroundImage.localPosition.x + halfWidth;
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal"); // -1 a 1
        if (inputX != 0)
        {
            Vector3 pos = character.localPosition;
            //pos.x += inputX * speed * Time.deltaTime;

            // Limitar el movimiento al rango
            pos.x = Mathf.Clamp(pos.x, minX, maxX);

            character.localPosition = pos;
        }
    }
}
