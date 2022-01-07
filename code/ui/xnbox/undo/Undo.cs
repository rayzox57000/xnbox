using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Xnbox;

public class Undo : Panel
{

	Panel Container;
	string LastObjectCache = "";
	int LastObjectUndoCache = -1;

	public void Init()
    {
		this.DeleteChildren(true);
		Container = Add.Panel("Container");
	}

	public Undo()
    {
		StyleSheet.Load("/ui/xnbox/undo/Undo.scss");
		Init();
	}

	public override void OnHotloaded()
	{
		base.OnHotloaded();
		Init();
	}

	public async void newElem(string Name, bool Undo = false, bool Infinite = false)
    {
		UndoElement ue = new UndoElement(Name, Undo);
		Container.AddChild(ue);
		await Task.Delay(100);
		ue.Container.AddClass("Show");
		if (Infinite == false)
		{
			await Task.Delay(3000);
			ue.Container.AddClass("Delete");
			await Task.Delay(250);
			ue.Delete(true);
		}
	}

	public override void Tick()
	{

		SandboxPlayer player = Local.Pawn as SandboxPlayer;
		if (player == null) return;

		string undo = player.LastObjectOk;


		if (undo != null || undo != "" || player.LastObjectOkUndo == -1)
        {
			if(LastObjectCache != undo && player.LastObjectOkUndo != LastObjectUndoCache)
            {
				LastObjectCache = player.LastObjectOk;
				LastObjectUndoCache = player.LastObjectOkUndo;
				string Type = undo.Split(",")[0];
				newElem(Type, LastObjectUndoCache == 1);
				LastObjectUndoCache = -1;
			}

			
        }

	}

}
