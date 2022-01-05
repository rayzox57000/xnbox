using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xnbox;

partial class SandboxPlayer
{
	[Net] public string SelectedModeString { get; set; } = "Build";
	[Net] public bool ShowModeMenu { get; set; } = true;

	public static int ModeLockTime = 60;
	[Net] public int ModeLockTimeRemains { get; set; } = 0;
	[Net] public bool ModeLockSwitch { get; set; } = false;

	[Net] public int Counter { get; set; } = 0;

	[Net] public ModeTwitch ModeTwitchPlayer { get; set; } = new ModeTwitch();

	public static CancellationTokenSource Modesource { get; set; } = new CancellationTokenSource();
	public static CancellationToken ModeToken { get; set; } = Modesource.Token;

	public Mode GetCurrentModeByName()
	{
		try
		{
			return Mode.GetModeById(SelectedModeString);
		}
		catch
		{
			return Mode.IdleMode;
		}
	}

	public string GetSelectedMode()
	{
		return SelectedModeString;
	}

	public void SetSelectedMode(string modeStr)
	{
		try
		{
			Mode mode = Mode.GetModeById(modeStr);
			SelectedModeString = modeStr;
			ModeTempLockSwitch();
			ShowModeMenu = false;
			ForceRespawn();
		}
		catch { Log.Error("MODE NOT FOUND [BAD KEY]"); }
		return;
	}

	public async void ModeTempLockSwitchInc(int i)
	{
		if (i == 0) { ModeLockTimeRemains = 0; return; }
		await Task.Delay(1000);
		i--;
		ModeLockTimeRemains = i;
		ModeTempLockSwitchInc(i);
		return;
	}

	public async void ModeTempLockSwitch()
	{
		ModeLockTimeRemains = ModeLockTime;
		ModeTempLockSwitchInc(ModeLockTime);
		ModeLockSwitch = true;
		await Task.Delay(ModeLockTime * 1000);
		ModeLockSwitch = false;
	}

	[ServerCmd("mode_set")]
	public static void ModeSet(string name)
	{
		var target = ConsoleSystem.Caller.Pawn as SandboxPlayer;
		if (target == null) return;
		if (target.ModeLockSwitch == true) return;

		target.SetSelectedMode(name);
	}

	[ServerCmd("mode_change")]
	public static void ModeChange()
	{
		var target = ConsoleSystem.Caller.Pawn as SandboxPlayer;
		if (target == null) return;
		target.ShowModeMenu = true;
	}

	[ServerCmd("mode_change_close")]
	public static void ModeChangeClose()
	{
		var target = ConsoleSystem.Caller.Pawn as SandboxPlayer;
		if (target == null) return;
		if (target.SelectedModeString == null) return;
		if (target.ModeTwitchPlayer.VoteStart == true) return;
		target.ShowModeMenu = false;
	}

	[ServerCmd("mode_start_vote")]
	public static void ModeStartVote()
	{
		var target = ConsoleSystem.Caller.Pawn as SandboxPlayer;
		if (target == null) return;
		target.Counter++;
		if (target.ModeLockSwitch == true) return;
		target.ModeTwitchPlayer.Start(ConsoleSystem.Caller.Pawn);
	}

	[ServerCmd("mode_stop_vote")]
	public static void ModeStopVote()
    {
		var target = ConsoleSystem.Caller.Pawn as SandboxPlayer;
		if (target == null) return;
		target.Counter = 0;
		target.ModeTwitchPlayer.Stop();
	}

	[ServerCmd("mode_new_vote")]
	public static void ModeNewVote(string user, string vote)
	{
		var target = ConsoleSystem.Caller.Pawn as SandboxPlayer;
		if (target == null) return;
		if (target.ModeTwitchPlayer.VoteStart == false) return;
		if (target.ModeTwitchPlayer.AlreadyVote(user) == true) return;
		target.ModeTwitchPlayer.NewVote(vote,user);
	}

	public void ModeCounterStop()
	{
		Modesource.Cancel();
	}



	[Event.Streamer.ChatMessage]
	public static void OnMessage(StreamChatMessage message)
	{
		SandboxPlayer p = Local.Pawn as SandboxPlayer;
		if (p == null) return;
		if (p.ModeTwitchPlayer.VoteStart == false) return;
		string m = message.Message.Trim();
		if (!m.StartsWith("!")) return;
		string vote = m.Replace("!", "");
		ModeNewVote(message.DisplayName, vote);
	}
}
