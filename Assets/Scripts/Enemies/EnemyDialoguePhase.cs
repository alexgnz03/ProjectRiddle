using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyDialoguePhase
{
    public string phaseName;
    public string[] dialogueLines;
    public string[] correctObjectLines;
    public string[] wrongObjectLines;
    public bool[] autoProgressLines;
    public bool[] endDialogueLines;
    public GameObject correctObject;
}
