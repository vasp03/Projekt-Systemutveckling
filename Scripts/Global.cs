using Godot;

namespace Goodot15.Scripts;

public partial class Global : Node {
    private const string baseTexturePath = "res://Assets/Cards/Ready To Use/";

    private const string textureEnding = ".png";

    public static string GetTexturePath(string textureAddress) {
        return baseTexturePath + textureAddress + textureEnding;
    }
}