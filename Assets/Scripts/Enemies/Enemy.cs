using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public static Enemy Instance { get; private set; }

    public EnemyDialogues dialogueData;
    public GameObject correctObject;

    private DialogueController dialogueUI;
    private int dialogueIndex;
    public int currentPhaseIndex = 0;
    private bool isTyping, isDialogueActive;

    private EnemyDialoguePhase CurrentPhase => dialogueData.phases[currentPhaseIndex];

    private string[] currentDialogue;

    private float interactionCooldown = 0.3f;
    private float lastInteractionTime = -10f;

    private bool dialogueEnabled = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        dialogueUI = DialogueController.Instance;
        dialogueUI.ClearChoices();

        currentDialogue = CurrentPhase.dialogueLines;

        if (dialogueUI == null)
        {
            Debug.LogError("DialogueController.Instance es null. Asegúrate de que exista en la escena.");
            return;
        }

        if (dialogueData == null || dialogueData.phases.Count == 0)
        {
            Debug.LogWarning("No hay diálogo asignado al enemigo.");
            return;
        }


        Interact();
    }

    public bool CanInteract() => !isDialogueActive;

    public void Interact(bool overrideDialogue = false)
    {
        if (dialogueData == null || dialogueData.phases.Count == 0)
            return;

        if (isDialogueActive && !overrideDialogue)
        {
            NextLine(currentDialogue);
        }
        else
        {
            StartDialogue();
        }
    }

    public void ShowObject(GameObject showedObject)
    {
        if (showedObject != null)
        {
            isDialogueActive = false;

            if (showedObject.GetComponent<Item>().itemData.itemName == dialogueData.phases[currentPhaseIndex].correctObject.GetComponent<Item>().itemData.itemName)
            {
                currentDialogue = CurrentPhase.correctObjectLines;
                FightStateController.Instance.AddProgress(10);
            }
            else
            {
                currentDialogue = CurrentPhase.wrongObjectLines;
                FightStateController.Instance.AddProgress(-10);
            }

            lastInteractionTime = Time.time;
            Interact(true);
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        Debug.Log("Arranca el diálogo");

        dialogueUI.SetNPCInfo(dialogueData.enemyName, dialogueData.enemyPortrait);
        dialogueUI.ShowDialogueUI(true);

        PauseController.SetPause(true);
        DisplayCurrentLine(currentDialogue);
        InventoryController.playerIsInteracting = true;
    }

    void NextLine(string[] dialogue)
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogue[dialogueIndex]);
            isTyping = false;
        }

        dialogueUI.ClearChoices();

        if (CurrentPhase.endDialogueLines.Length > dialogueIndex &&
            CurrentPhase.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        if (++dialogueIndex < dialogue.Length)
        {
            DisplayCurrentLine(currentDialogue);
        }
        else
        {
            //TODO este if cambiarlo, ya que si no cuando esté diciendo las correct o wrong lines,
            //si le presentas un objeto en ese momento puedes subir la barra infinitamente sin cambiar de fase
            if (currentDialogue == CurrentPhase.correctObjectLines)
            {
                AdvanceToNextPhase();
            }
            else
            {
                EndDialogue();
            }

            dialogueIndex = 0;
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

        if (CurrentPhase.autoProgressLines.Length > dialogueIndex &&
            CurrentPhase.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine(currentDialogue);
        }
    }

    void DisplayCurrentLine(string[] dialogue)
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine(dialogue));
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        if (!InventoryController.inventoryIsOpen)
            PauseController.SetPause(false);

        InventoryController.playerIsInteracting = false;
        if(currentDialogue == CurrentPhase.correctObjectLines && currentPhaseIndex == dialogueData.phases.Count - 1)
        {
            SceneManager.LoadScene("EndScreen");
        }
        currentDialogue = CurrentPhase.dialogueLines;
    }

    public void AdvanceToNextPhase()
    {
        
        if (currentPhaseIndex + 1 < dialogueData.phases.Count)
        {
            currentPhaseIndex++;
            dialogueIndex = 0;
            currentDialogue = CurrentPhase.dialogueLines;
        }
        else
        {
            Debug.Log("No hay más fases.");
        }

        RefreshDialogue();
    }

    public void RefreshDialogue()
    {
        EndDialogue();
        StartDialogue();
    }

    //TODO Este Update pasarlo al sistema de inputs nuevo
    private void Update()
    {
        if (!dialogueEnabled) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Time.time - lastInteractionTime > interactionCooldown)
            {
                Interact();
            }
        }
    }

    public void DialogueEnabled(bool isEnabled)
    {
        dialogueEnabled = isEnabled;

        if (!isEnabled && isDialogueActive)
        {
            EndDialogue();
        }

    }
}