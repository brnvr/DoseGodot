using Dose.Core.Scenes;
using Godot;
using System;

namespace Dose.Core.Library.GameEvents
{
    public class FlickeringLightEvent : ILocationEvent
    {
        const double MIN_LIGHTS_ON_SECONDS = 0.4;
        const double MAX_LIGHTS_ON_SECONDS = 3.0;
        const double MIN_LIGHTS_OFF_SECONDS = 0.05;
        const double MAX_LIGHTS_OFF_SECONDS = 0.3;

        double _nextToggleAt;

        public void Activate(Location location)
        {
            location.TurnLightsOn();
            ScheduleNextToggle(MIN_LIGHTS_ON_SECONDS, MAX_LIGHTS_ON_SECONDS);
        }

        public void Deactivate(Location location)
        {
            location.TurnLightsOn();
            _nextToggleAt = 0;
        }

        public bool ShouldTrigger(Location target)
        {
            return Time.GetTicksMsec() / 1000.0 >= _nextToggleAt;
        }

        public void OnTrigger(Location target)
        {
            if (target.LightsOn)
            {
                target.TurnLightsOff();
                ScheduleNextToggle(MIN_LIGHTS_OFF_SECONDS, MAX_LIGHTS_OFF_SECONDS);
                return;
            }

            target.TurnLightsOn();
            ScheduleNextToggle(MIN_LIGHTS_ON_SECONDS, MAX_LIGHTS_ON_SECONDS);
        }

        void ScheduleNextToggle(double minimumSeconds, double maximumSeconds)
        {
            var delay = minimumSeconds + new Random().NextDouble() * (maximumSeconds - minimumSeconds);
            _nextToggleAt = Time.GetTicksMsec() / 1000.0 + delay;
        }
    }
}
