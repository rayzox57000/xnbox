using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xnbox;
using Sandbox;

public partial class ModeTwitch : BaseNetworkable
	{
	public List<string> VotesNames { get; set; } = new List<string>();
	[Net] public Dictionary<string, int> Votes { get; set; } = new Dictionary<string, int>();
	[Net] public int TotalVotes { get; set; } = 0;
	[Net] public int VoteTimeDefault { get; set; } = 60;
	[Net] public int VoteTimeRemains { get; set; } = 0;
	[Net] public bool VoteStart { get; set; } = false;

	[Net] public bool CheckAlreadyVote { get; set; } = false;

	public static CancellationTokenSource source { get; set; } = new CancellationTokenSource();
	public static CancellationToken token { get; set; } = source.Token;

	public bool AlreadyVote(string p)
		{
			if (CheckAlreadyVote == false) return false;
			return VotesNames.Where(x => x.Contains(p)).FirstOrDefault() != null;
		}

	public void NewTokenSource()
    {
		source = new CancellationTokenSource();
		token = source.Token;
	}

	public void Reset()
    {
		NewTokenSource();
		TotalVotes = 0;
		VoteTimeRemains = 0;
		VoteStart = false;
		Votes = new Dictionary<string, int>();
		VotesNames = new List<string>();
		foreach (Mode mode in Mode.GetAllModes())
		{
			if (mode.ShowInMenu == true)
			{
				Votes.Add(mode.Name, 0);
			}
		}
	}

	public void VoteEndWinner(Sandbox.Entity p)
		{
			string winner = "";
			int max = 0;

			foreach (KeyValuePair<string, int>item in Votes)
			{
				Log.Info(item.Value); if (item.Value >= max) { winner = item.Key; max = item.Value; }
			}

			Log.Info("Le public à choisi : " + winner);

			if (Mode.GetModeById(winner) != null)
			{
				SandboxPlayer xp = p as SandboxPlayer;
				if (xp != null)
				{
					xp.SetSelectedMode(winner);
					xp.ForceRespawn();
				}
			}
		Reset();
		}


		public async void ModeTwitchTempStartSwitchInc(int i)
		{
			if (i == 0) { VoteTimeRemains = 0; return; }
			try { 
				await Task.Delay(1000, token);
				i--;
				VoteTimeRemains = i;
				ModeTwitchTempStartSwitchInc(i);
		}
			catch (Exception e)
			{
				Log.Info("TASK CANCEL !");
			}
			
			
			return;
		}

		public async void ModeTwitchTempStartSwitch(Sandbox.Entity p)
		{
			NewTokenSource();
			VoteTimeRemains = VoteTimeDefault;
			ModeTwitchTempStartSwitchInc(VoteTimeDefault);
			VoteStart = true;
			try
			{
				await Task.Delay(VoteTimeDefault * 1000, token);
				VoteStart = false;
				VoteEndWinner(p);

			}
			catch(Exception e)  { }
			
		}

		public ModeTwitch()
		{
			Reset();
		}

		public bool Start(Sandbox.Entity p)
        {
			if (VoteStart == true) return false;
			VotesNames = new List<string>();
			VoteStart = true;
			TotalVotes = 0;
			
			ModeTwitchTempStartSwitch(p);
			return true;
		}

		public void Stop()
		{
			if (VoteStart == false) return;
			source.Cancel();
			Reset();
		}

		public void NewVote(string m,string name)
		{
			VotesNames.Add(name);
			foreach (KeyValuePair<string, int> item in Votes)
			{
				if (m.ToLower() == item.Key.ToLower()) { Votes[item.Key] += 1; TotalVotes += 1; }
			}
		}

	}
