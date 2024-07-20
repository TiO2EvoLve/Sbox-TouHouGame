using Sandbox;

public sealed class PlayerBullet : Component,Component.ITriggerListener
{
	
	[Property] public float speed { get; set; } = 10f; 
	
	
	
	protected override void OnUpdate()
	{
		MoveBullet();
	}
	
	void MoveBullet()
	{
		
		Transform.Position += Vector3.Forward * speed * Time.Delta;
	}


	public void OnTriggerEnter( Collider other )
	{
		GameTags tag = other.GameObject.Tags;
		if(tag.Has( "not" ))	return;
		GameObject.Destroy();
	}

	public void OnTriggerExit( Collider other )
	{
		
	}
}
