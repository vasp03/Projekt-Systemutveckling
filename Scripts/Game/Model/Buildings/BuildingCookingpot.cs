using Goodot15.Scripts.Game.Model.Parents;

public class BuildingCookingpot() : CardBuilding("Cookingpot", true, 1) {
    public override string ProduceCard() {
        return "null";
    }
    
    protected override int SetValue() {
        return 35;
    }
}