namespace Dose.Core.Library.Quests
{
    public abstract class Quest {}

    public abstract class Quest<TArgs> : Quest
    {
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
    }
}