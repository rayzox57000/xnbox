using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xnbox;
using System.Diagnostics;

public delegate bool CallbackUndo(List<object> objects);


partial class SandboxPlayer
{
	public List<object> Objects { get; set; } = new List<object>();
	[Net] public string LastObjectOk { get; private set; } = "";
	[Net] public int LastObjectOkUndo { get; private set; } = -1;

	public string rnd()
    {
		var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		var Charsarr = new char[8];
		var random = new Random();

		for (int i = 0; i < Charsarr.Length; i++)
		{
			Charsarr[i] = characters[random.Next(characters.Length)];
		}

		var resultString = new String(Charsarr);
		return resultString;
	}

	public string ts()
    {
		DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
		return now.ToUnixTimeSeconds().ToString();
	}

	public void AddCustomUndo(string type, CallbackUndo callbackUndo = null, params object[] argslist)
	{

		List<object> list = new List<object>();

		foreach (object o in argslist)
		{
			list.Add(o);
		}

		CustomUndo cu = callbackUndo == null ? new CustomUndo(type, list) : new CustomUndo(type, list, callbackUndo);
		Objects.Add(cu);
		LastObjectOk = $"{cu.Type},{ts()},{rnd()}";
		LastObjectOkUndo = 0;
	}

	public void AddParticles(Particles p)
	{
		if (p == null) return;
		Log.Error("PARTICLES [SPAWNED]");
		Objects.Add(p);
	}

	public void AddWeld(Prop e, Prop root)
	{
		if (e == null || root == null) return;
		Log.Error("WELD [ADDED]");
		List<Prop> obj = new List<Prop>();
		obj.Add(e);
		obj.Add(root);
		Objects.Add(obj);
	}

	public void UndoObject()
	{
		if (Objects.Count == 0) return;
		object o = Objects.Last();

		if (o is CustomUndo)
		{
			CustomUndo cu = o as CustomUndo;
			if (cu != null)
			{
				var correct = cu.Launch();
				Objects.RemoveAt(Objects.Count - 1);
				if (correct == true)
				{
					LastObjectOk = $"{cu.Type},{ts()},{rnd()}";
					LastObjectOkUndo = 1;
					return;
				}
			}
		}
		LastObjectOkUndo = -1;
		LastObjectOk = "";
		UndoObject();
		return;

	}

}
