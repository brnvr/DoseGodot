using System.Collections.Generic;
using Godot;

namespace Dose.Core.Library.Common
{
    public abstract class ActionPerformer<TSender, TArgs>() : IActionPerformer
    {
        public abstract void Perform(TSender sender, TArgs args);
        public abstract TArgs BuildArgs(Dictionary<string, object?> argsDict);

        public void Perform(object sender, Dictionary<string, object?> argsDict)
        {
            if (sender is not TSender senderTyped)
            {
                GD.PushWarning($"{nameof(sender)} is not {nameof(TSender)}.");
                return;
            }

            Perform(senderTyped, BuildArgs(argsDict));
        }
    }
}