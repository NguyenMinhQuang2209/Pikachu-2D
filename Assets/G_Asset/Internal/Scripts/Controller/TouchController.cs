using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public static TouchController instance;
    private Slot currentSlot = null;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public void Touch(Slot newSlot)
    {
        if (currentSlot != null)
        {
            if (currentSlot == newSlot)
            {
                currentSlot.OnChoose(false);
                currentSlot = null;
            }
            else
            {
                bool isEqual = currentSlot.IsEqual(newSlot);
                if (!isEqual)
                {
                    currentSlot.OnChoose(false);
                    currentSlot = null;
                }
                else
                {
                    bool hasPath = currentSlot.StartCheck(newSlot.GetPosition());
                    if (hasPath)
                    {
                        currentSlot.TurnOff();
                        newSlot.TurnOff();
                    }
                    else
                    {
                        currentSlot.OnChoose(false);
                    }
                    currentSlot = null;
                }
            }
        }
        else
        {
            currentSlot = newSlot;
            currentSlot.OnChoose(true);
        }
    }
}
