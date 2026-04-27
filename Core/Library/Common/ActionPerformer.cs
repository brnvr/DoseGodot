using System.Collections.Generic;
using Godot;

namespace Dose.Core.Library.Common
{
    public abstract class ActionPerformer<TSender, TArgs>() : IActionPerformer
    {
        public abstract bool Perform(TSender sender, TArgs args);
        public abstract TArgs BuildArgs(Dictionary<string, object?> argsDict);

        public bool Perform(object sender, Dictionary<string, object?> argsDict)
        {
            if (sender is not TSender senderTyped)
            {
                GD.PushWarning($"{nameof(sender)} is not {nameof(TSender)}.");
                return true;
            }

            return Perform(senderTyped, BuildArgs(argsDict));
        }
    }
}