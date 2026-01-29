
namespace SuperLeague.Tests.Fakes
{
    public class FakeClock
    {
        private DateTime _currentTime;

        public FakeClock(DateTime? startTime = null) => _currentTime = startTime ?? new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        public DateTime UtcNow() => _currentTime;

        public void Set(DateTime time) => _currentTime = time;

        public void Advance(TimeSpan duration) => _currentTime += duration;

        public void AdvanceDays(int days) => _currentTime = _currentTime.AddDays(days);

        public void AdvanceHours(int hours) => _currentTime = _currentTime.AddHours(hours);
    }
}