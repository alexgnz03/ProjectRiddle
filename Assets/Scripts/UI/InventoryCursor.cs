using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCursor : MonoBehaviour
{
    private List<Slot> slots;
    public int slotID;

    public float moveDelay = 0.2f; // tiempo entre movimientos permitidos (en segundos)
    private float lastMoveTime = 0f;

    public void ProcessMove(Vector2 input)
    {
        // Si no ha pasado suficiente tiempo desde el último movimiento, salimos
        if (Time.time - lastMoveTime < moveDelay)
            return;

        bool moved = false;

        if (input.x > 0.5f)
        {
            slotID++;
            moved = true;
        }
        else if (input.x < -0.5f)
        {
            slotID--;
            moved = true;
        }

        if (moved)
        {
            lastMoveTime = Time.time; // registramos el tiempo del movimiento

            slotID = Mathf.Clamp(slotID, 0, slots.Count - 1);

            // Mover el cursor al nuevo slot
            Transform newSlotTransform = slots[slotID].transform;
            this.transform.SetParent(newSlotTransform);
            this.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    public void Interact()
    {
        Debug.Log("Interactuando con el objeto: " + slots[slotID].currentItem);
        InteractionDetector.interactableInRange?.ShowObject(slots[slotID].currentItem);
    }

    //Getters y setters
    public void SetSlots(List<Slot> newSlots)
    {
        slots = newSlots;
    }
}