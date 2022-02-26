using System.Collections.Generic;
using System.Linq;
using Sandbox.Joints;

namespace Sandbox.Tools
{
	[Library("tool_rope", Title = "Rope", Description = "Join two things together with a rope", Group = "construction")]
	public partial class RopeTool : BaseTool
	{
		private PhysicsBody targetBody;
		private int targetBone;
		private Vector3 localOrigin1;
		private Vector3 globalOrigin1;

		public override void Simulate()
		{
			if (!Host.IsServer)
				return;

			using (Prediction.Off())
			{
				if (!Input.Pressed(InputButton.Attack1))
					return;

				var startPos = Owner.EyePosition;
				var dir = Owner.EyeRotation.Forward;

				SandboxPlayer player = Owner as SandboxPlayer;
				if (player == null) return;

				var tr = Trace.Ray(startPos, startPos + dir * MaxTraceDistance)
					.Ignore(Owner)
					.Run();

				if (!tr.Hit)
					return;

				if (!tr.Body.IsValid())
					return;

				if (!tr.Entity.IsValid())
					return;

				if (tr.Entity is not ModelEntity)
					return;

				if ((!tr.Entity.IsWorld) && tr.Entity.Owner != Owner)
					return;

				if (!targetBody.IsValid())
				{
					targetBody = tr.Body;
					targetBone = tr.Bone;
					globalOrigin1 = tr.EndPosition;
					localOrigin1 = tr.Body.Transform.PointToLocal( globalOrigin1 );

					CreateHitEffects( tr.EndPosition );

					return;
				}

				if (targetBody == tr.Body)
					return;

				var rope = Particles.Create("particles/rope.vpcf");

				if ( targetBody.GetEntity().IsWorld )
				{
					rope.SetPosition(0, localOrigin1);
				}
				else
				{
					rope.SetEntityBone( 0, targetBody.GetEntity(), targetBone, new Transform( localOrigin1 * (1.0f / targetBody.GetEntity().Scale) ) );
				}

				var localOrigin2 = tr.Body.Transform.PointToLocal( tr.EndPosition );

				if (tr.Entity.IsWorld)
				{
					rope.SetPosition(1, localOrigin2);
				}
				else
				{
					rope.SetEntityBone( 1, tr.Body.GetEntity(), tr.Bone, new Transform( localOrigin2 * (1.0f / tr.Entity.Scale) ) );
				}

				var spring = PhysicsJoint.CreateLength( targetBody.LocalPoint( localOrigin1 ), tr.Body.LocalPoint( localOrigin2 ), tr.EndPosition.Distance( globalOrigin1 ) );
				spring.SpringLinear = new( 5, 0.7f );
				spring.Collisions = true;
				spring.EnableAngularConstraint = false;
				spring.OnBreak += () =>
				{
					rope?.Destroy(true);
					spring.Remove();
				};

				player.AddCustomUndo("ROPE", RemoveRope.Remove, spring, rope);

				CreateHitEffects( tr.EndPosition );

				Reset();
			}
		}

		private void Reset()
		{
			targetBody = null;
			targetBone = -1;
			localOrigin1 = default;
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

	class RemoveRope
	{
		public static bool Remove(List<object> objs)
		{
			if (objs != null && objs.Count <= 2)
			{
				if (objs[0] is SpringJoint)
				{
					Particles p = objs[1] as Particles;
					
					SpringJoint s = (SpringJoint)objs.First();
					if (p != null)
					{
						p?.Destroy(true);
						s.Remove();
						return true;
					}
				}
			}
			return false;
		}
	}


}
