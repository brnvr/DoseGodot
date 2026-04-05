using System.Collections.Generic;

namespace Dose.Core.Library.Common
{
    public abstract class ActionPerformer<TArgs>() : IActionPerformer
    {
        public abstract void Perform(TArgs args);
        public abstract TArgs BuildArgs(Dictionary<string, object?> argsDict);

        public void Perform(Dictionary<string, object?> argsDict)
        {
            Perform(BuildArgs(argsDict));
        }
    }
}