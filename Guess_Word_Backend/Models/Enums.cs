namespace WordleServer.Models
{
    public enum LetterState
    {
        Absent,   // grey
        Present,  // yellow
        Correct   // green
    }

    public static class GamePhase
    {
        public const string WaitingForPlayers = "waiting";
        public const string WaitingForSecrets = "secrets";
        public const string InProgress = "in_progress";
        public const string Finished = "finished";
    }
}
