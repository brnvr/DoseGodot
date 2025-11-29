using System;
using Dose.Core.Library.Actors;
using Dose.Core.Scenes;

namespace Dose.Core.Library.DataStructures
{
    public class ItemDroppedEventArgs(ItemContext itemContext) : EventArgs
    {
        public ItemContext ItemContext { get; } = itemContext;
    }
}