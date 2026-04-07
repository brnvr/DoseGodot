using System.Collections.Generic;

namespace Dose.Core.Library.Common
{
    public abstract class ActionPerformer<TArgs>() : IActionPerformer
    {
        public abstract void Perform(object sender, TArgs args);
        public abstract TArgs BuildArgs(Dictionary<string, object?> argsDict);

        public void Perform(object sender, Dictionary<string, object?> argsDict)
        {
            Perform(sender, BuildArgs(argsDict));
        }
    }
}