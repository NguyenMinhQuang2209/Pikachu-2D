using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public static TouchController instance;
    private Slot currentSlot = null;
    [SerializeField] private float fadeTimer = 1f;
    [SerializeField] private List<DirectionItem> dir = new();

    private Dictionary<SlotDirection, Sprite> storeDir = new();
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void Start()
    {
        for (int i = 0; i < dir.Count; i++)
        {
            DirectionItem currentDir = dir[i];
            storeDir[currentDir.direction] = currentDir.sprite;
        }
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
                    SoundController.instance.PlayWrongSound();
                    currentSlot.OnChoose(false);
                    currentSlot = null;
                }
                else
                {
                    bool hasPath = currentSlot.StartCheck(newSlot.GetPosition());
                    if (hasPath)
                    {

                        SoundController.instance.PlayWinSound();
                        List<SlotLine> slots = currentSlot.GetLine();
                        foreach (SlotLine slot in slots)
                        {
                            Slot currentSlot = slot.currentSlot;
                            currentSlot.ChangeSprite(GetSprite(slot.direction), fadeTimer);
                        }
                        currentSlot.TurnOff();
                        newSlot.TurnOff();

                        SlotController.instance.GetRight(currentSlot, newSlot);
                    }
                    else
                    {
                        SoundController.instance.PlayWrongSound();
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
    public Sprite GetSprite(SlotDirection direction)
    {
        return storeDir[direction];
    }
}
[System.Serializable]
public class DirectionItem
{
    public Sprite sprite;
    public SlotDirection direction;
}