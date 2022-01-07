using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xnbox;


public class CustomUndo
{
	public string Type { get; private set; } = "";
	private List<object> list { get; set; } = new List<object>();
	private readonly CallbackUndo Start;

	public CustomUndo(string type, List<object> list, CallbackUndo callbackundo)
	{
		this.Type = type;
		this.list = list;
		this.Start = callbackundo;
	}

	public CustomUndo(string type,List<object> list)
    {
		this.Type = type;
		this.list = list;
		this.Start = DefaultCustomRemove.Remove;
    }

	public bool Launch()
    {
		return this.Start(this.list);
	}

}

class DefaultCustomRemove
{
	public static bool Remove(List<object> objs)
	{
		if (objs.Count > 0)
		{
			Entity prop = objs.First() as Entity;
			if (prop != null && prop.IsValid())
			{
				prop.PlaySound("balloon_pop_cute");
				prop.Delete();
				return true;
			}
		}
		return false;
	}
}

