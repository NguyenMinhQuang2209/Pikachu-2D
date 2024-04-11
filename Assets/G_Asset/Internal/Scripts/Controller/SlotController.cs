using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

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
    private List<int> remainPositions = new();
    private Dictionary<string, List<Slot>> dataStore = new();
    private Dictionary<string, int> dataRemainStore = new();

    private List<DataItem> dataItems = new();

    int currentLevel = 3;
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
        currentWithSlot = maxWithSlot;
        currentHeightSlot = maxHeightSlot;
        SpawnSlotItem(currentWithSlot, currentHeightSlot);
    }
    public void InitItem()
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            Item newItem = new(sprites[i], i.ToString());
            items.Add(newItem);
        }
    }
    public Slot GetSlot(int pos)
    {
        return slots[pos];
    }
    public void SpawnSlotItem(int w, int h)
    {
        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }
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
        remainPositions = new(positions);

        SpawnItem(positions);
    }
    public void SpawnItem(List<int> usePositions)
    {
        dataStore?.Clear();
        List<int> notUse = new(usePositions);
        int currentIndex = 0;
        while (notUse.Count > 0)
        {
            int posInList = Random.Range(0, notUse.Count);
            int pos = notUse[posInList];
            Item currentItem = items[currentIndex];
            Slot currentSlot = slots[pos];

            string key = currentItem.itemName;
            List<Slot> keyData = dataStore.ContainsKey(key) ? dataStore[key] : new();
            keyData.Add(currentSlot);

            currentSlot.ChangeSlotItem(currentItem);
            notUse.Remove(pos);
            int sidePos = Random.Range(0, notUse.Count);
            int nextPos = notUse[sidePos];
            Slot sideSlot = slots[nextPos];
            sideSlot.ChangeSlotItem(currentItem);
            notUse.Remove(nextPos);

            keyData.Add(sideSlot);

            dataStore[key] = new(keyData);
            currentIndex = currentIndex == items.Count - 1 ? 0 : currentIndex + 1;
        }
        LoadDataRemain();
        LoadDataItems();
    }
    public void LoadDataRemain()
    {
        dataRemainStore?.Clear();
        for (int i = 0; i < items.Count; i++)
        {
            Item currentItem = items[i];
            string key = currentItem.itemName;
            dataRemainStore[key] = dataStore[key].Count;
        }
    }
    public void LoadDataStore()
    {
        dataStore?.Clear();
        for (int i = 0; i < remainPositions.Count; i++)
        {
            int pos = remainPositions[i];
            Slot currentSlot = slots[pos];
            string key = currentSlot.GetName();
            List<Slot> keyData = dataStore.ContainsKey(key) ? dataStore[key] : new();
            keyData.Add(currentSlot);
            dataStore[key] = new(keyData);
        }
        LoadDataRemain();
        LoadDataItems();
    }
    public void LoadDataItems()
    {
        dataItems?.Clear();
        for (int i = 0; i < items.Count; i++)
        {
            Item current = items[i];
            List<Slot> slotData = dataStore[current.itemName];
            for (int currentIndex = 0; currentIndex < slotData.Count; currentIndex++)
            {
                Slot currentSlot = slotData[currentIndex];

                for (int nextIndex = currentIndex + 1; nextIndex < slotData.Count; nextIndex++)
                {
                    Slot compareSlot = slotData[nextIndex];
                    bool canConnect = currentSlot.StartCheck(compareSlot.GetPosition());
                    if (canConnect)
                    {
                        dataItems.Add(new(currentSlot, compareSlot));
                    }
                }
            }
        }
        if (dataItems.Count == 0)
        {
            Refresh();
        }
    }
    public void Refresh()
    {
        List<int> remainPositionsTemp = new(remainPositions);
        for (int i = 0; i < items.Count; i++)
        {
            Item item = items[i];
            string key = item.itemName;
            int remain = dataRemainStore.ContainsKey(key) ? dataRemainStore[key] : 0;

            int pair = remain / 2;
            while (pair > 0 && remainPositionsTemp.Count > 0)
            {
                int key1 = Random.Range(0, remainPositionsTemp.Count);
                int pos1 = remainPositionsTemp[key1];
                Slot slot1 = slots[pos1];
                slot1.ChangeSlotItem(item);
                remainPositionsTemp.Remove(pos1);

                int key2 = Random.Range(0, remainPositionsTemp.Count);
                int pos2 = remainPositionsTemp[key2];
                Slot slot2 = slots[pos2];
                slot2.ChangeSlotItem(item);
                remainPositionsTemp.Remove(pos2);
                pair--;
            }
        }
        LoadDataStore();
    }

    public void GetRight(Slot slot1, Slot slot2)
    {
        if (currentLevel == 1)
        {
            GetRightLevel1(slot1, slot2);
        }
        else if (currentLevel == 2)
        {
            GetRightLevel2(slot1, slot2);
        }
        else if (currentLevel == 3)
        {
            GetRightLevel3(slot1, slot2);
        }
    }
    public void GetRightLevel1(Slot slot1, Slot slot2)
    {
        remainPositions.Remove(slot1.GetPosition());
        remainPositions.Remove(slot2.GetPosition());

        string key1 = slot1.GetName();
        string key2 = slot2.GetName();

        if (key1 != "")
        {
            dataRemainStore[key1] -= 1;
        }
        if (key2 != "")
        {
            dataRemainStore[key2] -= 1;
        }

        for (int i = 0; i < dataItems.Count; i++)
        {
            DataItem item = dataItems[i];
            if (item.Contain(slot1, slot2))
            {
                dataItems.RemoveAt(i);
                i--;
            }
        }
        if (dataItems.Count == 0)
        {
            LoadDataItems();
        }
    }

    public void GetRightLevel2(Slot slot1, Slot slot2)
    {
        int pos1 = slot1.GetPosition();
        int pos2 = slot2.GetPosition();
        int width = GetTotalWidthSlot();
        int firstRow = width * 2;

        bool first = false;

        if (pos1 < firstRow && pos2 < firstRow)
        {
            first = true;
        }
        if (pos1 < firstRow && pos1 == pos2 - width)
        {
            first = true;
        }
        if (pos2 < firstRow && pos2 == pos1 - width)
        {
            first = true;
        }
        if (first)
        {
            GetRightLevel1(slot1, slot2);
        }
        else
        {
            bool oneColumn = pos1 == pos2 - width || pos2 == pos1 - width;

            List<Slot> effectSlot = new();
            int smaller;
            int bigger;
            if (oneColumn)
            {
                smaller = pos1 < pos2 ? pos1 : pos2;
                bigger = pos1 > pos2 ? pos1 : pos2;
            }
            else
            {
                smaller = pos1;
                bigger = pos2;
            }
            while (smaller >= width * 2)
            {
                int to = smaller;
                int from = smaller - width;
                SwitchBwtTwoSlot(to, from);
                effectSlot.Add(slots[to]);
                effectSlot.Add(slots[from]);
                smaller -= width;
            }

            while (bigger >= width * 2)
            {
                int to = bigger;
                int from = bigger - width;
                SwitchBwtTwoSlot(to, from);
                effectSlot.Add(slots[to]);
                effectSlot.Add(slots[from]);
                bigger -= width;
            }

            for (int i = 0; i < dataItems.Count; i++)
            {
                DataItem item = dataItems[i];
                if (item.Contain(effectSlot))
                {
                    dataItems.RemoveAt(i);
                    i--;
                }
            }
            if (dataItems.Count == 0)
            {
                LoadDataItems();
            }
        }
    }
    public void GetRightLevel3(Slot slot1, Slot slot2)
    {
        int pos1 = slot1.GetPosition();
        int pos2 = slot2.GetPosition();
        int width = GetTotalWidthSlot();
        int height = GetTotalHeightSlot();

        int firstRow = width * height - width * 2;

        bool first = false;

        if (pos1 > firstRow && pos2 > firstRow)
        {
            first = true;
        }
        if (pos1 > firstRow && pos1 == pos2 + width)
        {
            first = true;
        }
        if (pos2 > firstRow && pos2 == pos1 + width)
        {
            first = true;
        }
        if (first)
        {
            GetRightLevel1(slot1, slot2);
        }
        else
        {
            bool oneColumn = pos1 == pos2 - width || pos2 == pos1 - width;

            List<Slot> effectSlot = new();
            int smaller;
            int bigger;
            if (oneColumn)
            {
                bigger = pos1 < pos2 ? pos1 : pos2;
                smaller = pos1 > pos2 ? pos1 : pos2;
            }
            else
            {
                smaller = pos1;
                bigger = pos2;
            }

            int compareValue = width * height - width * 2;

            while (smaller <= compareValue)
            {
                int to = smaller;
                int from = smaller + width;
                SwitchBwtTwoSlot(to, from);
                effectSlot.Add(slots[to]);
                effectSlot.Add(slots[from]);
                smaller += width;
            }

            while (bigger <= compareValue)
            {
                int to = bigger;
                int from = bigger + width;
                SwitchBwtTwoSlot(to, from);
                effectSlot.Add(slots[to]);
                effectSlot.Add(slots[from]);
                bigger += width;
            }

            for (int i = 0; i < dataItems.Count; i++)
            {
                DataItem item = dataItems[i];
                if (item.Contain(effectSlot))
                {
                    dataItems.RemoveAt(i);
                    i--;
                }
            }
            if (dataItems.Count == 0)
            {
                LoadDataItems();
            }
        }
    }
    public void SwitchBwtTwoSlot(int to, int from)
    {
        Slot currentSlot = slots[to];
        Slot nextSlot = slots[from];

        Item nextItem = nextSlot.GetCurrentItem();
        if (nextItem != null)
        {
            currentSlot.ChangeSlotItem(nextItem);
            nextSlot.TurnOff();
        }
    }
    public void ChangeLevel(int newLevel)
    {
        currentLevel = newLevel;
        SpawnItem(positions);
    }

    public int GetTotalWidthSlot()
    {
        return currentWithSlot + 2;
    }
    public int GetTotalHeightSlot()
    {
        return currentHeightSlot + 2;
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

public class DataItem
{
    public Slot slot1;
    public Slot slot2;
    public DataItem(Slot slot1, Slot slot2)
    {
        this.slot1 = slot1;
        this.slot2 = slot2;
    }
    public bool Contain(Slot slot_1, Slot slot_2)
    {
        if (slot1 == slot_1 || slot1 == slot_2)
        {
            return true;
        }
        if (slot2 == slot_1 || slot2 == slot_2)
        {
            return true;
        }
        return false;
    }
    public bool Contain(List<Slot> list)
    {
        if (list.Contains(slot1))
        {
            return true;
        }
        if (list.Contains(slot2))
        {
            return true;
        }
        return false;
    }
}