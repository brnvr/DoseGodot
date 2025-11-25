using System;
using System.IO;
using System.Linq;
using Godot;

namespace Dose.Core.Library.ESpeak
{
    public class Voice
    {
        public string Name { get; }
        public int SampleRate { get; }
        public string ESpeakPath { get; }

        public Voice(string name, int sampleRate = 22050, string eSpeakPath = "espeak-ng-data")
        {
            Name = name;
            SampleRate = sampleRate;
            ESpeakPath = eSpeakPath;

            ValidateVoice();
        }

        void ValidateVoice()
        {
            bool exists = Directory
                .EnumerateFiles(Path.Join(ESpeakPath, "voices"), "*", SearchOption.AllDirectories)
                .Any(f => Path.GetFileNameWithoutExtension(f).Equals(Name, StringComparison.OrdinalIgnoreCase));

            if (!exists)
            {
                GD.PushError($"Voice not found: {Name}");
            }
        }
    }
}