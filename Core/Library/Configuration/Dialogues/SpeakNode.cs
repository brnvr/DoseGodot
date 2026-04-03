namespace Dose.Core.Library.Configuration.Dialogues
{
    public class SpeakNode : DialogueConfigNode<SpeakNodeArgs>
    {
        public override string Type => "speak";
        public required override SpeakNodeArgs Args { get; init; }
        public override string? NextNode => Args.Next;
    }
}