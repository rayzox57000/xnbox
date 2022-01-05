using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Xnbox;

/* WIP */

public class Wip : Panel
{
	public Wip()
    {
		StyleSheet.Load("/ui/xnbox/Wip.scss");
		Add.Label("This Gamemode is currently WIP, thank you for being patient and understanding if you see bugs or other ...💕", "WIP1");
		Add.Label("The vehicles are currently broken ... Please do not take them at the risk of seeing your camera blocked (This comes from the basic sandbox gamemode, we are waiting for a fix from them) 🙂", "WIP2");
	}
}
