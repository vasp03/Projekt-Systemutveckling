using System;

public abstract class Card {
	private const string baseTexturePath = "res://Assets/Cards/Ready To Use/";
	private const string textureEnding = ".png";

	/// <summary>
	///     Constructor for the Card class
	///     Texture address should be the address after "res://Assets/Cards/Ready To Use/".
	/// </summary>
	/// <param name="name"></param>
	/// <param name="textureAddress"></param>
	/// <param name="movable"></param>
	/// <param name="cost"></param>
	public Card(string textureAddress, bool movable, int cost) {
		// Generate a unique uuid as name
		ID = Guid.NewGuid().ToString();
		TexturePath = baseTexturePath + textureAddress + textureEnding;
		Movable = movable;
		Cost = cost;
	}

	public string ID { get; private set; }
	public string TexturePath { get; protected set; }
	public bool Movable { get; set; }
	public int Cost { get; set; }
	public bool Highlighted { get; set; }
}