using Godot;
using Dose.Core.Library.Common;
using System;

namespace Dose.Core.Library.ESpeak
{
    public partial class ESpeaker : Node3D, ISpeaker
    {
        readonly AudioCollector _audioCollector;
        readonly AudioStreamPlayer3D _audioStreamPlayer;
        
        public Voice Voice { get; }
        public Action? OnFinished { get; }
        public bool IsSpeaking { get; protected set; }

        public ESpeaker(Voice voice, Action? onFinished)
        {
            OnFinished = onFinished;
            Voice = voice;

            _audioCollector = new AudioCollector(voice.SampleRate, ProcessESpeakAudio);
            _audioStreamPlayer = new AudioStreamPlayer3D();
            
            if (OnFinished != null)
            {
                _audioStreamPlayer.Finished += OnFinished;
            }

            _audioStreamPlayer.Finished += () =>
            {
                IsSpeaking = false;
            };

            AddChild(_audioStreamPlayer);
        }

        public void Speak(string text)
        {
            IsSpeaking = true;
            Dll.espeak_SetVoiceByName(Voice.Name);
            _audioCollector.Synth(text);
        }

         void ProcessESpeakAudio(byte[] data, int sampleRate)
		{
			var stream = new AudioStreamWav
			{
				MixRate = sampleRate,
				Data = data,
				Format = AudioStreamWav.FormatEnum.Format16Bits
			};

			_audioStreamPlayer.Stream = stream;

			_audioStreamPlayer.Play();
		}
    }
}