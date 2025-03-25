public partial class MaterialApple(string textureAddress, bool movable, int cost) : CardMaterial(textureAddress, movable, cost), IEdible
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
