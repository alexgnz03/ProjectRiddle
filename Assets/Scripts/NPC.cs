using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogues dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    public GameObject correctObject;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;
    private string[] currentDialogue;

    void Start()
    {
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

        currentDialogue = dialogueData.dialogueLines;
        Debug.Log("El dialogueIndexen este momento es: " + dialogueIndex);
    }

    public void ShowObject(GameObject showedObject)
    {
        Debug.Log("El dialogueIndex cuando muestro el objeto es: " + dialogueIndex);
        if (showedObject != null)
        {
            Debug.Log("Mostrando Objeto " + showedObject.name + " al NPC");
            if (showedObject.GetComponent<Item>().itemData.itemName == correctObject.GetComponent<Item>().itemData.itemName)
            {
                Debug.Log("El objeto es correcto");
                currentDialogue = dialogueData.correctObjectLines;
                Interact();
            }
            else
            {
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
        isDialogueActive = true;
        dialogueIndex = 0;

        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);
        PauseController.SetPause(dialoguePanel.activeSelf);

        StartCoroutine(TypeLine(currentDialogue));

        InventoryController.playerIsInteracting = true;

        Debug.Log("Empezó el diálogo");
    }

    void NextLine(string[] dialogue)
    {
        if (isTyping)
        {
            //Skipear animación de texto
            StopAllCoroutines();
            dialogueText.SetText(dialogue[dialogueIndex]);
            isTyping = false;
        }
        else if(++dialogueIndex < dialogue.Length)
        {
            //Si hay otra línea, escribirla
            StartCoroutine(TypeLine(dialogue));
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine(string[] dialogue)
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach (char letter in dialogue[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if(dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine(currentDialogue);
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
        if (InventoryController.inventoryIsOpen == false) {
            PauseController.SetPause(dialoguePanel.activeSelf);
        }
        
        InventoryController.playerIsInteracting = false;
    }
}
