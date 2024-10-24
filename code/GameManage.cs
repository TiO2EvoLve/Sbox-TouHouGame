using System;
using System.Linq;
using Sandbox;
using Sandbox.UI;

// 定义游戏的不同阶段枚举
public enum GameState
{
	Leave1,
	Leave2,
	Leave3,
}
public class GameManage : Component
{
	// 当前游戏状态
	private GameState currentState = GameState.Leave1;
	[Property] public float Leave1Time { get; set; }//第一关时间
	[Property] public float Leave2Time { get; set; }//第二关时间
	[Property] public float Leave3Time { get; set; }//第三关时间
	
	private TimeSince GameTime = 0;//游戏时间进度
	
	[Property] public TouhouPlayer TouhouPlayer { get; set; }//玩家实体
	[Property] public GameObject enemy { get; set; }//敌人实体
	[Property] public GameObject trackenemy { get; set; }//追踪弹敌人实体
	[Property] public GameObject boss { get; set; }//Boss实体
	[Property] public GameObject TimeLeftUI { get; set; }//倒计时UI
	[Property] public Dead EndUI { get; set; } //游戏结束菜单
	public bool Playing { get; private set; } = false;//是否在游戏中
	public Sandbox.Services.Leaderboards.Board chart;//排行榜
	public int MaxScore;//最高分
	bool isOne= true;//让上传排行榜只运行一次
	private TimeLeft timeLeft;
	private EnemyTrack enemyTrack;
	private Boss Boss;
	[Property] public SoundEvent GameMusic { get; set; } //游戏音乐
	public bool GameOver;
	
	protected override void OnStart()
	{
		StartGame();
		timeLeft = TimeLeftUI.Components.Get<TimeLeft>();
		enemyTrack = trackenemy.Components.Get<EnemyTrack>(  );
		Boss = boss.Components.Get<Boss>();
		timeLeft.RoundTime = Leave1Time;
	}
	protected override void OnUpdate()
	{
		checkTime();
		CheckGameState();
	}

	void checkTime()
	{
		if ( GameTime > Leave1Time &&GameTime< Leave1Time+Leave2Time) currentState = GameState.Leave2;
		if ( GameTime > Leave1Time + Leave2Time || enemyTrack.Health <= 0 )
		{
			currentState = GameState.Leave3;
			Boss.isAttack = true;
		}
		if ( GameTime >= Leave1Time + Leave2Time && enemyTrack.Health > 0 )
		{

			enemyTrack.Health = 0;
		}
		if ( GameTime >= Leave1Time + Leave2Time + Leave3Time ||Boss.Health <=0)
		{
			StopGame(TouhouPlayer.Score);
			GameOver = true;
		}
		
	}
	void CheckGameState( )
	{
		
		switch ( currentState )
		{
			case GameState.Leave1:
				SpawnEnemy();
				break;
			case GameState.Leave2:
				EnemyTrackLogic();
				break;
			case GameState.Leave3:
				SpawnBoss();
				break;
		}
	}

	void StartGame()
	{
		if ( Playing ) return;
		PlayMusic();
		Loadboard();
		Playing = true;
		
	}
	
	public void StopGame(int score)
	{
		if ( isOne )
		{
			isOne = false;
			EndUI.Enabled = true;
			Playing = false;
			Sandbox.Services.Stats.SetValue( "maxscore", score );
			Scene.TimeScale = 0f;
			Sound.StopAll( 5 );
			Sandbox.Services.Achievements.Unlock( "finishgame" );
		}
	}
	//重置游戏
	public void RemakeGame()
	{
		Game.ActiveScene.LoadFromFile("/scenes/MainGame.scene");
	}
	//加载排行榜
	async void Loadboard()
	{
		chart = Sandbox.Services.Leaderboards.Get( "maxscore" );
		chart.MaxEntries = 10;
		await chart.Refresh();
		//取出最高分付给MaxScore
		foreach ( var entry in chart.Entries )
		{
			if ( entry.SteamId == Game.SteamId )
			{
				MaxScore = (int)entry.Value;
			}
		}
		
	}
	
	float num= 0;//计时
	void SpawnEnemy()
	{
		num += Time.Delta;
		if(enemy is null)return;
		if ( num > 2f )
		{
			Random random = new Random();
			int x = random.Next(6, 47);
			int y = random.Next(-41, 41);
			Vector3 position = new Vector3( x, y, 1 ) ;
			enemy.Clone( position);
			num = 0;
		}
		
	}
	bool isTimeOne= true;//让时间赋值只运行一次
	void EnemyTrackLogic()
	{
		if ( isTimeOne )
		{
			isTimeOne = false;
			timeLeft.RoundTime = Leave2Time;
			enemyTrack.isAttacked = true;
			DestroyAll();
		}
	}
	bool isTimeOne2= true;//让时间赋值只运行一次
	void SpawnBoss()
	{
		if ( isTimeOne2 )
		{
			isTimeOne2 = false;
			boss.WorldPosition = new Vector3( 40, 0, 1 );
			timeLeft.RoundTime = Leave3Time;
			DestroyAll();
		}
		
	}
	
	void DestroyAll()
	{
		//var gameObjects = Scene.GetAllObjects( true ).Where( obj => obj.Tags.Has( "bullet" ) );
		//if ( !gameObjects.Any() ) return;
		//var objects = gameObjects.ToList();
		//foreach ( var o in objects )
		//{
		//	o.Destroy();
		//}

		//var gameObjects2 = Scene.GetAllObjects( true ).Where( obj => obj.Tags.Has( "clean" ) );
		//if ( !gameObjects2.Any() ) return;
		//var objects2 = gameObjects2.ToList();
		//foreach ( var o in objects2 )
		//{
		//	o.Destroy();
		//}
	}

	void PlayMusic()
	{
		if ( GameMusic is not null )
		{
			Sound.Play( GameMusic );
			GameMusic.Volume = GameSettings.Instance.MusicVolume;
		}
		
	}
	
}
