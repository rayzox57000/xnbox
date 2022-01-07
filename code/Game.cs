using Sandbox;
using System.Collections.Generic;
using System.Linq;
using Xnbox;

partial class SandboxGame : Game
{
	public SandboxGame()
	{
		if ( IsServer )
		{
			// Create the HUD
			_ = new SandboxHud();
		}
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );
		var player = new SandboxPlayer( cl );
		player.Respawn();

		cl.Pawn = player;
	}

	public override void ClientDisconnect(Client cl, NetworkDisconnectionReason reason)
	{

		SandboxPlayer p = cl.Pawn as SandboxPlayer;

		if (p != null) p.ModeCounterStop();

		base.ClientJoined(cl);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	[ServerCmd( "spawn" )]
	public static void Spawn( string modelname )
	{
		var owner = ConsoleSystem.Caller?.Pawn;
		SandboxPlayer player = owner as SandboxPlayer;

		if ( ConsoleSystem.Caller == null )
			return;
		if (player == null)
			return;

		var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 500 )
			.UseHitboxes()
			.Ignore( owner )
			.Run();

		var ent = new PropTouch();

		ent.Owner = owner;
		ent.OwnerSpawn = owner;
		ent.Position = tr.EndPos;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );
		ent.SetModel( modelname );
		ent.Position = tr.EndPos - Vector3.Up * ent.CollisionBounds.Mins.z;

		player.AddCustomUndo("PROP", null, ent as Entity);

	}

	[ServerCmd( "spawn_entity" )]
	public static void SpawnEntity( string entName )
	{
		var owner = ConsoleSystem.Caller.Pawn;

		if ( owner == null )
			return;

		SandboxPlayer player = owner as SandboxPlayer;
		if(player == null)
			return;

		var attribute = Library.GetAttribute( entName );
		Log.Info(attribute.Name);

		if ( attribute == null || !attribute.Spawnable )
			return;

		if (player.SelectedModeString != Mode.PvpMode.Name && attribute.Name.StartsWith("weapon_"))
		{
			Log.Info("CANT SPAWN WEAPON IN NO PVP MODE");
			return;
		}

		var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 200 )
			.UseHitboxes()
			.Ignore( owner )
			.Size( 2 )
			.Run();

		var ent = Library.Create<Entity>( entName );
		if ( ent is BaseCarriable && owner.Inventory != null )
		{
			if ( owner.Inventory.Add( ent, true ) )
				return;
		}

		ent.Owner = owner;
		ent.Position = tr.EndPos;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) );

		player.AddCustomUndo("ENTITY", null, ent as Entity);

		//Log.Info( $"ent: {ent}" );
	}

	[ServerCmd("undo_spawn")]
	public static void UndoSpawn()
	{
		var owner = ConsoleSystem.Caller.Pawn;

		if (owner == null)
			return;

		SandboxPlayer player = owner as SandboxPlayer;
		if (player == null)
			return;

		player.UndoObject();

	}

	public override void DoPlayerNoclip( Client player )
	{
		if ( player.Pawn is Player basePlayer )
		{
			if ( basePlayer.DevController is NoclipController )
			{
				Log.Info( "Noclip Mode Off" );
				basePlayer.DevController = null;
			}
			else
			{
				Log.Info( "Noclip Mode On" );
				basePlayer.DevController = new NoclipController();
			}
		}
	}

	[ClientCmd( "debug_write" )]
	public static void Write()
	{
		ConsoleSystem.Run( "quit" );
	}
}
