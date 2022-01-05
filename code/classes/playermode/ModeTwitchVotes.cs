using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public partial class ModeTwitchVotes : BaseNetworkable
{
	public string Mode { get; set; } = "";
	[Net] public int Votes { get; set; } = 0;


	public ModeTwitchVotes(string ModeName)
    {
		Mode = ModeName;
		Votes = 0;
    }

	public void AddVote()
    {
		Votes++;
    }

}
