using System;
using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.Configuration.Dialogues
{
    public class DialogueConfigConverter(Dictionary<string, YamlConfigConverter> nodeConverterMap) : YamlConfigConverter
    {
        Dictionary<string, YamlConfigConverter> NodeConverterMap { get; } = nodeConverterMap;

        public override YamlConfig FromDict(Dictionary<string, object?> dict)
        {
            var nodes = GetRequired<Dictionary<string, object?>>(dict, "nodes");

            var config = new DialogueConfig
            {
                StartAt = GetRequired<string>(dict, "startAt"),
                Nodes = nodes.ToDictionary(node => node.Key, node =>
                {
                    var nodeDict = node.Value as Dictionary<string, object> ?? throw new FormatException($"Invalid format for node {node.Key}.");
                    var action = nodeDict["action"] as string ?? throw new FormatException($"Invalid action for node {node.Key}.");
                    var converter = NodeConverterMap[action];
                    var args = nodeDict["args"] as Dictionary<string, object?> ?? throw new FormatException($"Invalid args for action {action} in node {node.Key}.");

                    return (DialogueConfigNode)converter.FromDict(args);
                })
            };

            return config;
        }
    }
}