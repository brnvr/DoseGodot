using System;
using System.Collections.Generic;
using System.IO;
using Godot;

namespace Dose.Core.Library.ESpeak
{
    public class Speaker(AudioCollector audioCollector)
    {
        public AudioCollector AudioColletor { get; } = audioCollector;
        
        public void Synth(string text)
        {
            AudioColletor.Synth(text);
        }
    }
}