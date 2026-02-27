using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Events;

namespace Dose.Core.Library.DataStructures
{
    [JsonConverter(typeof(InventoryJsonConverter))]
    public class Inventory : IEnumerable<ItemContext>
    {
        private readonly List<InventoryRecord> _records = [];

        public EventHandler<InventoryItemAddedEventArgs>? ItemAdded { get; set; }
        public EventHandler<InventoryItemDroppedEventArgs>? ItemDropped { get; set; }
        
        public void Add(IItem item)
        {
            if (item.Context is null) return;
    
            Add(item.Context);
        }

        public void Add(ItemContext itemContext)
        {
            var inventoryRecord = Find(itemContext);

            inventoryRecord?.Increment();

            if (inventoryRecord == null)
            {
                _records.Add(new InventoryRecord(itemContext));
            }

            ItemAdded?.Invoke(this, new InventoryItemAddedEventArgs(itemContext));
        }

        public void AddAndDestroy(IItem item)
        {
            Add(item);

            item.Destroy();
        }

        public void Drop(ItemContext itemContext)
        {
            var record = Find(itemContext);

            if (record?.Decrement() == 0)
            {
                _records.Remove(record);
            }

            ItemDropped?.Invoke(this, new InventoryItemDroppedEventArgs(itemContext));
        }

        public bool Exists(ItemContext itemContext)
        {
            return _records.Any(record => record.ItemContext == itemContext);
        }

        public IEnumerator<ItemContext> GetEnumerator()
        {
            return _records.Select(item => item.ItemContext).GetEnumerator();
        }

        public ItemContext this[int index]
        {
            get => _records[index].ItemContext;
        }

        InventoryRecord? Find(ItemContext itemContext)
        {
            return _records.Find(record => record.ItemContext == itemContext);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        record InventoryRecord
        {
            public ItemContext ItemContext { get; }
            public int Quantity { get; private set; }

            public InventoryRecord(ItemContext itemContext)
            {
                ItemContext = itemContext;

                Increment();
            }

            public int Increment()
            {
                return Quantity++;
            }

            public int Decrement()
            {
                return Math.Min(0, Quantity--);
            }
        }
    }

    file sealed class InventoryJsonConverter : JsonConverter<Inventory>
    {
        public override Inventory Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            throw new NotSupportedException("Deserializing Inventory is not supported.");
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, Inventory value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            var recordsField = typeof(Inventory).GetField("_records", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var records = (IEnumerable)recordsField.GetValue(value)!;

            foreach (var record in records)
            {
                var recordType = record.GetType();
                var itemContextProperty = recordType.GetProperty("ItemContext")!;
                var quantityProperty = recordType.GetProperty("Quantity")!;

                var itemContext = itemContextProperty.GetValue(record);
                var quantity = (int)quantityProperty.GetValue(record)!;
                var displayName = itemContext?.GetType().GetProperty("DisplayName")?.GetValue(itemContext) as string;

                writer.WriteStartObject();
                writer.WriteString("name", displayName);
                writer.WriteNumber("quantity", quantity);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}