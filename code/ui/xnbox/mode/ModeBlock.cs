using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Xnbox;

public class ModeBlock : Panel
{
	public Panel _this;
	public Panel BLOCKTOP;
	public Button SELECT;
	public Label NBVOTES;
	public Mode currMode { get; set; }
	public int NbVotes { get; set; } = -1;
	public string Percent { get; set; } = "";

	public void SetVotes(int n)
    {
		this.NbVotes = n;
    }

	public void SetPercent(int n,int t)
    {
		this.Percent = ((n <= 0 || t <= 0) ? "0" : ((n * 100) / t).ToString("#.##"));
    }

	public ModeBlock(Panel parent, Mode m)
	{
		currMode = m;
		_ = Add.Label(m.Desc, "DESC");
		SELECT = Add.Button("SELECT", () =>
		{
			SandboxPlayer.ModeSet(m.Name);

		});
		SELECT.AddClass("SELECT");
		BLOCKTOP = Add.Panel("BLOCKTOP");
		_ = BLOCKTOP.Add.Label(m.Icon, "ICON");
		_ = BLOCKTOP.Add.Label(m.Name, "NAME");
		NBVOTES = BLOCKTOP.Add.Label(NbVotes + " Votes", "NBVOTES");
		_this = this;
	}

	public override void Tick()
	{
		NBVOTES.SetClass("HIDE", NbVotes < 0);
		SandboxPlayer player = Local.Pawn as SandboxPlayer;
		if (player == null) return;
		if (player.ShowModeMenu == false) return;

		string GetSelectedMode = player.GetSelectedMode();

		ModeTwitch ModeTwitchPlayer = player.ModeTwitchPlayer;
		bool ModeLockSwitch = player.ModeLockSwitch;
		int ModeLockTimeRemains = player.ModeLockTimeRemains;
		bool VoteStart = (ModeTwitchPlayer != null && ModeTwitchPlayer.VoteStart == true);
		

		SELECT.Text = ModeLockSwitch ? string.Format("WAIT ({0} SEC)", (ModeLockTimeRemains > 9 ? "" : "0") + ModeLockTimeRemains) : "SELECT";
		SELECT.SetClass("LOCK", ModeLockSwitch == true || VoteStart);
		_this.SetClass("SELECTED", GetSelectedMode == currMode.Name || VoteStart);
		if (!VoteStart) return;
		NBVOTES.Text = NbVotes + " Votes (" + Percent + "%)";
	}

}
