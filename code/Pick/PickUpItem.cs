using System.Collections.Generic;
using System.Diagnostics;
using Sandbox;

public sealed class PickUpItem : Component,Component.ITriggerListener
{

	public enum PickUpType
	{
		Health,
		Spellcard,
		score,
		power
	}
	
	[Property] public float speed { get; set; }
	[Property] public PickUpType pickUpType { get; set; }
	[Property] public SoundEvent PickSound { get; set; }
	
	[Property] public GameObject Health { get; set; }
	[Property] public GameObject Spellcard { get; set; }
	[Property] public GameObject Score { get; set; }
	[Property] public GameObject Power { get; set; }
	
	
	protected override void DrawGizmos()
	{
		Health.Enabled = false;
		Spellcard.Enabled = false;
		Score.Enabled = false;
		Power.Enabled = false;

		switch ( (int)pickUpType )
		{
			case 0: Health.Enabled = true;
				break;
			case 1: Spellcard.Enabled = true;
				break;
			case 2: Score.Enabled = true;
				break;
			case 3: Power.Enabled = true;
				break;
		}
	}
	
	
	protected override void OnUpdate()
	{
		Transform.Position += Vector3.Backward * speed * Time.Delta;
		DrawGizmos();
	}
	
	public void OnTriggerEnter( Collider other )
	{
		GameTags tag = other.GameObject.Tags;
		if ( tag.Has( "not" ) )
		{
			GameObject.Destroy();
			return;
		}
		if ( PickSound is not null ) Sound.Play( PickSound );
		GameObject.Destroy();
	}

	public void OnTriggerExit( Collider other )
	{
		
	}

	
	
}
