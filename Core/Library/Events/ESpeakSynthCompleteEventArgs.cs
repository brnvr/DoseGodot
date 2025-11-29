namespace Dose.Core.Library.Events
{
    public class ESpeakSynthCompleteEventArgs(byte[] wavBytes, int sampleRate) : System.EventArgs
    {
        public byte[] WavBytes { get; } = wavBytes;
        public int SampleRate { get; } = sampleRate;
    }
}