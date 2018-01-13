namespace DG
{
    public interface ISwitch
    {
        bool IsTurnOn { get; }

        void TurnOn();
        void TurnOff();
    }
}
