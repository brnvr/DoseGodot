namespace Dose.Core.Library.Interactivity
{
    public interface ILightsToggleable
    {
        bool LightsOn { get; }
        void TurnLightsOn();
        void TurnLightsOff();
    }
}
