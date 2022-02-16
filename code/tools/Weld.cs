using System.Collections.Generic;
using System.Linq;
	
	namespace Sandbox.Tools
{
	[Library( "tool_weld", Title = "Weld", Description = "Weld stuff together", Group = "construction" )]
	public partial class WeldTool : BaseTool
	{
		private Prop target;

		public override void Simulate()
		{
			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{
				var startPos = Owner.EyePosition;
				var dir = Owner.EyeRotation.Forward;

				SandboxPlayer player = Owner as SandboxPlayer;
				if (player == null) return;

				var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
					.Ignore( Owner )
					.Run();

				if ( !tr.Hit || !tr.Body.IsValid() || !tr.Entity.IsValid() || tr.Entity.IsWorld )
					return;

				if ( tr.Entity.PhysicsGroup == null || tr.Entity.PhysicsGroup.BodyCount > 1 )
					return;

				if ( tr.Entity is not Prop prop )
					return;

				if (tr.Entity.Owner != Owner)
					return;

				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					if ( prop.Root is not Prop rootProp )
					{
						return;
					}

					if ( target == rootProp )
						return;

					if ( !target.IsValid() )
					{
						target = rootProp;
					}
					else
					{
						target.Weld( rootProp );
						player.AddCustomUndo("WELD", RemoveWeld.Remove, target, rootProp);
						target = null;
					}
				}
				else if ( Input.Pressed( InputButton.Attack2 ) )
				{
					prop.Unweld( true );

					Reset();
				}
				else if ( Input.Pressed( InputButton.Reload ) )
				{
					if ( prop.Root is not Prop rootProp )
					{
						return;
					}

					rootProp.Unweld();

					Reset();
				}
				else
				{
					return;
				}

				CreateHitEffects( tr.EndPos );
			}
		}

		private void Reset()
		{
			target = null;
		}

		public override void Activate()
		{
			base.Activate();

			Reset();
		}

		public override void Deactivate()
		{
			base.Deactivate();

			Reset();
		}
	}

	class RemoveWeld
	{
		public static bool Remove(List<object> objs)
		{
			if (objs != null && objs.Count == 2)
			{
				Prop e = objs.First() as Prop;
				Prop root = objs.Last() as Prop;

				if (e != null && root != null && e.IsValid() && root.IsValid())
				{
					root.Unweld(true,e);
					return true;
				}

			}
			return false;
		}
	}

}
