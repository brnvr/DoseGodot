// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using Dose.Library.Actors;
// using Godot;

// namespace Dose.Library.DataStructures
// {
//     public class ActorRegistry : IEnumerable<ActorRegistryRecord>
//     {
//         private readonly List<ActorRegistryRecord> _actors = [];

//         public ActorRegistryRecord? Get<TActor>(TActor actor) where TActor : IActor
//         {
//             return _actors
//                 .Where(a => a.ActorType.IsInstanceOfType(actor) && a.ActorController == actor.Controller)
//                 .FirstOrDefault();
//         }

//         public void Register<TActor>(IActorController actorController)
//         {
//             var actorType = typeof(TActor);

//             if (_actors.Any(a => a.ActorType == actorType && a.ActorController == actorController))
//             {
//                 GD.PushWarning("Trying to register an already registered actor.");
//             }

//             _actors.Add(new ActorRegistryRecord(actorType, actorController));
//         }

//         public IEnumerator<ActorRegistryRecord> GetEnumerator() => _actors.GetEnumerator();
//         IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//     }
// }