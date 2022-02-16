using Sandbox;
using System;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Text.RegularExpressions;
using System.IO;
using Xnbox;

partial class BottomLeft : Panel
{

	Image Avatar;
	Label NameL;
	Client client;

	Image EyeProp;

	Label HealthIcon;
	int HealthIconDelayDown = 0;
	bool HealthIconUp = false;

	Label HealthIconValue;

	Panel HealthBar;
	Panel HealthBarInline;

	ModelEntity EyeEntity;
	Image PropIcon;
	Label PropName;
	Label PropOwner;

	public void Init()
	{
		this.DeleteChildren(true);

		Avatar = this.Add.Image($"avatarbig:{Local.PlayerId}");
		Avatar.AddClass("AVATAR");

		NameL = Add.Label("", "NAME");

		HealthBarInline = Add.Panel("HEALTH_BAR INLINE");
		HealthBar = Add.Panel("HEALTH_BAR");

		HealthIcon = Add.Label("❤️", "ICON_HEALTH");

		HealthIconValue = Add.Label("100", "ICON_HEALTH_VALUE");

		PropIcon = Add.Image("");
		PropIcon.AddClass("PROP_ICON");

		PropName = Add.Label("PROPNAME", "PROPNAME");
		PropOwner = Add.Label("PROPOWNER", "PROPOWNER");
	}

	public BottomLeft()
	{
		StyleSheet.Load("/ui/xnbox/base/BottomLeft.scss");
		Init();
	}


	public override void OnHotloaded()
	{
		base.OnHotloaded();

		Init();
	}

	public override void Tick()
	{
		
		SandboxPlayer player = Local.Pawn as SandboxPlayer;
		if (player == null) return;
		EyeEntity = null;

		if (client == null) {
			foreach (Client cl in Client.All) if (cl.PlayerId == Local.PlayerId) {
					client = cl;
			}
		} else if (client != null && client.Name != "")
        {
			NameL.Text = client.Name;
        }

		if (player.Vehicle == null)
		{
			var eyePos = player.EyePosition;
			var eyeRot = player.EyeRotation;
			var tr = Trace.Ray(eyePos, eyePos + eyeRot.Forward * 5000).Radius(2).Ignore(player).EntitiesOnly().Run();
			EyeEntity = tr.Entity as ModelEntity;
		}

		string MN = "";
		string CN = "";
		string path = "";
		SandboxPlayer sp = null;

		if (EyeEntity != null)
		{
			PropTouch p = EyeEntity as PropTouch;
			if (p != null)
			{
				sp = p.OwnerSpawn as SandboxPlayer;
				path = EyeEntity.GetModelName();
				if(sp != null) CN = $"OWNER : {sp.Client.Name} <{sp.Client.PlayerId}>";
				if (sp != null) MN = $"MODEL NAME : {Path.GetFileNameWithoutExtension(path)}";

			}


			if (sp != null && EyeEntity.ClassInfo.Name == "prop_physics" && path != "")
			{
				try
				{
					PropIcon.SetTexture($"{path}_c.png");
				}
				catch (Exception e)
				{}
			} else if (sp == null)
                {
					PropIcon.SetClass("HIDE", true);
				}
		}

		PropIcon.SetClass("HIDE", sp == null || EyeEntity == null || (EyeEntity != null && EyeEntity.ClassInfo.Name != "prop_physics"));

		PropName.Text = MN;
		PropOwner.Text = CN;


		int health = player.Health.CeilToInt();
		string hlh = (health < 10 ? "00" : (health < 100 ? "0" : "")) + health;

		HealthBar.SetProperty("style", $"width:{(((health>100?100:health)* 268.5) /100)}px;");
		HealthIconValue.Text = hlh.ToString();

		if (health > 20) HealthBar.RemoveClass("ALERT");

		if (HealthIconDelayDown == 0) {
			if (HealthIconUp == false) {
				HealthIcon.AddClass("DOWN");
				HealthIconValue.AddClass("DOWN");
				if (health <= 20) HealthBar.AddClass("ALERT");
			} HealthIconDelayDown++;
		}
		else {
			HealthIconDelayDown++;
			if(HealthIconDelayDown > 20) {
				if(HealthIconUp == false) { HealthIcon.RemoveClass("DOWN"); HealthIconValue.RemoveClass("DOWN"); HealthBar.RemoveClass("ALERT"); }
				HealthIconUp = !HealthIconUp;
				HealthIconDelayDown = 0;
			}
		}
	}
}
