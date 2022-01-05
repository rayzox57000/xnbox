using Sandbox;
using System;

partial class SandboxPlayer
{

	public string[] AntiPK = { "prop_physics" };
	[Net] public bool PropKillEnabled { get; set; } = false;
	[Net] public bool PropKillLockSwitch { get; set; } = false;
	public int PropKillLockTime { get; set; } = 120;
	[Net] public int PropKillLockTimeRemains { get; set; } = 0;


	private bool IsCorrectPropPrevention( DamageInfo dmg )
    {
		if (dmg.Attacker == null) return false;
		return Array.Exists(AntiPK, element => element == dmg.Attacker.ClassInfo.Name.ToString());

	}

	private bool AntiPropKill( DamageInfo dmg, bool AlreadyCheckProp = true)
    {
		if (dmg.Attacker == null) return false;

		PropTouch prop = dmg.Attacker as PropTouch;

		if(prop != null)
        {
			SandboxPlayer p = prop.LastTouch as SandboxPlayer;
			if(p != null)
            {
				if (p.PropKillEnabled == false) return true;
			}
			
        }

		bool IsCoorectPropPreventionValue = AlreadyCheckProp == true ? true : IsCorrectPropPrevention(dmg);

		if (IsCoorectPropPreventionValue == true && PropKillEnabled == false) return true;
		return false;
	}
	
	public async void PropKillTempLockSwitchInc(int i)
	{
		if (i == 0) { PropKillLockTimeRemains = 0; return; }
		await Task.Delay(1000);
		i--;
		PropKillLockTimeRemains = i;
		PropKillTempLockSwitchInc(i);
		return;
	}

	public async void PropKillTempLockSwitch()
	{
		PropKillLockTimeRemains = PropKillLockTime;
		PropKillTempLockSwitchInc(PropKillLockTime);
		PropKillLockSwitch = true;
		await Task.Delay(PropKillLockTime * 1000);
		PropKillLockSwitch = false;
	}


	[ServerCmd("propkill_on")]
	public static void SetPropKillModeOn()
	{
		var target = ConsoleSystem.Caller.Pawn as SandboxPlayer;
		if (target == null || target.PropKillEnabled) return;
		if (target.PropKillLockSwitch == true) return;
		target.PropKillEnabled = true;
		target.PropKillTempLockSwitch();
		return;
	}

	[ServerCmd("propkill_off")]

	public static void SetPropKillModeOff()
	{
		var target = ConsoleSystem.Caller.Pawn as SandboxPlayer;
		if (target == null || (!target.PropKillEnabled)) return;
		if (target.PropKillLockSwitch == true) return;
		target.PropKillEnabled = false;
		target.PropKillTempLockSwitch();
		return;
	}

	[ServerCmd("propkill_switch")]

	public static void SetPropKillModeSwitch()
	{
		var target = ConsoleSystem.Caller.Pawn as SandboxPlayer;
		if (target == null) return;
		if (target.PropKillLockSwitch == true) return;
		target.PropKillEnabled = !target.PropKillEnabled;
		target.PropKillTempLockSwitch();
		return;
	}




}
