using Godot;
using Dose.Core.Library.Common;
using System;

namespace Dose.Core.Library.ESpeak
{
    public partial class ESpeaker : Node, ISpeaker
    {
        readonly AudioCollector _audioCollector;
        readonly AudioStreamPlayer _audioStreamPlayer;
        
        public Action OnFinished { get; }

        public ESpeaker(Action onFinished)
        {
            OnFinished = onFinished;

            _audioCollector = new AudioCollector(ProcessESpeakAudio);
            _audioStreamPlayer = new AudioStreamPlayer();
            _audioStreamPlayer.Finished += OnFinished;

            AddChild(_audioStreamPlayer);
        }

        public void Speak(string text)
        {
            _audioCollector.Synth(text);
        }

         void ProcessESpeakAudio(byte[] data)
		{
			var stream = new AudioStreamWav
			{
				MixRate = 22050,
				Data = data,
				Format = AudioStreamWav.FormatEnum.Format16Bits
			};

			_audioStreamPlayer.Stream = stream;

			_audioStreamPlayer.Play();
		}
    }
}