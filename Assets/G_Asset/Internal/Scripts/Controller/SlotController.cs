using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{
    public static SlotController instance;
    public Slot slot;
    public GridLayoutGroup group;
    public Transform container;
    private Dictionary<int, Slot> slots = new();

    public int maxWithSlot = 16;
    public int maxHeightSlot = 11;

    int currentWithSlot = 0;
    int currentHeightSlot = 0;
    private List<Item> items = new();
    public List<Sprite> sprites = new();

    private List<int> positions = new();
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
        InitItem();
        SpawnSlotItem(16, 11);
        currentWithSlot = maxWithSlot;
        currentHeightSlot = maxHeightSlot;
    }
    public void InitItem()
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            Item newItem = new(sprites[i], i.ToString());
            items.Add(newItem);
        }
    }
    public void SpawnSlotItem(int w, int h)
    {
        positions?.Clear();
        group.constraintCount = w + 2;
        for (int i = 0; i < h + 2; i++)
        {
            for (int j = 0; j < w + 2; j++)
            {
                int pos = i * (w + 2) + j;
                Slot currentSlot = Instantiate(slot, container.transform);
                currentSlot.SlotInit(pos);
                slots[pos] = currentSlot;
                if (i != 0 && i != h + 1 && j != 0 && j != w + 1)
                {
                    positions.Add(pos);
                }
                else
                {
                    SlotPosition slotPosition = SlotPosition.Top;
                    if (i == 0)
                    {
                        slotPosition = SlotPosition.Top;
                        if (j == 0)
                        {
                            slotPosition = SlotPosition.Top_Left;
                        }
                        else if (j == w + 1)
                        {
                            slotPosition = SlotPosition.Top_Right;
                        }
                    }
                    else if (i == h + 1)
                    {
                        slotPosition = SlotPosition.Bottom;
                        if (j == 0)
                        {
                            slotPosition = SlotPosition.Bottom_Left;
                        }
                        else if (j == w + 1)
                        {
                            slotPosition = SlotPosition.Bottom_Right;
                        }
                    }
                    else
                    {
                        if (j == 0)
                        {
                            slotPosition = SlotPosition.Left;
                        }
                        else if (j == w + 1)
                        {
                            slotPosition = SlotPosition.Right;
                        }
                    }
                    currentSlot.TurnOff(slotPosition);
                }
            }
        }

        SpawnItem();
    }
    public void SpawnItem()
    {
        List<int> notUse = new(positions);
        int currentIndex = 0;
        while (notUse.Count > 0)
        {
            int pos = notUse[0];
            Item currentItem = items[currentIndex];
            Slot currentSlot = slots[pos];
            currentSlot.ChangeSlotItem(currentItem);
            notUse.Remove(pos);
            int sidePos = Random.Range(0, notUse.Count);
            int nextPos = notUse[sidePos];
            Slot sideSlot = slots[nextPos];
            sideSlot.ChangeSlotItem(currentItem);
            notUse.Remove(nextPos);
            currentIndex = currentIndex == items.Count - 1 ? 0 : currentIndex + 1;
        }
    }

}
[System.Serializable]
public class Item
{
    public Sprite sprite;
    public string itemName = "";
    public Item(Sprite sprite, string name)
    {
        this.sprite = sprite;
        itemName = name;
    }
}