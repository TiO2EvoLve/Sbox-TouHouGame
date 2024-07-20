using Sandbox;

public sealed class JumpBullet : Component, Component.ITriggerListener
{
	public float speed = 25f * DifficultyRatio.GetDifficultRatio(); // 子弹速度
	public Vector3 direction = Vector3.Backward; // 子弹运动方向
	[Property] public int MaxJump { set; get; } = 3; //最大弹跳次数

	private int JumpNum = 0; //弹跳次数

	protected override void OnFixedUpdate()
	{
		Transform.Position += (direction * speed * Time.Delta);
		if ( JumpNum > MaxJump ) { GameObject.Destroy(); }
	}

	public void OnTriggerEnter( Collider other )
	{
		JumpNum++;
		GameTags tag = other.GameObject.Tags;
		if ( tag.Has( "solid" ) &&tag.Has( "y" ))
		{
			Vector3 normal = other.GameObject.Transform.Rotation * Vector3.Left; // 获取碰撞物体的法线
			direction = Vector3.Reflect(direction, normal); // 计算反射方向
			//direction = -direction;
		}else if ( tag.Has( "solid" ) )
		{
			Vector3 normal = other.GameObject.Transform.Rotation * Vector3.Forward; // 获取碰撞物体的法线
			direction = Vector3.Reflect(direction, normal); // 计算反射方向
		}
	}

	public void OnTriggerExit( Collider other )
	{
	}
}
