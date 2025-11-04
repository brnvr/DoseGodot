// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using Dose.Core.Library.Actors;

// namespace Dose.Core.Library.DataStructures
// {
//     public class Inventory(ActorRegistry registry) : IEnumerable
//     {
//         private readonly List<InventoryRecord> _registryRecords = [];

//         public ActorRegistry Registry { get; } = registry;

//         public void Add<T>() where T : BaseItem
//         {
//             var inventoryRecord = Find<T>();

//             inventoryRecord?.Increment();

//             if (inventoryRecord == null)
//             {
//                 var registryRecord = Registry.Where(r => r.ActorType == typeof(T)).FirstOrDefault();

//                 if (registryRecord != null)
//                 {
//                     _registryRecords.Add(new InventoryRecord(registryRecord));
//                 }
//             }
//         }

//         public void AddAndDestroy<T>(T item) where T : BaseItem
//         {
//             Add<T>();

//             item.Destroy();
//         }

//         public void Drop<T>() where T : BaseItem
//         {
//             var registryRecord = Find<T>();

//             if (registryRecord?.Decrement() == 0)
//             {
//                 _registryRecords.Remove(registryRecord);
//             }
//         }

//         public bool Exists<T>() where T : BaseItem
//         {
//             return _registryRecords.Any(item => item.RegistryRecord.ActorType == typeof(T));
//         }

//         InventoryRecord? Find<T>() where T : BaseItem
//         {
//             return _registryRecords.Find(item => item.RegistryRecord.ActorType == typeof(T));
//         }

//         IEnumerator IEnumerable.GetEnumerator()
//         {
//             return _registryRecords.Select(item => item.RegistryRecord.ActorType).GetEnumerator();
//         }

//         record InventoryRecord
//         {
//             public ActorRegistryRecord RegistryRecord { get; }
//             public int Quantity { get; private set; }

//             public InventoryRecord(ActorRegistryRecord registryRecord)
//             {
//                 if (!registryRecord.ActorType.IsSubclassOf(typeof(BaseItem)))
//                 {
//                     throw new ArgumentException($"{nameof(registryRecord)} is not a subclass of {nameof(BaseItem)}");
//                 }

//                 RegistryRecord = registryRecord;
                
//                 Increment();
//             }

//             public int Increment()
//             {
//                 return Quantity++;
//             }

//             public int Decrement()
//             {
//                 return Math.Min(0, Quantity--);
//             }
//         }
//     }
// }