using System.Collections;
using System.Collections.Generic;
using Dose.Core.Library.Common;
using Dose.Core.Library.DataStructures.Dialogues;
using Dose.Core.Library.ESpeak;
using Godot;
using Dose.Core.Library.Numbers;

namespace Dose.Core.Library.Actors
{
    public record NpcContext : IActorContext, IHasRarity
    {
        public required string Identifier { get; init; }
        public string DisplayName { get; init; } = "";
        public Rarity Rarity { get; init; }
        public Dictionary<string, IDialogue> DialogueMap { get; init; } = [];
        public IEnumerable<string> AvailableDialogueKeys { get; init; } = [];
        public Texture2D? Texture { get; init; }
        public Voice? Voice { get; init; }
        public int MaxQuantity { get; init; } = 1;
        public ScaleFactor Scale { get; init; } = 1f;
    }
}
