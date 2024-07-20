using System.Linq;
using Sandbox;

public sealed class AccelerationBullet : Component,Component.ITriggerListener
{
	public float speed = 25f * DifficultyRatio.GetDifficultRatio();  // 子弹速度
	public Vector3 direction = Vector3.Backward;  // 子弹运动方向
	public GameObject Player;//玩家物体
	public float distance;
	
	protected override void OnStart()
	{
		var gameObjects = Scene.GetAllObjects( true ).Where( obj => obj.Tags.Has( "player" ) );
		if ( !gameObjects.Any() ) return;
		var objects = gameObjects.ToList();
		foreach ( var o in objects )
		{
			Player = o;
		}
	}
	protected override void OnFixedUpdate()
	{
		Transform.Position += (direction * speed * Time.Delta);
		distance = Transform.Position.Distance( Player.Transform.Position );
		if ( distance < 20f )
		{
			speed = distance;
		}
	}
	public void OnTriggerEnter( Collider other )
	{
		GameObject.Destroy();
	}

	public void OnTriggerExit( Collider other )
	{
	
	}
}
