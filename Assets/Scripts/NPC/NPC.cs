using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogues dialogueData;
    public GameObject correctObject;
    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;
    private string[] currentDialogue;

    private enum giveItemCase { DontGive, EndDialogue, CorrectObject }
    [SerializeField]
    private giveItemCase giveItem = giveItemCase.DontGive;
    [SerializeField]
    private GameObject itemToGive;
    private bool itemAlreadyGived = false;

    private enum QuestState { NotStarted, InProgress, Completed}
    private QuestState questState = QuestState.NotStarted;

    void Start()
    {
        dialogueUI = DialogueController.Instance;

        if (dialogueData == null)
            return;

        currentDialogue = dialogueData.dialogueLines;
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        Debug.Log("Interact");
        //Si no hay datos de diálogos
        if (dialogueData == null)
            return;

        if (isDialogueActive)
        {
            NextLine(currentDialogue);
        }
        else if (isDialogueActive == false)
        {
            StartDialogue();
        }
        Debug.Log("El dialogueIndexen este momento es: " + dialogueIndex);
    }

    public void ShowObject(GameObject showedObject)
    {
        Debug.Log("El dialogueIndex cuando muestro el objeto es: " + dialogueIndex);
        if (showedObject != null)
        {
            Debug.Log("Mostrando Objeto " + showedObject.name + " al NPC");
            if (showedObject.GetComponent<Item>().itemData == correctObject.GetComponent<Item>().itemData)
            {
                isDialogueActive = false;
                Debug.Log("El objeto es correcto");
                currentDialogue = dialogueData.correctObjectLines;

                if (giveItem == giveItemCase.CorrectObject && itemAlreadyGived == false)
                {
                    InventoryController.Instance.AddItem(itemToGive);
                    itemAlreadyGived = true;
                }
                Interact();
            }
            else
            {
                isDialogueActive = false;
                Debug.Log("El objeto es erróneo: " + correctObject);
                currentDialogue = dialogueData.wrongObjectLines;
                Interact();
            }
        }
        else
        {
            Debug.Log("No hay objeto en el espacio elegido");
        }
        
    }

    //Lógica de escritura

    void StartDialogue()
    {

        //Sync with quest data
        SyncQuestState();

        //Set dialogue line based on questState
        if(questState == QuestState.NotStarted)
        {
            dialogueIndex = 0;
        }
        else if(questState == QuestState.InProgress)
        {
            dialogueIndex = dialogueData.questIntProgressIndex;
        }
        else if(questState == QuestState.Completed)
        {
            dialogueIndex = dialogueData.questCompletedIndex;
        }


        isDialogueActive = true;

        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);

        PauseController.SetPause(true);

        DisplayCurrentLine(currentDialogue);

        InventoryController.playerIsInteracting = true;

        Debug.Log("Empezó el diálogo");
    }

    private void SyncQuestState()
    {
        if (dialogueData.quest == null) return;

        string questID = dialogueData.quest.questID;

        //Future update add completing quest and handing in!
        if(QuestController.Instance.IsQuestCompleted(questID) || QuestController.Instance.IsQuestHandedIn(questID))
        {
            questState = QuestState.Completed;
        }
        else if (QuestController.Instance.IsQuestActive(questID))
        {
            questState = QuestState.InProgress;
        }
        else
        {
            questState = QuestState.NotStarted;
        }
    }

    void NextLine(string[] dialogue)
    {
        if (isTyping)
        {
            //Skipear animación de texto
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogue[dialogueIndex]);
            isTyping = false;
        }

        //Clear Choices
        dialogueUI.ClearChoices();

        //Check endDialogueLines
        if(dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        //Check if choices & display
        foreach(DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if(dialogueChoice.dialogueIndex == dialogueIndex)
            {
                if (currentDialogue == dialogueData.dialogueLines)
                {
                    DisplayChoices(dialogueChoice);
                    return;
                }
            }
        }
        
        if(++dialogueIndex < dialogue.Length)
        {
            //Si hay otra línea, escribirla
            DisplayCurrentLine(currentDialogue);
        }
        else
        {
            if (giveItem == giveItemCase.EndDialogue && itemAlreadyGived == false)
            {
                InventoryController.Instance.AddItem(itemToGive);
                itemAlreadyGived = true;
            }
            EndDialogue();
        }
    }

    IEnumerator TypeLine(string[] dialogue)
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");

        foreach (char letter in dialogue[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if(dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine(currentDialogue);
        }
    }

    void DisplayChoices(DialogueChoice choice)
    {
        if (choice.choices == null ||
            choice.nextDialogueIndexes == null ||
            choice.givesQuest == null)
        {
            Debug.LogError($"[NPC: {dialogueData.npcName}] DialogueChoice incompleto o nulo.");
            return;
        }

        if (choice.choices.Length != choice.nextDialogueIndexes.Length ||
            choice.choices.Length != choice.givesQuest.Length)
        {
            Debug.LogError($"[NPC: {dialogueData.npcName}] Inconsistencia de diálogos: choices ({choice.choices.Length}), nextIndexes ({choice.nextDialogueIndexes.Length}), givesQuest ({choice.givesQuest.Length})");
            return;
        }

        for (int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexes[i];
            bool givesQuest = choice.givesQuest[i];

            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex, givesQuest));
        }
    }

    void ChooseOption(int nextIndex, bool givesQuest)
    {
        if(givesQuest)
        {
            QuestController.Instance.AcceptQuest(dialogueData.quest);
            questState = QuestState.InProgress;
        }
        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine(currentDialogue);
    }

    void DisplayCurrentLine(string[] dialogue)
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine(dialogue));
    }

    public void EndDialogue()
    {
        if (questState == QuestState.Completed && !QuestController.Instance.IsQuestHandedIn(dialogueData.quest.questID))
        {
            Debug.Log("La quest deberia terminar aqui");
            HandleQuestCompletion(dialogueData.quest);
        }

        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        if (InventoryController.inventoryIsOpen == false) {
            PauseController.SetPause(false);
        }
        
        InventoryController.playerIsInteracting = false;
        currentDialogue = dialogueData.dialogueLines;
    }

    void HandleQuestCompletion(Quest quest)
    {
        RewardsController.Instance.GiveQuestReward(quest);
        QuestController.Instance.HandInQuest(quest.questID);
    }
}
