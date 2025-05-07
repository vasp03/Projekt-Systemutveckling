namespace Goodot15.Scripts.Game.Controller.Events;

public class MeteoriteEvent : IGameEvent {
    public string EventName => "Meteorite Strike";
    public int TicksUntilNextEvent => Utilities.TimeToTicks(1);
    public double Chance => .5d;

    public void OnEvent(GameEventContext context) {
    }
}