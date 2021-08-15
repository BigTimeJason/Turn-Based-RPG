using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public Sprite characterImage;
    public bool isLeftSide;
    public string characterName;
    public string dialogue;
    public AudioClip clip;
}

[System.Serializable] 
public class LevelDialogue
{
    public DialogueLine[] levelDialogue;
}