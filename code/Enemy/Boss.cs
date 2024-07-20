using System;
using System.Linq;
using Sandbox;
using Sandbox.UI;

public sealed class Boss : Component,Component.ITriggerListener

{
	public bool isAttack{ get; set; }//是否可以攻击
	[Property] public GameObject Bullet{ get; set; }//子弹
	[Property] public GameObject Bullet2{ get; set; }//子弹2
	[Property] public GameObject Bullet3{ get; set; }//子弹3
	[Property] public GameObject Bullet4{ get; set; }//子弹4
	[Property] public SoundEvent BulletSound{ get; set; }//子弹声音
	[Property] public BossHealth BossHealth { get; set; }//血条
	[Property] public SoundEvent DeadSound { get; set; }//死亡声音
	[Property] public float Health { get; set; } = 100;//生命值
	GameObject Player;//玩家
	public int AttackedType;//攻击方式
	
	private TimeSince AttackTime;//切换攻击的时间
	private TimeSince Bullet3Time;//Bullet3发射时间
	private Vector3 TargetPos;//目标位置
	private bool ischange = true;//是否可以切换攻击
	
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
	
	protected override void OnUpdate()
	{
		if(!isAttack) return;
		BossHealth.Enabled = true;
		SpwanBullet();
		ChangeAttack();
		
		if ( BossHealth is null ) return;
		BossHealth.bosshealth = Health;
		if ( Health <= 0 )
		{
			if ( BulletSound is not null ) Sound.Play( DeadSound);
			DestroyAll();
			GameObject.Destroy();
			BossHealth.Enabled = false;
			Log.Error( "Game Fnish,hold Esc to quit game" );
		}
	}

	private TimeSince Time;//攻击间隔
	void ChangeAttack()
	{
		if ( AttackTime > 2f && ischange)
		{
			// 生成一个随机整数
			Random random = new Random();
			int randomInt = random.Next( 0, 3 );
			AttackedType = randomInt;
			AttackTime = 0;
			Time = 0;
			ischange = false;
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
	}

	private bool isone =true;
	void Attacked1()
	{
		
		if ( Time < 3 )
		{
			TargetPos = Player.Transform.Position - Transform.Position;
			ShowGizmos(true);
		}
		if ( Time >= 3 )
		{
			if ( isone )
			{
				ShowGizmos(false);
				isone = false;
				if ( Bullet is not null )
				{
				GameObject bulletInstance = Bullet.Clone(Transform.Position);
				RotationBullet bullet = bulletInstance.Components.Get<RotationBullet>();
				// 设置子弹方向
				bullet.direction = TargetPos.Normal;	
				}
			}
			if ( Time >= 5 )
			{
				Time = 0;
				isone = true;
				ischange = true;
			}
		}
		
	}
	
	void Attacked2()
	{
		if ( Time > 1f )
		{
			Time = 0;
			if ( Bullet2 is not null )
			{
				Bullet2.Clone(Player.Transform.Position + Vector3.Forward * 80);
			}
			ischange = true;
		}
			
	}
	void Attacked3()
	{
		if ( Time > 1f )
		{
			Time = 0;
			if ( Bullet4 is not null )
			{
				Bullet4.Clone(Transform.Position);
			}
			ischange = true;
		}
	}

	void SpwanBullet()
	{
		if ( Bullet3Time > 0.1f )
		{
			Random random = new Random();
			int y = random.Next(-41, 41);
			Bullet3.Clone( new Vector3( 60, y, 1 ));
			Bullet3Time = 0;
		}
		
	}
	void ShowGizmos(bool show)
	{
		if(!show) return;
		if(TargetPos == 0) return;
		var bbox = BBox.FromPositionAndSize( 0, 1 );
		var tr = Scene.Trace
			.Ray( Transform.Position ,Transform.Position+ (Player.Transform.Position - Transform.Position).Normal * 1000)
			.Size( bbox )
			.WithoutTags( "player","bullet","playerbullet")
			.Radius( 1 )
			.Run();
		Gizmo.Draw.Color = Color.Red;
		Gizmo.Draw.LineThickness = 3;
		Gizmo.Draw.Line( tr.StartPosition,tr.EndPosition);
	}
	
	public void OnTriggerEnter( Collider other )
	{
		GameTags tag = other.GameObject.Tags;
		if ( tag.Has( "playerbullet" ) )
		{
			Health -= 0.1f;
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
