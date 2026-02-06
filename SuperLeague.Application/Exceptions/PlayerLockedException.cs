// Exceptions/PlayerLockedException.cs
namespace SuperLeague.Application.Exceptions
{
    public class PlayerLockedException : Exception
    {
        public PlayerLockedException(string message) : base(message) { }
    }
}