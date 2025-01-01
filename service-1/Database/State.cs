namespace service_1.Database;

public class State{
    public int Id { get; set; }
    public AppState CurrentAppState { get; set; }
}

public enum AppState
{
    INIT, 
    PAUSED,
    RUNNING,
    SHUTDOWN
}