using System;

public abstract class Card {
	private const string baseTexturePath = "res://Assets/Cards/Ready To Use/";
	private const string textureEnding = ".png";

	/// <summary>
	///     Constructor for the Card class
	///     Texture address should be the address after "res://Assets/Cards/Ready To Use/".
	/// </summary>
	/// <param name="textureAddress"></param>
	/// <param name="movable"></param>
	/// <param name="cardNode"></param>
	/// <param name="name"></param>
	public Card(string textureAddress, bool movable, CardNode cardNode) {
		// Generate a unique uuid as name
		ID = Guid.NewGuid().ToString();
		_texturePath = baseTexturePath + textureAddress + textureEnding;
		Movable = movable;
		CardNode = cardNode;
	}

	public CardNode CardNode { get; set; }

	public string ID { get; private set; }
	private readonly string _texturePath;
	public string TexturePath { get => _texturePath; }
	public bool Movable { get; set; }

	public string TextureType {
		get {
			string[] split = TexturePath.Split("/");
			string textureType = split[split.Length - 1];
			textureType = textureType.Substring(0, textureType.Length - 4);
			return textureType;
		}
	}
}