using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Image bgImg;
    public Image iconImg;
    public Item currentItem = null;
    public Color defaultColor = Color.white;
    public Color selectedColor = Color.grey;
    public Button touchBtn;

    private int pos = -1;

    private SlotPosition position = SlotPosition.Other;

    bool isTurnOff = false;


    private void Start()
    {
        touchBtn.onClick.AddListener(() =>
        {
            OnTouch();
        });
    }
    public void OnTouch()
    {
        TouchController.instance.Touch(this);
    }
    public void ChangeSlotItem(Item newItem)
    {
        currentItem = newItem;
        iconImg.sprite = newItem.sprite;
        OnChoose(false);
    }
    public void SlotInit(int pos)
    {
        this.pos = pos;
    }
    public void TurnOff(SlotPosition newPos)
    {
        position = newPos;
        bgImg.gameObject.SetActive(false);
        iconImg.gameObject.SetActive(false);
        isTurnOff = true;
    }
    public void TurnOff()
    {
        bgImg.gameObject.SetActive(false);
        iconImg.gameObject.SetActive(false);
        isTurnOff = true;
    }
    public bool IsTurnOff()
    {
        return isTurnOff;
    }
    public void OnChoose(bool v)
    {
        Color newColor = v ? selectedColor : defaultColor;
        bgImg.color = newColor;
    }
    public bool IsEqual(string newV)
    {
        return currentItem.itemName == newV;
    }
    public bool IsEqual(Slot checkSlot)
    {
        return IsEqual(checkSlot.currentItem.itemName);
    }

    public void Check()
    {

    }
}
