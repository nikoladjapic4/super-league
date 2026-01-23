namespace SuperLeague.Exceptions
{
    public class TeamLockedException : Exception
    {
        public TeamLockedException(string message) : base(message) { }
    }
}