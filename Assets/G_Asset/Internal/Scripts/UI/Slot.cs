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
        position = SlotPosition.Other;
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
    public int GetPosition()
    {
        return pos;
    }
    public bool StartCheck(int target)
    {
        return Check(0, pos, target, false);
    }
    public bool Check(int total, int parent, int target, bool isNotRoot = true)
    {
        if (pos == target)
        {
            return true;
        }
        if (total > 3)
        {
            return false;
        }
        if (isNotRoot)
        {
            if (!IsTurnOff())
            {
                return false;
            }
        }
        int width = SlotController.instance.GetTotalWidthSlot();
        int left = pos - 1;
        int right = pos + 1;
        int top = pos - width;
        int bottom = pos + width;
        switch (position)
        {
            case SlotPosition.Left:
                left = -1;
                break;
            case SlotPosition.Right:
                right = -1;
                break;
            case SlotPosition.Top:
                top = -1;
                break;
            case SlotPosition.Bottom:
                bottom = -1;
                break;
            case SlotPosition.Top_Left:
                left = -1;
                top = -1;
                break;
            case SlotPosition.Top_Right:
                right = -1;
                top = -1;
                break;
            case SlotPosition.Bottom_Left:
                bottom = -1;
                left = -1;
                break;
            case SlotPosition.Bottom_Right:
                bottom = -1;
                right = -1;
                break;
        }
        ParentPosition parentPosition = ParentPosition.Other;
        if (isNotRoot)
        {
            if (parent == pos - 1)
            {
                parentPosition = ParentPosition.Left;
            }
            else if (parent == pos + 1)
            {
                parentPosition = ParentPosition.Right;
            }
            else if (parent == pos - width)
            {
                parentPosition = ParentPosition.Top;
            }
            else if (parent == pos + width)
            {
                parentPosition = ParentPosition.Bottom;
            }
        }

        bool result;
        if (left != -1 && parentPosition != ParentPosition.Left)
        {
            Slot checkSlot = SlotController.instance.GetSlot(left);
            int nextTotal = (parentPosition != ParentPosition.Top && parentPosition != ParentPosition.Bottom) ? total : total + 1;
            result = checkSlot.Check(nextTotal, pos, target);
            if (result)
            {
                return true;
            }
        }
        if (right != -1 && parentPosition != ParentPosition.Right)
        {
            Slot checkSlot = SlotController.instance.GetSlot(right);
            int nextTotal = (parentPosition != ParentPosition.Top && parentPosition != ParentPosition.Bottom) ? total : total + 1;
            result = checkSlot.Check(nextTotal, pos, target);
            if (result)
            {
                return true;
            }
        }
        if (top != -1 && parentPosition != ParentPosition.Top)
        {
            Slot checkSlot = SlotController.instance.GetSlot(top);
            int nextTotal = (parentPosition != ParentPosition.Right && parentPosition != ParentPosition.Left) ? total : total + 1;
            result = checkSlot.Check(nextTotal, pos, target);
            if (result)
            {
                return true;
            }
        }
        if (bottom != -1 && parentPosition != ParentPosition.Bottom)
        {
            Slot checkSlot = SlotController.instance.GetSlot(bottom);
            int nextTotal = (parentPosition != ParentPosition.Right && parentPosition != ParentPosition.Left) ? total : total + 1;
            result = checkSlot.Check(nextTotal, pos, target);
            if (result)
            {
                return true;
            }
        }

        return false;
    }
}
