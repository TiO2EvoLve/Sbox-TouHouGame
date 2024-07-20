using System;
using Sandbox;

public sealed class ScaleBullet : Component,Component.ITriggerListener
{
	
	public float speed = 10f * DifficultyRatio.GetDifficultRatio();  // 子弹速度
	public Vector3 direction = Vector3.Backward;  // 子弹运动方向
	private TimeSince time;

	protected override void OnStart()
	{
		time = 0;
	}

	protected override void OnUpdate()
	{
		Transform.Position += (direction * speed * Time.Delta);
		Transform.Scale = Math.Abs((float)time);
		if ( time > 5 ) time = -5;
	}
	
	public void OnTriggerEnter( Collider other )
	{
		GameTags tag = other.GameObject.Tags;

		if ( tag.Has( "not" ) ) return;
		GameObject.Destroy();
	}

	public void OnTriggerExit( Collider other )
	{
	
	}
}
