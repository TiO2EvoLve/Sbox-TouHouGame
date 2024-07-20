using System;
using System.Numerics;
using Sandbox;

namespace TouhouGame;



public sealed class Enemy : Component,Component.ITriggerListener
{


	[Property] public GameObject Bulletprefab { get; set; }
	[Property] public GameObject PickUpprefab { get; set; }
	[Property] public SoundEvent FireSound { get; set; }
	private EnemyHealthBar enemyHealthBar;

	[Property] public PickUpItem PickUpItem{ get; set; }
	
	public int numberOfBullets = (int)(12 * DifficultyRatio.GetDifficultRatio());  // �ӵ�����
	public float circleRadius = 5f ;  // �ӵ����ɵ�Բ�İ뾶
	public float spawntime = 0;
	
	
	
	protected override void OnStart()
	{
		enemyHealthBar = Components.GetInDescendantsOrSelf<EnemyHealthBar>();
	}


	protected override void OnUpdate()
	{
		if ( enemyHealthBar.health <= 0 )
		{
			SpawnPickUp();
			GameObject.Destroy();
		}

		spawntime += Time.Delta;
		if ( spawntime > 0.5f )
		{
			SpawnBulletCircle();
			spawntime = 0;
		}
		
	}

	public void OnTriggerEnter( Collider other )
	{
		GameTags tag = other.GameObject.Tags;
		if (tag.Has("playerbullet"))
		{
			if(enemyHealthBar is null) return;
			enemyHealthBar.health -= 20f;
		}
	}

	public void OnTriggerExit( Collider other )
	{
		
	}
	
	void SpawnBulletCircle()
	{
		// ����һ����0��359֮����������
		Random random = new Random();
		int randomInt = random.Next(0, 60);
		
		for (int i = 0; i < numberOfBullets; i++)
		{
			
			// �����ӵ���Բ�ϵ�λ��
			float angle = i * (360f / numberOfBullets) + randomInt;
			Vector3 spawnPosition = Transform.Position + new Vector3((float)Math.Cos(MathX.DegreeToRadian(angle)), (float)Math.Sin(MathX.DegreeToRadian( angle )),0) * circleRadius;
			// �����ӵ�
			if(Bulletprefab is null)return;
			GameObject bulletInstance = Bulletprefab.Clone( spawnPosition );
			Bullet bullet = bulletInstance.Components.Get<Bullet>();

			// �����ӵ�����
			bullet.direction = (spawnPosition - (Vector3)Transform.Position).Normal;
		}

		if ( FireSound is not null ) Sound.Play( FireSound );
	}

	void SpawnPickUp()
	{
		
		Random random = new Random();
		// ����һ������0��9֮����������
		int randomNumber = random.Next(10);
		if(PickUpprefab is null) return;
		if (randomNumber == 0 )
		{
			SpawnDrop( 0 );
		}
		else if(randomNumber ==1)
		{
			SpawnDrop( 1 );
		}
		else if(randomNumber <7)
		{
			SpawnDrop( 2 );
		}
		else
		{
			SpawnDrop( 3 );
		}
	}

	void SpawnDrop(int type)
	{
		GameObject gameObject = PickUpprefab.Clone( Transform.Position );
		PickUpItem pickUpItem = gameObject.Components.Get<PickUpItem>();

		switch ( type )
		{
			case 0:pickUpItem.pickUpType = PickUpItem.PickUpType.Health;
				break;
			case 1:pickUpItem.pickUpType = PickUpItem.PickUpType.Spellcard;
				break;
			case 2:pickUpItem.pickUpType = PickUpItem.PickUpType.score;
				break;
			case 3:pickUpItem.pickUpType = PickUpItem.PickUpType.power;
				break;
			
		}
		
	}
	
}
