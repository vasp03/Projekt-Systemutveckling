using Godot;
using Goodot15.Scripts.Game.View;


public partial class MenuController : Node {
	private Control currentMenu;
	private Control previousMenu;
	
	private Control mainMenu;
	private Control pauseMenu;
	private Control optionsMenu;
	private NodeController nodeController;
	
	
	public override void _Ready() {
		mainMenu = GetParent().GetNode<Control>("MainMenu");
		currentMenu = mainMenu;
		pauseMenu = null;
		optionsMenu = null;
	}
	
	public void OpenMainMenu() {
		if (mainMenu == null) {
			PackedScene packedMainMenu = GD.Load<PackedScene>("res://Scenes/MenuScenes/MainMenu.tscn");
			mainMenu = packedMainMenu.Instantiate() as Control;
			AddChild(mainMenu);
		} 
		
		SwitchMenu(mainMenu);
	}
	
	public void OpenPauseMenu() {
		if (GetTree().Paused) {
			return;
		}
		GetTree().Paused = true;
		
		if(pauseMenu == null) {
			PackedScene packedPauseMenu = GD.Load<PackedScene>("res://Scenes/MenuScenes/GamePausedMenu.tscn");
			pauseMenu = packedPauseMenu.Instantiate() as Control;
			AddChild(pauseMenu);
		} 
		
		SwitchMenu(pauseMenu);
	}
	
	public void OpenOptionsMenu() {
		previousMenu = currentMenu;
		if(optionsMenu == null) {
			PackedScene packedOptionsMenu = GD.Load<PackedScene>("res://Scenes/MenuScenes/OptionsMenu.tscn");
			optionsMenu = packedOptionsMenu.Instantiate() as Control;
			AddChild(optionsMenu);
		} 
		SwitchMenu(optionsMenu);
	}

	public void SwitchMenu(Control newMenu)
	{
		if (newMenu != null && newMenu.IsInsideTree())
		{	
			currentMenu = newMenu;
			newMenu.Visible = true;
			if (previousMenu != null) {
				previousMenu.Visible = false;
			}
		}
	}
	
	public void GoBackToPreviousMenu() {
		if (previousMenu != null && previousMenu.IsInsideTree()) {
			Control menuToSwitchTo = previousMenu;
			previousMenu = currentMenu;
			SwitchMenu(menuToSwitchTo);
		} 
	}

	public void CloseMenus() {
		foreach (Node menu in GetChildren()) {
			if (menu is Control controlMenu && controlMenu.IsInsideTree()) {
				controlMenu.Visible = false;
			}
		}
		GetTree().Paused = false;
		nodeController.Visible = true;
	}
	
	public void SetNodeController(NodeController nodeController) {
		this.nodeController = nodeController;
	}
}