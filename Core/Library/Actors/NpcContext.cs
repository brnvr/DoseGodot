using System.Collections;
using System.Collections.Generic;
using Dose.Core.Library.DataStructures.Dialogues;
using Dose.Core.Library.ESpeak;
using Godot;

namespace Dose.Core.Library.Actors
{
    public record NpcContext : IActorContext
    {
        public required string Identifier { get; init; }
        public string DisplayName { get; init; } = "";
        public Dictionary<string, IDialogue> DialogueMap { get; init; } = [];
        public IEnumerable<string> AvaliableDialogueKeys { get; init; } = [];
        public Texture2D? Texture { get; init; }
        public Voice? Voice { get; init; }
        public int MaxQuantity { get; init; } = 1;
    }
}