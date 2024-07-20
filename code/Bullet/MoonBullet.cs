using System.Linq;
using Sandbox;

public sealed class MoonBullet : Component
{
	[Property] public GameObject pos { get; set; }
	public float speed = 10f * DifficultyRatio.GetDifficultRatio();  // 子弹速度
	public Vector3 direction = Vector3.Backward;  // 子弹运动方向
	public GameObject Player;//玩家物体
	public TimeSince life = 0;
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
		direction = Player.Transform.Position - Transform.Position;
		Transform.Position += (direction.Normal * speed * Time.Delta); 
		Gizmo.Draw.Color = Color.Red;
		Gizmo.Draw.LineThickness = 3;
		Gizmo.Draw.Text((10-life.Relative.CeilToInt()).ToString(),pos.Transform.World,"Roboto",40f);
		if(life > 10 ) GameObject.Destroy();
	}
	
	
}
