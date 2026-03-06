using System;
using System.Collections;

namespace Dose.Core.Library.Quests
{
    public abstract class Quest {}

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
        protected abstract void OnComplete(TArgs args);

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