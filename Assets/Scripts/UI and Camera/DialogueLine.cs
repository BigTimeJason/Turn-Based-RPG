using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine
{
    public Sprite characterImage;
    public bool isLeftSide;
    public string characterName;
    public string dialogue;
    public AudioClip clip;
}
