using Dose.Core.Library.ESpeak;
using Godot;

namespace Dose.Core.Library.Common
{
    public interface ISpeaker
    {
        void Speak(string text);
        bool IsSpeaking { get; }
    }
}