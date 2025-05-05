using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;
    public GameObject cursorPrefab;

    public static bool inventoryIsOpen = false;
    public static bool playerIsInteracting = false;

    private InventoryCursor invCursor;
    private List<Slot> slots = new List<Slot>();

    // Start is called before the first frame update
    void Start()
    {

        for(int i = 0; i < slotCount; i++)
        {
            
            Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();
            slot.slotID = i;
            slots.Add(slot);
            Debug.Log("Slot " + (i+1));

            if (i == 0)
            {
                cursorPrefab = Instantiate(cursorPrefab, slot.transform);
                cursorPrefab.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.cursor = cursorPrefab;
                invCursor = cursorPrefab.GetComponent<InventoryCursor>();
                invCursor.slotID = slot.slotID;
                invCursor.SetSlots(slots);
            }
            if(i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = item;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
