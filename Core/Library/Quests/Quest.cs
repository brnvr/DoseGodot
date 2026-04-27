using System;
using System.Collections;
using Godot;

namespace Dose.Core.Library.Quests
{
    public abstract class Quest
    {
        public abstract string Name { get; }
    }

    public abstract class Quest<TArgs> : Quest, IEquatable<Quest<TArgs>>
    {
        public abstract bool Equals(Quest<TArgs>? other);

        public bool TestCompleted(TArgs args)
        {
            if (HasCompleted(args))
            {
                OnComplete(args);
                return true;
            }

            return false;
        }

        protected abstract bool HasCompleted(TArgs args);

        protected virtual void OnComplete(TArgs args)
        {
            GD.Print($"Completed quest: {Name}");
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Quest<TArgs>);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}