using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class CharacterInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image characterImage;
    public TextMeshProUGUI charName, shields, health, power, speed;

    public int charIndex;
    private Character currCharacter;
    public Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
        currCharacter = GameManager.Instance.heroes[charIndex];
        UpdateInfo();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.DOMoveY(450, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.DOMoveY(startPos.y, 1f);
    }

    public void UpdateInfo()
    {
        characterImage.sprite = currCharacter.characterArt;
        characterImage.SetNativeSize();
        charName.text = currCharacter.charName;
        shields.text = "Shield: " + currCharacter.CurrShieldHP + "/" + currCharacter.MaxShieldHP + "(" + currCharacter.ShieldElement + ")";
        health.text = "Health: " + currCharacter.CurrHP + "/" + currCharacter.MaxHP;
        power.text = "Power: " + currCharacter.CurrPower + "";
        speed.text = "Speed: " + currCharacter.CurrSpeed + "";
    }

    public void ChangeCharacter(bool increased)
    {
        if (increased)
        {
            if(charIndex + 1 >= GameManager.Instance.heroes.Count)
            {
                charIndex = 0;
            } else
            {
                charIndex += 1;
            }
        } else
        {
            if (charIndex - 1 < 0)
            {
                charIndex = GameManager.Instance.heroes.Count - 1;
            }
            else
            {
                charIndex -= 1;
            }
        }
        currCharacter = GameManager.Instance.heroes[charIndex];
        UpdateInfo();
    }
}
