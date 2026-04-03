namespace Dose.Core.Library.Configuration.Dialogues
{
    public class ExecuteNode : DialogueConfigNode<ExecuteNodeArgs>
    {
        public required override ExecuteNodeArgs Args { get; init; }
        public override string Type => "execute";
        public override string? NextNode => Args.Next;
    }
}