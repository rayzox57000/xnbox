using System;
using System.Threading.Tasks;

namespace Sandbox
{
	/// <summary>
	/// A prop that physically simulates as a single rigid body. It can be constrained to other physics objects using hinges
	/// or other constraints. It can also be configured to break when it takes enough damage.
	/// Note that the health of the object will be overridden by the health inside the model, to ensure consistent health game-wide.
	/// If the model used by the prop is configured to be used as a prop_dynamic (i.e. it should not be physically simulated) then it CANNOT be
	/// used as a prop_physics. Upon level load it will display a warning in the console and remove itself. Use a prop_dynamic instead.
	/// </summary>
	[Library("prop_physics")]
	[Hammer.Model]
	[Hammer.RenderFields]
	[Hammer.VisGroup(Hammer.VisGroup.Physics)]
	public partial class PropTouch : Prop
	{
		[Net] public Entity LastTouch { get; set; }
		[Net] public Entity OwnerSpawn { get; set; }

		public void setLastGrab(Entity owner)
		{
			if (IsServer)
			{
				if (owner as SandboxPlayer != null)
				{
					Log.Info("GRAB UPDATE");
					LastTouch = owner;
				}
			}

		}
	}
}
