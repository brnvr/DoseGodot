namespace Dose.Core.Library.Configuration.Dialogues
{
    public class DisplayOptionsNode : DialogueConfigNode<DisplayOptionsNodeArgs>
    {
        public override string Action => "displayOptions";
        public required override DisplayOptionsNodeArgs Args { get; init; }
        public override string? NextAction => null;
    }
}