namespace Dose.Core.Library.ESpeak
{
    public class SynthCompleteEventArgs(byte[] wavBytes, int sampleRate)
    {
        public byte[] WavBytes { get; } = wavBytes;
        public int SampleRate { get; } = sampleRate;
    }
}