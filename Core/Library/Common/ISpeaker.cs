namespace Dose.Core.Library.Common
{
    public interface ISpeaker
    {
        void Speak(string text);
        void Stop();
        bool IsSpeaking { get; }
    }
}