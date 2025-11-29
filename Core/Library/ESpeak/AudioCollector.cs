using System;
using System.Collections.Generic;
using Godot;

namespace Dose.Core.Library.ESpeak
{
    public class AudioCollector(int sampleRate)
    {
        readonly List<short> _samples = [];

        public int SampleRate { get; } = sampleRate;
        public EventHandler<SynthCompleteEventArgs>? SynthCompleted { get; set; }

        public unsafe void Synth(string text)
        {
            _samples.Clear();

            Dll.espeak_SetSynthCallback(SynthCallback);
            Dll.espeak_Synth(text, 10000, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero);
        }

        public static void InitializeESpeak()
        {
            Dll.espeak_Initialize(2, 0, "espeak-ng-data", 0);
        }

        unsafe int SynthCallback(short* wav, int numsamples, IntPtr events)
		{
			if (wav == null)
			{
                var wavBytes = CreateWav([.. _samples]);

                SynthCompleted?.Invoke(this, new SynthCompleteEventArgs(wavBytes, SampleRate));

				return 0;
			}

			for (int i = 0; i < numsamples; i++)
            {
                _samples.Add(wav[i]);
            }

			return 0;
		}

        static byte[] CreateWav(short[] pcm, int sampleRate = 22050)
        {
            int byteRate = sampleRate * 2;
            int dataLength = pcm.Length * 2;

            using var ms = new System.IO.MemoryStream();
            using var bw = new System.IO.BinaryWriter(ms);

            bw.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            bw.Write(36 + dataLength);
            bw.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

            bw.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            bw.Write(16);
            bw.Write((short)1);
            bw.Write((short)1);
            bw.Write(sampleRate);
            bw.Write(byteRate);
            bw.Write((short)2);
            bw.Write((short)16);

            bw.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            bw.Write(dataLength);

            foreach (var s in pcm)
            {
                bw.Write(s);
            }   

            return ms.ToArray();
        }
    }
}