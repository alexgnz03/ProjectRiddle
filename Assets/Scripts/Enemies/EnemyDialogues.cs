using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyDialogue", menuName = "Enemy Dialogue")]
public class EnemyDialogues : ScriptableObject
{
    public string enemyName;
    public Sprite enemyPortrait;
    public List<EnemyDialoguePhase> phases = new List<EnemyDialoguePhase>();
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;
    public AudioClip voiceSound;
    public float voicePitch = 1f;
}