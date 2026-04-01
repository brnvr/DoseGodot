using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Actors;
using Dose.Core.Library.Configuration.Dialogues;
using Dose.Core.Library.DataStructures.Dialogues;
using Dose.Core.Library.ESpeak;
using Godot;

namespace Dose.Core.Library.Actors
{
    public class NpcContextBuilder(DialogueConfigConverter dialogueConfigConverter, DialogueBuilder dialogueBuilder) : IActorContextBuilder
    {
        DialogueConfigConverter DialogueConfigConverter { get; } = dialogueConfigConverter;
        DialogueBuilder DialogueBuilder { get; } = dialogueBuilder;

        public NpcContext BuildContext(NpcConfig npcConfig)
        {
            return new NpcContext
            {
                DisplayName = npcConfig.DisplayName,
                Texture = ResourceLoader.Load<Texture2D>(npcConfig.TexturePath),
                Voice = new Voice(npcConfig.Voice, 16000),
                DialogueMap = GetDialogueMap(npcConfig),
                AvaliableDialogueKeys = npcConfig.AvaliableDialogues
            };
        }

        IActorContext IActorContextBuilder.BuildContext(YamlConfig config)
        {
            return BuildContext((NpcConfig)config);
        }

        Dictionary<string, IDialogue> GetDialogueMap(NpcConfig npcConfig)
        {
            var files = Directory.GetFiles(npcConfig.DialoguesDir);

            return files.ToDictionary(f => Path.GetFileNameWithoutExtension(f), f =>
            {
                var loaded = (Dictionary<string, object?>)YamlConfig.FromYaml(f);
                var config = DialogueConfigConverter.FromDict(loaded);
                var dialogue = DialogueBuilder.BuildDialogue(config);

                return dialogue;
            });
        }
    }
}