using System;
using Sandbox;

public sealed class RotationBullet : Component,Component.ITriggerListener
{

	public float speed = 25f * DifficultyRatio.GetDifficultRatio();  // 子弹速度
	public Vector3 direction = Vector3.Backward;  // 子弹运动方向
	[Property] public GameObject Bulletprefab { get; set; }
	[Property] public SoundEvent FireSound { get; set; }
	public int numberOfBullets = (int)(12 * DifficultyRatio.GetDifficultRatio());  // 子弹数量
	public float circleRadius = 5f ;  // 子弹生成的圆的半径
	public float spawntime = 0;
	
	protected override void OnUpdate()
	{
		Transform.Rotation = Transform.Rotation.RotateAroundAxis( Vector3.Up, 0.4f );
		Transform.Position += (direction * speed * Time.Delta);
		spawntime += Time.Delta;
		if ( spawntime > 0.2f )
		{
			SpawnBulletCircle();
			spawntime = 0;
		}
	}

	void SpawnBulletCircle()
	{
		// 生成一个在0到359之间的随机整数
		Random random = new Random();
		int randomInt = random.Next(0, 60);
		
		for (int i = 0; i < numberOfBullets; i++)
		{
			
			// 计算子弹在圆上的位置
			float angle = i * (360f / numberOfBullets) + randomInt;
			Vector3 spawnPosition = Transform.Position + new Vector3((float)Math.Cos(MathX.DegreeToRadian(angle)), (float)Math.Sin(MathX.DegreeToRadian( angle )),0) * circleRadius;
			// 生成子弹
			if(Bulletprefab is null)return;
			GameObject bulletInstance = Bulletprefab.Clone( spawnPosition );
			Bullet bullet = bulletInstance.Components.Get<Bullet>();

			// 设置子弹方向
			bullet.direction = (spawnPosition - (Vector3)Transform.Position).Normal;
		}

		if ( FireSound is not null ) Sound.Play( FireSound );
	}
	public void OnTriggerEnter( Collider other )
	{
		GameObject.Destroy();
	}

	public void OnTriggerExit( Collider other )
	{
	
	}
}
