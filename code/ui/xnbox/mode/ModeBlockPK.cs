using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Xnbox;

public class ModeBlockPK : Panel
{
	public Panel _this;
	public Panel BLOCKTOP;
	public Button SELECT;

	public Label LABEL;
	public ModeBlockPK(Panel parent)
	{
		_ = Add.Label("You can propkill people who have ON this option ✌", "DESC");
		SELECT = Add.Button("", () => {
			SandboxPlayer.SetPropKillModeSwitch();
			
		});
		SELECT.AddClass("SELECT");
		BLOCKTOP = Add.Panel("BLOCKTOP");
		_ = BLOCKTOP.Add.Label("📦","ICON");
		LABEL = BLOCKTOP.Add.Label("PROPKILL : OFF","NAME");
		_this = this;
	}

	public override void Tick()
	{
		SandboxPlayer player = Local.Pawn as SandboxPlayer;
		if (player == null) return;
		if (player.ShowModeMenu == false) return;

		bool PropKillLockSwitch = player.PropKillLockSwitch;
		int PropKillLockTimeRemains = player.PropKillLockTimeRemains;
		bool PropKillEnabled = player.PropKillEnabled;

		SELECT.Text = PropKillLockSwitch ? string.Format("WAIT ({0} SEC)",(PropKillLockTimeRemains > 9 ? "" : "0") + PropKillLockTimeRemains) : (PropKillEnabled ? "DISABLED" : "ENABLED");
		SELECT.SetClass("LOCK", PropKillLockSwitch);
		LABEL.Text = "PROPKILL : " + (PropKillEnabled ? "ON" : "OFF");
	}
}
