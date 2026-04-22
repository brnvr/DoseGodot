using System.Collections.Generic;
using Dose.Core.Library.Actors;
using Godot;

namespace Dose.Core.Library.Common
{
    public abstract class InteractionPerformer<TSender, TArgs>() : ActionPerformer<TSender, TArgs> where TArgs : InteractionPerformerArgs, new()
    {
        public override TArgs BuildArgs(Dictionary<string, object?> argsDict)
        {
            argsDict.TryGetValue("actorContext", out object? contextObj);

            if (contextObj is not IActorContext context)
            {
                GD.PushWarning($"{nameof(argsDict)}.actorContext is not of type ${nameof(IActorContext)}.");
                return BuildArgs(null, argsDict);
            }
            
            return BuildArgs(context, argsDict);
        }

        public virtual TArgs BuildArgs(IActorContext? context, Dictionary<string, object?> argsDict)
        {
            return new TArgs
            {
                ActorContext = context
            };
        }
    }

    public abstract class InteractionPerformer<TSender> : InteractionPerformer<TSender, InteractionPerformerArgs> { }
}