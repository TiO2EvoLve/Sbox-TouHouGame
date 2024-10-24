using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.Diagnostics;
using Sandbox.UI;

public sealed class TouhouPlayer : Component, Component.ITriggerListener
{
	
	[Property] public GameObject bullet { get; set; } //子弹
	[Property] public GameManage GameManage { get; set; } //游戏管理器
	[Property] public Dead EndUI { get; set; } //游戏结束菜单
	[Property] public GameObject point { get; set; } //判定点
	[Property] public float spawnInterval { get; set; } = 0.1f; // 发射间隔时间
	private float lastSpawnTime; //上次生成的时间
	[Property, Range( 0, 8 )] public int health { get; set; } = 2; //生命
	[Property, Range( 0, 8 )] public int spellcard { get; set; } = 2; //符卡
	 public int Score { get; private set; } //得分
	[Property, Range( 0, 4,0.1f )] public float Power { get; set; } //火力值
	
	[Property] public int MaxPoint { get; set; } //得点
	public float speed { get; set; }//速度
	[Property, Range( 0, 20 )] public float moveSpeed { get; set; } = 5f; //正常移动速度
	[Property, Range( 0, 10 )] public float lowSpeed { get; set; } = 2.5f; //慢速移动
	[Property] public float lerpSpeed { get; set; } = 0.5f; //平滑时间
	[Property] public SoundEvent BulletSound { get; set; } //子弹发射声音
	[Property] public SoundEvent Biu { get; set; } //自机被击中音效
	[Property] public SoundEvent SpellCardSound { get; set; } //释放bomb音效
	private TimeSince TimeScore = 0;//时间分

	protected override void OnStart()
	{
		health = GameSettings.Instance.Life;
		spellcard = GameSettings.Instance.Bomb;
	}

	protected override void OnFixedUpdate()
	{
		//计算得分
		Score = (int)((TimeScore * 1000 + MaxPoint * 100000) * DifficultyRatio.GetScoreRatio());
		
		if ( GameSettings.Instance.Bomb != 2 || GameSettings.Instance.Life != 2 ) Score = 0;
		//如果生命为0
		if ( health < 0 )
		{
			//停止游戏
			GameManage.StopGame( Score );
		}
		if ( !GameManage.Playing ) return;
		//移动
		Move();
		//松开鼠标右键
		if ( Input.Released( "attack2" ) && spellcard > 0 )
		{
			if ( spellcard > 0 )
			{
				spellcard--;
				DestroyAll();
				if ( SpellCardSound is not null ) Sound.Play( SpellCardSound );
			}
		}
		//按下鼠标左键
		if ( (Input.Down( "attack1" ) && Time.Now - lastSpawnTime > spawnInterval)
		    || (GameSettings.Instance.AutoShoot&& Time.Now - lastSpawnTime > spawnInterval) )
		{
			lastSpawnTime = Time.Now; // 更新上一次生成的时间
			if ( Power >= 0 && Power < 1 ) { bullet.Clone( WorldPosition + Vector3.Forward * 10 ); }
			else if ( Power >= 1 && Power < 2 )
			{
				bullet.Clone( WorldPosition + Vector3.Left * 3 + Vector3.Forward * 10 );
				bullet.Clone( WorldPosition + Vector3.Right * 3 + Vector3.Forward * 10 );
			}
			else if ( Power >= 2 && Power < 3 )
			{
				bullet.Clone( WorldPosition + Vector3.Forward * 10 );
				bullet.Clone( WorldPosition + Vector3.Left * 5 + Vector3.Forward * 10 );
				bullet.Clone( WorldPosition + Vector3.Right * 5 + Vector3.Forward * 10 );
			}
			else if ( Power >= 3 )
			{
				bullet.Clone( WorldPosition + Vector3.Left * 2 + Vector3.Forward * 10 );
				bullet.Clone( WorldPosition + Vector3.Right * 2 + Vector3.Forward * 10 );
				bullet.Clone( WorldPosition + Vector3.Left * 6 + Vector3.Forward * 10 );
				bullet.Clone( WorldPosition + Vector3.Right * 6 + Vector3.Forward * 10 );
			}
			if ( BulletSound is not null ) Sound.Play( BulletSound );
		}
	}


	//根据玩家输入进行移动
	void Move()
	{
		// 如果按下Mouse3速度设为low speed
		if ( Input.Down( "Run" ) )
		{
			point.Enabled = true;
			speed = lowSpeed;
		}
		else
		{
			point.Enabled = false;
			speed = moveSpeed;
		}
		// 获取玩家输入
		Vector3 movedirection = Input.AnalogMove;
		// 移动
		WorldPosition += movedirection * speed * Time.Delta *10;
		// 限制移动范围
		WorldPosition = new Vector3( WorldPosition.x.Clamp( -58, 58 ), WorldPosition.y.Clamp( -46, 46 ), 0 );
		// 平滑移动




	}

	//和子弹发生碰撞
	public void OnTriggerEnter( Collider other )
	{
		GameTags tag = other.GameObject.Tags;
		
		if ( tag.Has( "bullet" ) )
		{
			DestroyAll();
			health -= 1;
			if ( Biu is not null ) Sound.Play( Biu );
			WorldPosition = new Vector3( -50, 0, 0 );
		}
		if ( tag.Has( "pickup" ) )
		{
			PickUpItem pickUpItem = other.GameObject.Components.Get<PickUpItem>();
			switch ( (int)pickUpItem.pickUpType )
			{
				case 0:
					health = health >= 8 ? 8 : ++health;
					break;
				case 1:
					spellcard = spellcard >= 8 ? 8 : ++spellcard;
					break;
				case 2:
					MaxPoint++;
					break;
				case 3:
					Power = Power > 4 ? 4 : Power + 0.1f;
					break;
			}
		}
	}

	public void OnTriggerExit( Collider other )
	{
	}

	void DestroyAll()
	{
		var gameObjects = Scene.GetAllObjects( true ).Where( obj => obj.Tags.Has( "bullet" ) );
		if ( !gameObjects.Any() ) return;
		var objects = gameObjects.ToList();
		foreach ( var o in objects )
		{
			o.Destroy();
		}

		var gameObjects2 = Scene.GetAllObjects( true ).Where( obj => obj.Tags.Has( "clean" ) );
		if ( !gameObjects2.Any() ) return;
		var objects2 = gameObjects2.ToList();
		foreach ( var o in objects2 )
		{
			o.Destroy();
		}
	}
}
