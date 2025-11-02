using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Dose.Library.DataStructures
{
    public class ActorRegistry : IEnumerable<ActorRegistryRecord>
    {
        private readonly List<ActorRegistryRecord> _actors = [];

        public ActorRegistryRecord? Get(object actor)
        {
            return _actors.Where(a => a.ActorType.IsInstanceOfType(actor)).FirstOrDefault();
        }

        public void Register<TActor>(string ActorName)
        {
            var actorType = typeof(TActor);

            if (_actors.Any(a => a.ActorType == actorType))
            {
                GD.PushWarning("Trying to register an already registered actor.");
            }

            _actors.Add(new ActorRegistryRecord(ActorName, actorType));
        }

        public IEnumerator<ActorRegistryRecord> GetEnumerator() => _actors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}