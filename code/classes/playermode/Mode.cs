using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xnbox
{

	public class Mode : IEquatable<Mode>
	{
		private static readonly Dictionary<string, Mode> allModes;
		
		public static Mode GetModeById(string Name)
		{
			if (!allModes.ContainsKey(Name)) throw new KeyNotFoundException();
			return allModes[Name];
		}
		public static Mode[] GetAllModes() => allModes.Values.ToArray();

		public static Mode IdleMode;
		public static Mode BuildMode;
		public static Mode PvpMode;

		static Mode()
		{
			allModes = new Dictionary<string, Mode>();
			IdleMode = new Mode("Idle", "", 0.0f, false, false, false, false, false);
			BuildMode = new Mode("Build", "🏗", 0.0f, false, false,true, true, true, "Be at peace! Build without fear 😊");
			PvpMode = new Mode("PvP", "⚔️", 1.0f, true, true,true, false, true, "Attack! Looking for some action 💪");
		}

		public string Name { get; } = "Base";
		public string Icon { get; } = "";
		public string Desc { get; } = "No Description";
		public double DamageMultiplicator { get; } = 1.0f;
		public bool CanUseWeapon { get; } = true;
		public bool CanTakeDamage { get; } = true;
		public bool CanMove { get; } = true;
		public bool CanNoClip { get; } = false;
		public bool ShowInMenu { get; } = true;
		

		public Mode(string name, string icon, double damageMultiplicator = 1.0f, bool canUseWeapon = true, bool canTakeDamage = true, bool canMove = false, bool canNoclip = false, bool showInMenu = true, string desc = "No Description.")
		{
			Name = name;
			Icon = icon;
			DamageMultiplicator = damageMultiplicator;
			CanUseWeapon = canUseWeapon;
			CanTakeDamage = canTakeDamage;
			CanMove = canMove;
			CanNoClip = canNoclip;
			ShowInMenu = showInMenu;
			Desc = desc;
			allModes.Add(Name, this);
		}

		public override string ToString() => Name;

		public bool Equals(Mode other) => Name.Equals(other.Name);

	}
}
