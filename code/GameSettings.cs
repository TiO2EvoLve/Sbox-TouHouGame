using Sandbox;


public class GameSettings
{
    public static GameSettings Instance
    {
        get
        {
            if ( _instance == null )
            {
                var file = "/settings/GameSetting.json";
                Log.Info( FileSystem.Data.ReadAllText( file ) );
                _instance = FileSystem.Data.ReadJson( file, new GameSettings() );
                
            }

            return _instance;
        }
    }
    static GameSettings _instance;

   
    public float MusicVolume { get; set; } = 10f;
    public int Life{ get; set; }= 2;
    public int Bomb { get; set; }= 2;
    public bool AutoShoot { get; set; } = false;
    public Difficult difficult { get; set; } = Difficult.Easy;
    
    public enum Difficult
    {
	    None,
	    Easy,
	    Normal,
	    Hard,
	    Lunatic
    }
    public void Save()
    {
        var file = "/settings/GameSetting.json";
        if ( FileSystem.Data.DirectoryExists( "/settings" ) == false )
            FileSystem.Data.CreateDirectory( "/settings" );
        FileSystem.Data.WriteJson( file, this );
    }
}
