using System;

namespace Dose.Library.DataStructures
{
    public record ActorRegistryRecord(string ActorName, Type ActorType)
    {
        public string ActorName { get; } = ActorName;
        public Type ActorType { get; } = ActorType;
    }
}