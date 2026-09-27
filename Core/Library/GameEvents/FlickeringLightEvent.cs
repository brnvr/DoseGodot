using Dose.Core.Library.Interactivity;
using Godot;
using System;

namespace Dose.Core.Library.GameEvents
{
    public class FlickeringLightEvent(
        double minLightsOnSeconds,
        double maxLightsOnSeconds,
        double minLightsOffSeconds,
        double maxLightsOffSeconds
    ) : IGameEvent<ILightsToggleable>
    {
        double _nextToggleAt;

        public void Activate(ILightsToggleable target)
        {
            target.TurnLightsOn();
            ScheduleNextToggle(minLightsOnSeconds, maxLightsOnSeconds);
        }

        public void Deactivate(ILightsToggleable target)
        {
            target.TurnLightsOn();
            _nextToggleAt = 0;
        }

        public bool ShouldTrigger(ILightsToggleable target)
        {
            return Time.GetTicksMsec() / 1000.0 >= _nextToggleAt;
        }

        public void OnTrigger(ILightsToggleable target)
        {
            if (target.LightsOn)
            {
                target.TurnLightsOff();
                ScheduleNextToggle(minLightsOffSeconds, maxLightsOffSeconds);
                return;
            }

            target.TurnLightsOn();
            ScheduleNextToggle(minLightsOnSeconds, maxLightsOnSeconds);
        }

        void ScheduleNextToggle(double minimumSeconds, double maximumSeconds)
        {
            var delay = minimumSeconds + new Random().NextDouble() * (maximumSeconds - minimumSeconds);
            _nextToggleAt = Time.GetTicksMsec() / 1000.0 + delay;
        }
    }
}
