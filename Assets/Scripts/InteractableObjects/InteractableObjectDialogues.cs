using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectDialogue", menuName = "Object Dialogue")]
public class InteractableObjectDialogues : ScriptableObject
{
    public string npcName = "";
    public Sprite npcPortrait;
    public string[] dialogueLines;
    public string[] correctObjectLines;
    public string[] wrongObjectLines;
    public bool[] autoProgressLines;
    public bool[] endDialogueLines;
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;
    public AudioClip voiceSound;

    public float voicePitch = 1f;

    public DialogueChoice[] choices;

    public int questIntProgressIndex;
    public int questCompletedIndex;
    public Quest quest;
}
