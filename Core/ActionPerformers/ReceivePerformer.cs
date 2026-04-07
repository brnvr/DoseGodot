using System.Collections.Generic;
using Dose.Core.Library.Common;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.ActionPerformers
{
    public class ReceivePerformer : IActionPerformer
    {
        public void Perform(Dictionary<string, object?> args)
        {
            GD.Print("Item received");
        }
    }
}