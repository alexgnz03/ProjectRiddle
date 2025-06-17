using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCursor : MonoBehaviour
{
    private List<Slot> slots;
    public int slotID;
    public MenuController menuController;
    public float moveDelay = 0.2f; // tiempo entre movimientos permitidos (en segundos)
    private float lastMoveTime = 0f;
    GameObject enemy;

    private Image ItemIcon;
    private TMP_Text ItemName;
    private TMP_Text ItemDescription;

    void Awake()
    {
        menuController = transform.root.GetComponent<MenuController>();

        enemy = GameObject.FindWithTag("Enemy");

    }

    public void InitializeUI(GameObject iconGO, GameObject nameGO, GameObject descGO)
    {
        ItemIcon = iconGO.GetComponent<Image>();
        ItemName = nameGO.GetComponent<TMP_Text>();
        ItemDescription = descGO.GetComponent<TMP_Text>();
    }

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

            // Cambia de padre
            this.transform.SetParent(newSlotTransform);

            // Ponerlo como primer hijo (el más atrás visualmente)
            this.transform.SetSiblingIndex(0);

            // Centrar el cursor en el slot
            this.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        UpdateItemAttributes();
    }

    public void Interact()
    {
        if (slots[slotID].currentItem != null)
        {
            Debug.Log("Interactuando con el objeto: " + slots[slotID].currentItem);

            if (FightStateController.Instance.onFight == true)
            {
                Debug.Log("Hay un enemigo en el mapa.");
                Enemy.Instance.ShowObject(slots[slotID].currentItem);
            }
            else
            {
                InteractionDetector.interactableInRange?.ShowObject(slots[slotID].currentItem);
            }
            menuController.menuCanvas.SetActive(false);
        }

    }

    public void UpdateItemAttributes()
    {
        if (slots[slotID].currentItem != null)
        {
            Item item = slots[slotID].currentItem.GetComponent<Item>();
            ItemIcon.sprite = item.itemData.itemSprite;
            ItemName.text = item.itemData.itemName;
            ItemDescription.text = item.itemData.itemDescription;
        }
    }

    //Getters y setters
    public void SetSlots(List<Slot> newSlots)
    {
        slots = newSlots;
    }
}