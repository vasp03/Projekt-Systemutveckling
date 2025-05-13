using Goodot15.Scripts.Game.Model.Parents;

public class BuildingGreenhouse() : CardBuilding("Greenhouse", true, 1) {
    public override string ProduceCard() {
        return "null";
    }

    protected override int SetValue() {
        return 40;
    }
}