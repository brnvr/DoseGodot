using System.Collections.Generic;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.ActionPerformers
{
    public class ReceivePerformer : IActionPerformer
    {
        public void Perform(object sender, Dictionary<string, object?> args)
        {
            if (sender is not ItemContext item)
            {
                GD.PushWarning($"{nameof(sender)} is not an {nameof(ItemContext)}. Skipping {nameof(Perform)}.");
                return;
            }

            GD.Print($"Item received: {item.Identifier}");
        }
    }
}