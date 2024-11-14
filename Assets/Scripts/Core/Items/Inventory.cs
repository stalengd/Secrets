using System;
using System.Collections.Generic;
using System.Linq;
using Anomalus.Items.Owner;
using Anomalus.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Anomalus.Items
{
    public class Inventory : IReadOnlyInventory
    {
        public IEnumerable<Slot> Slots => Items;

        public UnityEvent<Slot> OnItemAdded { get; } = new UnityEvent<Slot>();
        public UnityEvent<Slot> OnItemChanged { get; } = new UnityEvent<Slot>();
        public UnityEvent<Slot> OnItemRemoved { get; } = new UnityEvent<Slot>();
        public UnityEvent OnChange { get; } = new UnityEvent();

        public int SlotsCount { get; private set; }
        public bool IsSlotsLimited => SlotsCount >= 0;

        public Predicate<ItemStack> Filter { get; set; }
        public InventoryMass Mass { get; set; }
        public string Name { get; set; }

        public bool UseSorting { get; set; }
        public bool IsDescendingSorting { get; set; }
        public Func<ItemConfig, float> SortingSelector { get; set; } = i => -i.SortingOrder;

        protected bool InsertItemsAsList { get; set; } = true;

        protected readonly List<Slot> Items = new();

        public Inventory(int slots = -1, Predicate<ItemStack> filter = null)
        {
            SlotsCount = slots;
            Filter = filter;

            OnItemAdded.AddListener(i => OnChange.Invoke());
            OnItemChanged.AddListener(i => OnChange.Invoke());
            OnItemRemoved.AddListener(i => OnChange.Invoke());
        }

        public Slot this[int index]
        {
            get => GetCell(index);
        }

        protected virtual Slot AddItem(Slot item, int? slot)
        {
            Items.Add(item);

            OnItemAdded.Invoke(item);
            OnItemChanged.Invoke(item);

            return item;
        }

        public Slot AppendItem(ItemStack stack)
            => AppendItem(stack, false);

        public virtual Slot AppendItem(ItemStack stack, bool forceNewStack)
        {
            Slot item = null;
            int addToNewStack;
            int addToOldStack = 0;

            if (forceNewStack == true)
            {
                addToNewStack = stack.Count;
            }
            else
            {
                foreach (var (existingStack, toAdd, sumAdded) in GetStacksForAdding(stack))
                {
                    item = existingStack;
                    existingStack.Count += toAdd;
                    addToOldStack = sumAdded;
                }
                addToNewStack = stack.Count - addToOldStack;
            }

            if (addToNewStack > 0)
            {
                if (IsSlotsLimited && Slots.Count() >= SlotsCount)
                {
                    //var last = Items.Last();
                    //from.PlaceItem(last.AsStack);
                }

                item = new Slot(this, stack.ItemType, addToNewStack);
                var result = AddItem(item, null);
                if (result == null) return null;
                OnItemChanged.Invoke(item);
            }

            return item;
        }

        public virtual Slot InsertItemToSlotAuto(ItemStack stack, int slot, FullItemAddress from)
        {
            (var insertedItem, var removedItem) = InsertItemToSlot(stack, slot);

            if (removedItem.HasValue)
            {
                from.PlaceItem(removedItem.Value);
            }

            return insertedItem;
        }

        public virtual (Slot insertedItem, ItemStack? removedItem) InsertItemToSlot(ItemStack stack, int slot)
        {
            if (slot > Items.Count)
            {
                return (AppendItem(stack, true), null);
            }

            Slot item;
            var currentItem = Items[slot];

            if (currentItem == null)
            {
                item = new Slot(this, stack.ItemType, stack.Count);
                AddItem(item, slot);
                return (item, null);
            }

            if (currentItem.Item == stack.ItemType)
            {
                if (currentItem.Item.MaxStack < 0 || currentItem.Count + stack.Count <= currentItem.Item.MaxStack)
                {
                    currentItem.Count += stack.Count;
                    return (currentItem, null);
                }
                else
                {
                    var canAddCount = currentItem.Item.MaxStack - currentItem.Count;
                    currentItem.Count += canAddCount;
                    stack = new(stack.ItemType, stack.Count - canAddCount, stack.Attributes);
                    return (currentItem, stack);
                }
            }

            if (InsertItemsAsList)
            {
                return (AppendItem(stack, true), null);
            }
            else
            {
                var removedItem = currentItem.AsStack;
                item = new Slot(this, stack.ItemType, stack.Count);
                AddItem(item, slot);
                return (item, removedItem);
            }
        }

        public virtual void RemoveItem(Slot item)
        {
            Items.Remove(item);

            if (item.Inventory == this)
                item.IsRemoved = true;
            OnItemRemoved.Invoke(item);
        }

        public void RemoveItem(ItemStack stack)
        {
            var countToRemove = stack.Count;

            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i];

                if (item != null && stack.ItemType == item.Item)
                {
                    if (item.Count >= countToRemove)
                    {
                        item.Count -= countToRemove;
                        return;
                    }

                    countToRemove -= item.Count;
                    item.Count = 0;
                    i--;
                }
            }

            if (countToRemove > 0)
                Debug.LogError($"Removing {stack.Count} {stack.ItemType}, while in inventory no such count of items");
        }

        public bool HasItem(ItemStack stack)
        {
            var countToRemove = stack.Count;

            foreach (var item in Items)
            {
                if (item != null && stack.ItemType == item.Item)
                {
                    if (item.Count >= countToRemove)
                    {
                        return true;
                    }

                    countToRemove -= item.Count;
                }
            }

            return false;
        }

        public int GetItemCount(ItemConfig itemType)
        {
            var count = 0;
            foreach (var item in Items)
            {
                if (item != null && itemType == item.Item)
                {
                    count += item.Count;
                }
            }
            return count;
        }

        public Slot GetCell(int index)
        {
            if (index < 0 || index >= Items.Count)
                return null;

            return Items[index];
        }

        public int GetCellIndex(Slot cell)
        {
            return Items.IndexOf(cell);
        }

        public bool HasEmptySlot()
        {
            return Items.Any(item => item == null);
        }

        public Slot GetStackWithItem(ItemConfig itemData, int skip = 0)
        {
            var index = GetStackIndexWithItem(itemData, skip);
            if (index < 0) return null;
            return GetCell(index);
        }

        public int GetStackIndexWithItem(ItemConfig itemData, int skip = 0)
        {
            var index = Items.Skip(skip).IndexOf(i => i != null && i.Item == itemData);
            if (index < 0) return -1;
            return index + skip;
        }

        public (ItemStack canAdd, ItemStack left) SplitStackToCanAdd(ItemStack stack, int? slot, Inventory from = null)
        {
            var canAddCount = CanAddItem(stack, slot, from);
            return (new ItemStack(stack.ItemType, canAddCount, stack.Attributes), new ItemStack(stack.ItemType, stack.Count - canAddCount, stack.Attributes));
        }

        public void SplitStackToCanAdd(ItemStack stack, Action<ItemStack> canAddAction, Action<ItemStack> leftAction)
        {
            var (canAdd, left) = SplitStackToCanAdd(stack, null);
            if (canAdd.Count > 0)
                canAddAction(canAdd);
            if (left.Count > 0)
                leftAction(left);
        }

        public virtual int CanAddItem(ItemStack stack, int? slot, Inventory from = null)
        {
            if (!(Filter == null || Filter(stack) == true))
                return 0;

            if (Mass == null || (from != null && from.Mass == Mass))
            {
                return stack.Count;
            }
            else
            {
                var freeMass = Mass.MaxMass - Mass.Mass;
                var count = Mathf.FloorToInt(freeMass / stack.ItemType.Mass);

                return Mathf.Min(count, stack.Count);
            }
        }

        public virtual bool HandleMoving(Slot item, FullItemAddress from, int? count, int? slot)
        {
            if (from.Inventory == this)
            {
                if (slot.HasValue && slot.Value == from.Slot)
                {
                    return false;
                }
            }

            var stack = new ItemStack(item.Item, count ?? item.Count, item.Attributes);

            stack = new ItemStack(item.Item, CanAddItem(stack, slot, from.Inventory), item.Attributes);
            if (stack.Count == 0)
                return false;

            var newCount = item.Count - stack.Count;
            var success = true;
            item.IsRemoved = true;
            ItemStack? itemForReplace = null;

            if (slot.HasValue)
                (_, itemForReplace) = InsertItemToSlot(stack, slot.Value);
            else
                success = AppendItem(stack, false) != null;

            if (success)
            {
                if (newCount <= 0)
                    from.Inventory.RemoveItem(item);
                else
                {
                    item.IsRemoved = false;
                    item.Count = newCount;
                }
            }
            else
                item.IsRemoved = false;

            if (itemForReplace.HasValue)
                from.PlaceItem(itemForReplace.Value);

            return success;
        }

        //public JObject ToJson()
        //{
        //    return new JObject()
        //    {
        //        ["items"] = new JArray(_items.Select(i => i?.AsStack.ToJson()))
        //    };
        //}

        //public void LoadFromJson(JToken json, ItemsRepository repository)
        //{
        //    _items = json["items"].Select(t =>
        //    {
        //        if (t == null || t.Value<JObject>() == null) return null;
        //        var b = ItemStack.FromJson(t, repository);
        //        return new StoredItem(this, b.ItemType, b.Count);
        //    }).ToList();
        //}

        protected IEnumerable<(Slot item, int toAdd, int sumAdded)> GetStacksForAdding(ItemStack stack)
        {
            Slot existingStack;
            var existingStackIndex = -1;
            var addedCount = 0;
            do
            {
                existingStackIndex = GetStackIndexWithItem(stack.ItemType, existingStackIndex + 1);
                existingStack = GetCell(existingStackIndex);
                if (existingStack != null && existingStack.IsRemoved) existingStack = null;
                if (existingStack != null)
                {
                    var canBeAdded = (existingStack.Item.MaxStack > 0 ? existingStack.Item.MaxStack : int.MaxValue) - existingStack.Count;
                    var toAdd = Mathf.Min(stack.Count - addedCount, canBeAdded);
                    addedCount += toAdd;
                    yield return (existingStack, toAdd, addedCount);
                }
            } while (existingStack != null && addedCount < stack.Count);
        }


        public class Slot
        {
            public Inventory Inventory { get; private set; }
            public ItemConfig Item { get; private set; }
            public int Count
            {
                get => _count;
                set
                {
                    _count = value;
                    if (_count <= 0)
                    {
                        Inventory.RemoveItem(this);
                        return;
                    }
                    Inventory.OnItemChanged.Invoke(this);
                }
            }
            public ItemAttributes Attributes { get; set; }

            public ItemStack AsStack => new(Item, Count, Attributes);

            public bool IsRemoved { get; set; } = false;
            public bool IsEmpty
                => Count <= 0 || Item == null;

            private int _count;


            public Slot(Inventory inventory, ItemConfig item, int count)
            {
                Inventory = inventory;

                Item = item;
                _count = count;
            }

            public void SetState(ItemConfig item, int count, ItemAttributes attributes)
            {
                Item = item;
                Count = count;
                Attributes = attributes;
            }

            public void SetEmpty()
            {
                SetState(null, 0, default);
            }

            public bool MoveTo(Inventory inventory, int? slot, int? count = null)
            {
                return inventory.HandleMoving(this, new FullItemAddress(Inventory, Inventory.GetCellIndex(this)), count, slot);
            }
        }
    }
}
