using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI dialogue;
    public TextMeshProUGUI characterName;
    public GameObject blockClicks;
    public Image characterSpriteOne;
    public Image characterSpriteTwo;
    public LevelDialogue[] dialogueLines;
    public float textSpeed;

    public Vector3 spriteOneStartPos;
    public Vector3 spriteTwoStartPos;
    public Vector3 startPos;

    private int index;
    private bool hasInitiated;

    private void Start()
    {
        hasInitiated = false;
        dialogue.text = string.Empty;
        characterName.text = string.Empty;

        startPos = transform.position;
        spriteOneStartPos = characterSpriteOne.gameObject.transform.position;
        spriteTwoStartPos = characterSpriteTwo.gameObject.transform.position;
        characterSpriteTwo.transform.localScale = new Vector3(-1, 1);

        if (dialogueLines[GameManager.Instance.level].levelDialogue?.Length > 0)
        {
            StartDialogue();
        } else
        {
            Close();
        }
    }

    private void Update()
    {
        if (dialogueLines[GameManager.Instance.level].levelDialogue?.Length > 0 && Input.GetMouseButtonDown(0))
        {
            if (dialogue.text == dialogueLines[GameManager.Instance.level].levelDialogue[index].dialogue)
            {
                NextLine();
            } else if (hasInitiated == true)
            {
                StopAllCoroutines();
                dialogue.text = dialogueLines[GameManager.Instance.level].levelDialogue[index].dialogue;
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        blockClicks.SetActive(true);

        transform.position = startPos;
        characterSpriteOne.gameObject.transform.position = spriteOneStartPos;
        characterSpriteTwo.gameObject.transform.position = spriteTwoStartPos;

        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        characterSpriteOne.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        characterSpriteTwo.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        transform.localScale = new Vector3(0, 1);
        transform.DOScaleX(1, 2f).OnComplete(() =>
        {
            ShowCharacter(dialogueLines[GameManager.Instance.level].levelDialogue[index].isLeftSide);
            hasInitiated = true;
            StartCoroutine(TypeLine());
        });
    }

    IEnumerator TypeLine()
    {
        characterName.text = dialogueLines[GameManager.Instance.level].levelDialogue[index].characterName;
        if(dialogueLines[GameManager.Instance.level].levelDialogue[index].clip != null) SoundManager.Instance.Play(dialogueLines[GameManager.Instance.level].levelDialogue[index].clip);
        foreach(char c in dialogueLines[GameManager.Instance.level].levelDialogue[index].dialogue.ToCharArray())
        {
            dialogue.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void ShowCharacter(bool isCharacterOne)
    {
        if (isCharacterOne)
        {
            characterSpriteOne.gameObject.SetActive(true);
            characterSpriteOne.color = new Color(1, 1, 1, 0f);
            characterSpriteOne.DOColor(new Color(1, 1, 1, 1f), 0.2f);
            characterSpriteOne.gameObject.transform.position = spriteOneStartPos - new Vector3(0, 50);
            characterSpriteOne.gameObject.transform.DOMove(spriteOneStartPos, 0.2f);
            characterSpriteOne.sprite = dialogueLines[GameManager.Instance.level].levelDialogue[index].characterImage;
            characterSpriteOne.SetNativeSize();

            characterSpriteTwo.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        } else
        {
            characterSpriteTwo.gameObject.SetActive(true);
            characterSpriteTwo.color = new Color(1, 1, 1, 0f);
            characterSpriteTwo.DOColor(new Color(1, 1, 1, 1f), 0.2f);
            characterSpriteTwo.gameObject.transform.position = spriteTwoStartPos - new Vector3(0, 50);
            characterSpriteTwo.gameObject.transform.DOMove(spriteTwoStartPos, 0.2f);
            characterSpriteTwo.sprite = dialogueLines[GameManager.Instance.level].levelDialogue[index].characterImage;
            characterSpriteTwo.SetNativeSize();

            characterSpriteOne.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        }

    }

    void NextLine()
    {
        if (dialogueLines[GameManager.Instance.level].levelDialogue.Length - 1 != index)
        {
            dialogue.text = string.Empty;
            characterName.text = string.Empty;
        }

        if (index < dialogueLines[GameManager.Instance.level].levelDialogue.Length - 1)
        {
            index++;
            dialogue.text = string.Empty;
            ShowCharacter(dialogueLines[GameManager.Instance.level].levelDialogue[index].isLeftSide);
            StartCoroutine(TypeLine());
        } else
        {
            Close();
        }
    }

    void Close()
    {
        gameObject.transform.DOMoveY(-200, 1f);
        gameObject.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 0.5f).OnComplete(() =>
        {
            characterSpriteOne.gameObject.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 0.5f);
            characterSpriteOne.gameObject.transform.DOMoveX(-200, 1f).OnComplete(() =>
            {
                characterSpriteTwo.gameObject.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 0.5f);
                characterSpriteTwo.gameObject.transform.DOMoveX(-200, 1f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    characterSpriteOne.gameObject.SetActive(false);
                    characterSpriteTwo.gameObject.SetActive(false);
                    blockClicks.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.4f);
                    blockClicks.gameObject.GetComponent<Image>().DOColor(new Color(0, 0, 0, 0f), 1f).OnComplete(() =>
                    {
                        blockClicks.SetActive(false);
                    });
                });
            });
        });
    }
}
