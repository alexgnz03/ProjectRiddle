using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;

    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;
    public GameObject cursorPrefab;

    public static bool inventoryIsOpen = false;
    public static bool playerIsInteracting = false;

    private InventoryCursor invCursor;
    private List<Slot> slots = new List<Slot>();

    public static InventoryController Instance { get; private set; }
    Dictionary<int, int> itemsCountCache = new();
    public event Action OnInventoryChanged;

    public GameObject TestPrefab;

    [Header("Referencias UI para el cursor")]
    public GameObject itemIconGO;
    public GameObject itemNameGO;
    public GameObject itemDescriptionGO;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();

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
                invCursor.InitializeUI(itemIconGO, itemNameGO, itemDescriptionGO);
            }
            if(i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = item;
            }
        }

        RebuildItemCounts();
    }

    public bool AddItem(GameObject itemPrefab)
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;

                RebuildItemCounts(); // Notificamos cambios

                return true;
            }
        }
        Debug.Log("El inventario está lleno");
        return false;
    }

    public void RebuildItemCounts()
    {
        itemsCountCache.Clear();
        
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null)
                {
                    Debug.Log($"Recuento de item ID {item.ID}");
                    itemsCountCache[item.ID] = itemsCountCache.GetValueOrDefault(item.ID, 0) + 1;
                }
            }
        }

        OnInventoryChanged?.Invoke();
    }

    public Dictionary<int, int> GetItemCounts => itemsCountCache;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("¡Se presionó la tecla K!");
            AddItem(TestPrefab);
        }
    }

    public void RemoveItemsFromInventory(int itemID, int amountToRemove)
    {
        Debug.Log("Items removidos del inventario");
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            if (amountToRemove <= 0) break;

            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot?.currentItem?.GetComponent<Item>() is Item item && item.ID == itemID)
            {
                // Como no usas stacks, cada item es una unidad:
                Destroy(slot.currentItem);
                slot.currentItem = null;

                amountToRemove--;
            }
        }

        RebuildItemCounts();
    }
}
