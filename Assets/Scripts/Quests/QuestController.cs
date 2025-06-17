using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance { get; private set; }
    public List<QuestProgress> activateQuests = new();
    private QuestUIController questUI;

    public List<string> handingQuestIDs = new();

    private void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        questUI = FindObjectOfType<QuestUIController>();
        InventoryController.Instance.OnInventoryChanged += CheckInventoryForQuests;
    }

    public void AcceptQuest(Quest quest)
    {
        if (IsQuestActive(quest.questID)) return;

        activateQuests.Add(new QuestProgress(quest));

        questUI.UpdateQuestUI();
    }

    public bool IsQuestActive(string questID) => activateQuests.Exists(q => q.QuestID == questID);
    
    public bool IsQuestCompleted(string questID)
    {
        QuestProgress quest = activateQuests.Find(q => q.QuestID == questID);
        return quest != null && quest.objectives.TrueForAll(o => o.IsCompleted);
    }

    public void HandInQuest(string questID)
    {
        Debug.Log("Hnad in Quest");

        //Try remove required items
        if(!RemoveRequiredItemsFromInventory(questID))
        {
            //Quest couldn't be completed/Missing items
            return;
        }

        //Remove quest from quest log
        QuestProgress quest = activateQuests.Find(q => q.QuestID == questID);
        if (quest != null)
        {
            handingQuestIDs.Add(questID);
            activateQuests.Remove(quest);
            questUI.UpdateQuestUI();
        }
    }

    public bool IsQuestHandedIn(string questID)
    {
        return handingQuestIDs.Contains(questID);
    }

    public bool RemoveRequiredItemsFromInventory(string questID)
    {

        QuestProgress quest = activateQuests.Find(q => q.QuestID == questID);
        if (quest == null) return false;

        Dictionary<int, int> requiredItem = new();

        Debug.Log($"Quest: {quest.QuestID}, Required Items: {requiredItem.Count}");

        //Item requirements from objectives
        foreach (QuestObjective objective in quest.objectives)
        {
            if(objective.type == ObjectiveType.CollectItem && int.TryParse(objective.objectiveID, out int itemID))
            {
                requiredItem[itemID] = objective.requiredAmount;
            }
        }

        //Verify we have items
        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts;
        foreach(var item in requiredItem)
        {
            Debug.Log($"Requiere item ID {item.Key} x{item.Value}");

        if (itemCounts.GetValueOrDefault(item.Key) < item.Value)
            {
                //Not enough items to complete quest
                return false;
            }
        }

        foreach (var item in itemCounts)
        {
            Debug.Log($"Inventario tiene item ID {item.Key} x{item.Value}");
        }

        //Remove required items from inventory
        foreach (var itemRequirement in requiredItem)
        {
            Debug.Log($"Eliminando item ID {itemRequirement.Key} x{itemRequirement.Value}");

            //RemoveItemsFromInventory
            InventoryController.Instance.RemoveItemsFromInventory(itemRequirement.Key, itemRequirement.Value);
        }

        return true;
    }
    public void CheckInventoryForQuests()
    {
        Debug.Log("CheckInventoryForQuests ejecutado");

        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts;

        foreach(QuestProgress quest in activateQuests)
        {
            foreach(QuestObjective questObjective in quest.objectives)
            {
                if (questObjective.type != ObjectiveType.CollectItem) continue;
                if (!int.TryParse(questObjective.objectiveID, out int itemID)) continue;

                int newAmount = itemCounts.TryGetValue(itemID, out int count) ? Mathf.Min(count, questObjective.requiredAmount) : 0;
                Debug.Log($"Comprobando objetivo: {questObjective.description}, ID: {questObjective.objectiveID}");

                if (questObjective.currentAmount != newAmount)
                {
                    Debug.Log($"Actualizando objetivo {questObjective.description} de {questObjective.currentAmount} a {newAmount}");
                    questObjective.currentAmount = newAmount;
                }
            }
        }
        questUI.UpdateQuestUI();
    }
}
