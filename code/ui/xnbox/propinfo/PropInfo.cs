using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Xnbox;

/* WIP */

public class PropInfo : Panel
{

	Panel Window;
	Panel WindowTop;
	Label WindowTopTitle;
	Button WindowTopClose;

	Panel WindowBottom;

	Panel WindowLeft;
	Panel WindowRight;

	IReadOnlyList<Client> Players = new ReadOnlyCollection<Client>(new Client[] { });

	public void SetCenterFlex(Panel e, bool col = false)
	{
		e.AddClass(string.Format("CENTER_FLEX {0}", col == true ? " COL" : ""));
	}

	public void Init()
	{
		this.DeleteChildren(true);
		SetCenterFlex(this, true);
		Window = Add.Panel("Window");
		WindowTop = Window.Add.Panel("Top");
		WindowTopTitle = WindowTop.Add.Label("Props Info","Title");
		WindowTopClose = WindowTop.Add.Button("X","Close");
		SetCenterFlex(WindowTopClose, true);

		WindowBottom = Window.Add.Panel("Bottom");

		WindowLeft = WindowBottom.Add.Panel("Left");
		WindowRight = WindowBottom.Add.Panel("Right");
	}

	public PropInfo()
	{
		StyleSheet.Load("/ui/xnbox/propinfo/PropInfo.scss");
		Init();
	}

	public override void OnHotloaded()
	{
		base.OnHotloaded();

		Init();
	}

	public void WindowLeftUpdate()
    {
		WindowLeft.DeleteChildren(true);
		foreach (Client cl in Players)
        {
			Log.Info("ADD CLIENT ");
        }

	}

	public override void Tick()
    {

		/*IReadOnlyList<Client> CPlayers = Client.All;

		if (Players.Count != CPlayers.Count)
		{
			Log.Info("change");
			Players = CPlayers;
			WindowLeftUpdate();
		}*/
    }


	

}
