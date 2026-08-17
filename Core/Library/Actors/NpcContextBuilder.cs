using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Actors;
using Dose.Core.Library.Configuration.Dialogues;
using Dose.Core.Library.DataStructures.Dialogues;
using Dose.Core.Library.ESpeak;
using Dose.Core.Library.Interactivity;
using Godot;

namespace Dose.Core.Library.Actors
{
    public class NpcContextBuilder(
        IEnumerable<NpcContextBuilderAction> actions,
        Dictionary<string, IActionPerformer> itemInteractionPerformerMap
    ) : IObjectFromConfigBuilder<NpcContext>
    {
        public Dictionary<string, IActionPerformer> ItemInteractionPerformerMap { get; } = itemInteractionPerformerMap;
        public DialogueConfigConverter DialogueConfigConverter = new(actions.ToDictionary(a => a.Key, a => a.DialogueNodeConverter));
        public DialogueBuilder DialogueBuilder = new(
            actions.ToDictionary(a => a.Key, a => a.DialogueNodeBuilder),
            buildShortDialogue: node =>
            {
                if (node is not SpeakNode speakNode || speakNode.Args.Lines.Count() > 1)
                {
                    return null;
                }

                return speakNode.Args.Lines.First();
            });

        public NpcContext Build(NpcConfig npcConfig)
        {
            return new NpcContext
            {
                Identifier = npcConfig.Identifier,
                DisplayName = npcConfig.DisplayName,
                Rarity = Enum.Parse<Rarity>(npcConfig.Rarity, true),
                PackId = npcConfig.PackId,
                LocationRestriction = npcConfig.LocationRestriction,
                Texture = ResourceLoader.Load<Texture2D>(npcConfig.TexturePath),
                Voice = new Voice(npcConfig.Voice, 16000),
                DialogueMap = GetDialogueMap(npcConfig),
                AvailableDialogueKeys = npcConfig.AvailableDialogues,
                InteractWith = npcConfig.InteractWith.Select(config => new ItemInteractionOperator
                {
                    ItemIdentifier = config.Item,
                    Performer = ItemInteractionPerformerMap[config.Action],
                    Args = config.Args
                }),
                Scale = npcConfig.Scale
            };
        }

        NpcContext IObjectFromConfigBuilder<NpcContext>.Build(YamlConfig config)
        {
            return Build((NpcConfig)config);
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
