using System.Linq;
using Sandbox;

public sealed class TrackBullet : Component,Component.ITriggerListener
{

	public GameObject target;
	float speed = 30f * DifficultyRatio.GetDifficultRatio();
	private Vector3 direction;
	
	protected override void OnStart()
	{
		var gameObjects = Scene.GetAllObjects( true ).Where( obj => obj.Tags.Has( "player" ) );
		if ( !gameObjects.Any() ) return;
		var objects = gameObjects.ToList();
		foreach ( var o in objects )
		{
			target = o;
			direction = target.Transform.Position - Transform.Position;
		}
	}
	protected override void OnFixedUpdate()
	{
		Transform.Position += direction.Normal * speed * Time.Delta;
	}


	public void OnTriggerEnter( Collider other )
	{
		GameObject.Destroy();
	}

	public void OnTriggerExit( Collider other )
	{
	}
}
