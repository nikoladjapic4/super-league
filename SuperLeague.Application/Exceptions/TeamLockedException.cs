namespace SuperLeague.Application.Exceptions
{
    public class TeamLockedException : Exception
    {
        public TeamLockedException(string message) : base(message) { }
    }
}