using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Xnbox;

public class UndoElement : Panel
{

	public Panel Container;
	Label Text;

	public UndoElement(string Name = "Entity", bool Undo = false)
	{
		StyleSheet.Load("/ui/xnbox/undo/Undo.scss");
		this.DeleteChildren(true);
		Container = Add.Panel("Container");
		Container.AddClass($"{(Undo == true ? "Undo" : "Spawn")}");
		Text = Container.Add.Label($"{Name} {(Undo == true ? "Undo" : "Spawned")}!", "label");
	}

}
