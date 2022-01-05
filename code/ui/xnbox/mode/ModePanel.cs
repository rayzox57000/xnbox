using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Xnbox;

public class ModePanel : Panel
{
	public static Panel _this;
	public Panel Top;
	public Panel Bottom;
	public Button Close;

	public Button Vote;
	public Button CVote;
	
	public List<ModeBlock> ListModeBlock = new List<ModeBlock>();

	public Label VoteTime;

	public void SetCenterFlex(Panel e, bool col = false)
    {
		e.AddClass(string.Format("CENTER_FLEX {0}",col == true ? " COL" : ""));
	}

	public ModePanel()
	{
		StyleSheet.Load("/ui/xnbox/mode/Mode.scss");
		SetCenterFlex(this,true);
		
		Top = this.Add.Panel("TOP");
		SetCenterFlex(Top,true);
		SetCenterFlex(Top.Add.Label("SELECT MODE"),true);

		Vote = Top.Add.Button("TWITCH VOTE", () => {
			SandboxPlayer.ModeStartVote();
		});

		CVote = Top.Add.Button("STOP VOTE", () =>
		{
			SandboxPlayer.ModeStopVote();
		});

		SetCenterFlex(Vote, true);
		SetCenterFlex(CVote, true);

		SetCenterFlex(Vote, true);
		Vote.AddClass("VOTE");
		CVote.AddClass("CVOTE");

		Bottom = this.Add.Panel("BOTTOM");
		Close = Bottom.Add.Button("CLOSE", () => {
			SandboxPlayer.ModeChangeClose();
		});
		VoteTime = Bottom.Add.Label("", "VOTETIME");
		SetCenterFlex(VoteTime, true);
		SetCenterFlex(Bottom);
		Close.AddClass("CLOSE_BTN");
		SetCenterFlex(Close);

		foreach (Mode mode in Mode.GetAllModes())
        {
			if (mode.ShowInMenu == true)
			{
				ModeBlock mb = new ModeBlock(this,mode);
				SetCenterFlex(mb);
				SetCenterFlex(mb.BLOCKTOP,true);
				SetCenterFlex(mb.SELECT, true);
				Bottom.AddChild(mb);
				ListModeBlock.Add(mb);
			}
		}

		ModeBlockPK mbk = new ModeBlockPK(this);
		SetCenterFlex(mbk);
		SetCenterFlex(mbk.BLOCKTOP,true);
		SetCenterFlex(mbk.SELECT, true);
		Bottom.AddChild(mbk);

		_this = this;



	}

	public override void Tick()
	{
		SandboxPlayer player = Local.Pawn as SandboxPlayer;
		if (player == null) return;

		bool MenuOpen = player.ShowModeMenu;
		_this.SetClass("HIDE", !MenuOpen);
		if (MenuOpen == false) return;

		ModeTwitch ModeTwitchPlayer = player.ModeTwitchPlayer;
		string SelectedModeString = player.SelectedModeString;
		bool MenuVoteStart = ModeTwitchPlayer.VoteStart;
		int MenuVoteRemains = ModeTwitchPlayer.VoteTimeRemains;


		Close.SetClass("HIDE", SelectedModeString == Mode.IdleMode.Name || SelectedModeString == "" || SelectedModeString == null || MenuVoteStart == true);
		Vote.SetClass("HIDE", player.ModeLockSwitch == true || MenuVoteStart == true);
		CVote.SetClass("HIDE", player.ModeLockSwitch == true || MenuVoteStart == false);
		VoteTime.SetClass("HIDE", MenuVoteStart == false);
		VoteTime.Text = MenuVoteStart ? $"Voting ends in {MenuVoteRemains} seconds" : "";

		if(MenuVoteStart == true)
        {
			int MenuVoteTimeMax = ModeTwitchPlayer.VoteTimeDefault;
			string Percent = ((MenuVoteRemains * 100) / MenuVoteTimeMax).ToString("#.##");
			VoteTime.SetProperty("style", $"background: linear-gradient(to right, rgba(224,224,224,0.3) 0%, rgba(224,224,224,0.3) {Percent}%, rgba(14,14,14,0.7) {Percent}%);");

		}

		IDictionary<string, int> votes = new Dictionary<string, int>();

		if (MenuVoteStart == true || ModeTwitchPlayer == null || ModeTwitchPlayer.Votes == null) votes = ModeTwitchPlayer.Votes;

		foreach (ModeBlock mb in ListModeBlock)
		{
			int currvote = -1;
			if (MenuVoteStart == true)
			{
				foreach (KeyValuePair<string, int> item in votes) { if (item.Key == mb.currMode.Name) currvote = item.Value; }
				if (currvote > -1) mb.SetPercent(currvote, ModeTwitchPlayer.TotalVotes);
			}
			mb.SetVotes(currvote);
		}


	}



}
