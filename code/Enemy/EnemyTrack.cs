using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;

public sealed class EnemyTrack : Component, Component.ITriggerListener
{
	TimeSince fire = 0;//计时器
	float firetime = 1f / DifficultyRatio.GetDifficultRatio();//攻击间隔
	[Property] public GameObject bullet { get; set; }//子弹1
	[Property] public GameObject bullet2 { get; set; }//子弹1
	[Property] public GameObject bullet3 { get; set; }//子弹2
	[Property] public BossHealth BossHealth { get; set; }//血条
	[Property] public SoundEvent bulletSound { get; set; }//发射子弹声音
	
	[Property] public SoundEvent DeadSound { get; set; }//死亡声音
	[Property] public float Health { get; set; }//生命值

	public int numberOfBullets = (int)(12 * DifficultyRatio.GetDifficultRatio()); // 子弹数量
	public float circleRadius = 5f; // 子弹生成的圆的半径

	public bool isAttacked;//能否被攻击
	public int AttackedType;//攻击方式

	private Collider _collider;

	protected override void OnStart()
	{
		_collider = Components.Get<Collider>();
		_collider.Enabled = false;
	}

	protected override void OnUpdate()
	{
		//如果可以被攻击，开启血条，攻击方式改为1
		if ( isAttacked )
		{
			BossHealth.Enabled = true;
			AttackedType = 1;
			_collider.Enabled = true;
		}
		//如果生命值低于某值，攻击方式改为2
		if ( Health < 50 )
		{
			AttackedType = 2;
		}
		//判断攻击方式
		switch ( AttackedType )
		{
			case 0:
				Attacked1();
				break;
			case 1:
				Attacked2();
				break;
			case 2:
				Attacked3();
				break;
			default: break;
		}
		
		if ( BossHealth is null ) return;
		BossHealth.bosshealth = Health;
		if ( Health <= 0 )
		{
			if ( bulletSound is not null ) Sound.Play( DeadSound);
			DestroyAll();
			GameObject.Destroy();
			BossHealth.Enabled = false;
		}
	}

	void Attacked1()
	{
		if ( fire > firetime )
		{
			if ( bulletSound is not null ) Sound.Play( bulletSound );
			bullet.Clone( Transform.Position );
			fire = 0;
		}
	}

	void Attacked2()
	{
		if ( fire > firetime / 5 )
		{
			fire = 0;
			// 生成一个随机整数
			Random random = new Random();
			int randomInt = random.Next( 0, 90 );

			for ( int i = 0; i < numberOfBullets; i++ )
			{
				// 计算子弹在圆上的位置
				float angle = i * (360f / numberOfBullets) + randomInt;
				Vector3 spawnPosition = Transform.Position + new Vector3(
					(float)Math.Cos( MathX.DegreeToRadian( angle ) ),
					(float)Math.Sin( MathX.DegreeToRadian( angle ) ), 0 ) * circleRadius;
				// 生成子弹
				if ( bullet2 is null ) return;
				GameObject bulletInstance = bullet2.Clone( spawnPosition );
				Bullet bullet = bulletInstance.Components.Get<Bullet>();
				// 设置子弹方向
				bullet.direction = (spawnPosition - (Vector3)Transform.Position).Normal;
			}
		}
	}

	void Attacked3()
	{
		if ( fire > firetime )
		{
			fire = 0;
			// 生成一个随机整数
			Random random = new Random();
			int randomInt = random.Next( 0, 30 );

			for ( int i = 0; i < numberOfBullets; i++ )
			{
				// 计算子弹在圆上的位置
				float angle = i * (360f / numberOfBullets) + randomInt;
				Vector3 spawnPosition = Transform.Position + new Vector3(
					(float)Math.Cos( MathX.DegreeToRadian( angle ) ),
					(float)Math.Sin( MathX.DegreeToRadian( angle ) ), 0 ) * circleRadius;
				// 生成子弹
				if ( bullet3 is null ) return;
				if ( bulletSound is not null ) Sound.Play( bulletSound );
				GameObject bulletInstance = bullet3.Clone( spawnPosition );
				JumpBullet bullet = bulletInstance.Components.Get<JumpBullet>();
				// 设置子弹方向
				bullet.direction = (spawnPosition - (Vector3)Transform.Position).Normal;
			}
		}
	}
	public void OnTriggerEnter( Collider other )
	{
		if ( !isAttacked ) return;
		GameTags tag = other.GameObject.Tags;
		if ( tag.Has( "playerbullet" ) )
		{
			Health -= 0.2f;
		}
	}

	public void OnTriggerExit( Collider other )
	{
	}
	
	void DestroyAll()
	{
		var gameObjects = Scene.GetAllObjects( true ).Where( obj => obj.Tags.Has( "bullet" ));
		if ( !gameObjects.Any() ) return;
		var objects = gameObjects.ToList();
		foreach ( var o in objects )
		{
			o.Destroy();
		}

		var gameObjects2 = Scene.GetAllObjects( true ).Where( obj => obj.Tags.Has( "enemy" ) );
		if ( !gameObjects2.Any() ) return;
		var objects2 = gameObjects2.ToList();
		foreach ( var o in objects2 )
		{
			o.Destroy();
		}
	}
}
