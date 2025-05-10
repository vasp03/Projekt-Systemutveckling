using Goodot15.Scripts.Game.Model.Parents;

public class BuildingCampfire() : CardBuilding("Campfire", true, 1) {
    public override string ProduceCard() {
        return "null";
    }
    
    protected override int SetValue() {
        return 5;  // Wood is worth 5 coins
    }
}