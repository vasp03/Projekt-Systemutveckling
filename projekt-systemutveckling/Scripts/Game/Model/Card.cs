public class Card(string name, string textureAddress, bool movable, int cost) {
	/// <summary>
	///     Name of the card type.
	/// </summary>
	public string Name { get; private set; } = name;

	/// <summary>
	///     Location for Texture Address
	/// </summary>
	public string TextureAddress { get; protected set; } = textureAddress;

	/// <summary>
	///     Determines if the Card can be moved.
	/// </summary>
	public bool Movable { get; protected set; } = movable;

	public int Cost { get; protected set; } = cost;
}