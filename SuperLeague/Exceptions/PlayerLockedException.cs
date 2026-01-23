// Exceptions/PlayerLockedException.cs
namespace SuperLeague.Exceptions
{
    public class PlayerLockedException : Exception
    {
        public PlayerLockedException(string message) : base(message) { }
    }
}