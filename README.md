# [Project Card - Group 15](https://github.com/vasp03/Projekt-Systemutveckling)
[Github page](https://github.com/vasp03/Projekt-Systemutveckling)

Project Card is a game project built with the game engine `Godot`. Utilizing the programming language **C#**.

This project is a part of a university system development project.

# Installation & Development environment
To set up a development environment for this project; A few tools and Software Development Kits are necessary

 - [Godot 4.4, Mono Edition (Windows)](https://github.com/godotengine/godot-builds/releases/download/4.4-stable/Godot_v4.4-stable_mono_win64.zip) | [Godot 4.4, Mono Edition (Mac)](https://github.com/godotengine/godot-builds/releases/download/4.4-stable/Godot_v4.4-stable_mono_macos.universal.zip)
 - [.NET SDK 8.0 (Windows)](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.407-windows-x64-installer) | [.NET SDK 8.0 (Mac)](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.407-macos-arm64-installer)
 - (Any) of the optional IDE with support for `C#`
    - [Rider](https://www.jetbrains.com/rider/) (Recommended)
    - [Visual Studio Code](https://code.visualstudio.com/) (Install **C#** Extension Kit)
    - [Visual Studio](https://visualstudio.microsoft.com/vs/community/)

# Opening the project
To open the project, use the Godot executable and open the `project.godot` file using the `Godot` executable.

To open the source code for the project, use any of the Integrated Development Environments and open the `Goodot 15.sln` solution file.

Project code is located in the `Scripts` folder.
Assets are located in the `Assets` folder.

Misc. project items are located in misc folders like `documentation`, `Div` and `Visual Paradigm` folders.

# Building & Running the project
Building the project requires `.NET 8.0`. To build the project through Godot. Simply run the Build and run the project in the Godot editor.
![alt text](documentation/run.png)

This is also possible to do through your IDE of your choice.

- For Rider, import the `.sln` file and build & run the project directly in the editor
- For Visual Studio, import the `.sln` file and build & run the project directly in the editor
- For Visual Studio Code, additional setup is required. Read more [here](https://docs.godotengine.org/en/stable/contributing/development/configuring_an_ide/visual_studio_code.html)