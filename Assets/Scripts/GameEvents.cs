using System.Collections.Generic;
using KemothStudios.Board;
using KemothStudios.Utility.Events;

namespace KemothStudios
{
    /// <summary>
    /// Called from <b>GameManager</b> to start the game
    /// </summary>
    public struct GameStartedEvent : IEvent{}
    
    /// <summary>
    /// Called when a major button is used
    /// </summary>
    public struct MajorButtonClickedEvent : IEvent {}

    /// <summary>
    /// Called when a line is drawn
    /// </summary>
    public struct DrawLineEvent : IEvent
    {
        public Line Line{get; private set;}
        public DrawLineEvent(Line line) => Line = line;
    }
    
    /// <summary>
    /// Called when a line is drawn and any cell(s) completed because of this line has been acquired
    /// </summary>
    public struct BoardReadyAfterDrawLineEvent : IEvent {}

    /// <summary>
    /// Called when a line draw results in one or more cells being acquired and the acquire process is stared 
    /// </summary>
    public struct CellsAcquireStartedEvent : IEvent
    {
        public IEnumerable<Cell> Cells { get; private set; }
        public CellsAcquireStartedEvent(IEnumerable<Cell> cells) => Cells = cells;
    }
    
    /// <summary>
    /// Called when a cell is being acquired, only one cell will be acquired at a time 
    /// </summary>
    public struct CellAcquiredEvent : IEvent
    {
        public Cell Cell{get; private set;}
        public CellAcquiredEvent(Cell cell) => Cell = cell;
    }

    /// <summary>
    /// Called when cell acquire process is completed
    /// </summary>
    public struct CellAcquireCompletedEvent : IEvent {}
    
    /// <summary>
    /// Called when all cells being acquired and a winner has been chosen, basically game is over
    /// </summary>
    public struct PlayerWonEvent : IEvent
    {
        public Player WinnerPlayer {get; private set;}
        public PlayerWonEvent(Player winner) => WinnerPlayer = winner;
    }
    
    /// <summary>
    /// Called when a minor button is clicked
    /// </summary>
    public struct MinorButtonClickedEvent : IEvent{}
    
    /// <summary>
    /// Called when an avatar being clicked on avatar selection screen
    /// </summary>
    public struct AvatarClickedEvent : IEvent{}
    
    /// <summary>
    /// Called when score has been updated
    /// </summary>
    public struct ScoreUpdatedEvent : IEvent{}

    /// <summary>
    /// Called when a turn has been started
    /// </summary>
    public struct TurnStartedEvent : IEvent
    {
        public Player Player {get; private set;}
        public TurnStartedEvent(Player player) => Player = player;
    }
    
    /// <summary>
    /// Called when a turn has been ended
    /// </summary>
    public struct TurnEndedEvent : IEvent
    {
        public Player Player {get; private set;}
        public TurnEndedEvent(Player player) => Player = player;
    }
    
    /// <summary>
    /// Called when victory screen needed to be shown
    /// </summary>
    public struct ShowVictoryEvent : IEvent
    {
        public Player Player {get; private set;}
        public ShowVictoryEvent(Player player) => Player = player;
    }
    
    /// <summary>
    /// Called when a scene has been loaded and loading screen is not visible anymore
    /// </summary>
    public struct SceneLoadingCompleteEvent : IEvent {}
}