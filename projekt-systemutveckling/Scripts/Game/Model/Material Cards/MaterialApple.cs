public partial class MaterialApple(string name, string textureAddress, bool movable, int cost) : CardMaterial(name, textureAddress, movable, cost), IEdible
{
    private int foodAmount;

    public int GetFoodAmount()
    {
        return foodAmount;
    }

    public int RemoveFoodAmount()
    {
        foodAmount = 0;
        return foodAmount;
    }

}
