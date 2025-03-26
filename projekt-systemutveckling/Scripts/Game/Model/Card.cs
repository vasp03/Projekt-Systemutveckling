public class Card
{
	public Card(string name, string textureAddress, bool movable, int cost)
	{
		Name = name;
		TextureAddress = textureAddress;
		Movable = movable;
		Cost = cost;
	}

	/// <summary>
	///     Name of the card type.
	/// </summary>
	public string Name { get; private set; }

	/// <summary>
	///     Location for Texture Address
	/// </summary>
	public string TextureAddress { get; protected set; }

	/// <summary>
	///     Determines if the Card can be moved.
	/// </summary>
	public bool Movable { get; protected set; }

	public int Cost { get; protected set; }

	// public String GetName()
	// {
	//     return Name;
	// }
// 
// public String GetTextureAddress()
// {
//     return TextureAddress;
// }

// public bool IsMovable()
// {
//     return movable;
// }

// public int GetCost()
// {
//     return cost;
// }
}