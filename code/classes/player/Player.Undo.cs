using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xnbox;

public delegate bool CallbackUndo(List<object> objects);


partial class SandboxPlayer
{
	public List<object> Objects { get; set; } = new List<object>();

	public void AddCustomUndo(string type, CallbackUndo callbackUndo = null, params object[] argslist)
	{

		List<object> list = new List<object>();

		foreach (object o in argslist)
		{
			Log.Info("add " + o);
			list.Add(o);
		}

		Log.Info(callbackUndo == null);

		CustomUndo cu = callbackUndo == null ? new CustomUndo(type, list) : new CustomUndo(type, list, callbackUndo);
		Objects.Add(cu);
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
				if (correct == true) return;
			}
		}
		UndoObject();
		return;

	}

}
