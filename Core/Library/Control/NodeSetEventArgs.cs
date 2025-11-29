using System;
using Dose.Core.Library.DataStructures;

namespace Dose.Core.Library.Control
{
    public class NodeSetEventArgs(IDialogueNode node) : EventArgs
    {
        public IDialogueNode Node { get; } = node;
    }
}