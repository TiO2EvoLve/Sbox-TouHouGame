using Sandbox;

public class DifficultyRatio
{
	private static float difficultRatio;
	private static float scoreRatio;

	

	public static float GetDifficultRatio()
	{

		switch ( (int)GameSettings.Instance.difficult )
		{
			case 1:
				difficultRatio = 1f;
				break;
			case 2:
				difficultRatio = 1.2f;
				break;
			case 3:
				difficultRatio = 1.6f;
				break;
			case 4:
				difficultRatio = 2f;
				break;
			default: difficultRatio = 1;break;
		}
		return difficultRatio;
	}
	
	public static float GetScoreRatio()
	{

		switch ( (int)GameSettings.Instance.difficult )
		{
			case 1:
				scoreRatio = 1f;
				break;
			case 2:
				scoreRatio = 2f;
				break;
			case 3:
				scoreRatio = 3f;
				break;
			case 4:
				scoreRatio = 4f;
				break;
			default: scoreRatio = 1;break;
		}
		return scoreRatio;
	}
}
