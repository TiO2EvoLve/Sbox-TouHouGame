using Sandbox;

public sealed class Bullet : Component,Component.ITriggerListener
{
	
	public float speed = 25f * DifficultyRatio.GetDifficultRatio();  // 子弹速度
	public Vector3 direction = Vector3.Backward;  // 子弹运动方向
	
	protected override void OnStart()
	{
		
	}
	
	protected override void OnFixedUpdate()
	{
		Transform.Position += (direction * speed * Time.Delta);
	}
	public void OnTriggerEnter( Collider other )
	{
		GameObject.Destroy();
	}

	public void OnTriggerExit( Collider other )
	{
	
	}

	
}
