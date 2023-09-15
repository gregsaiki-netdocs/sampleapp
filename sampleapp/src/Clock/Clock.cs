using System;
using System.Diagnostics;
using System.Threading;

namespace sampleapp.Clock;

/// <summary>
/// This class provides a high resolution clock by using a manually tuned 
/// and compensated <c>DateTime</c> which takes advantage of the high resolution
/// available in <see cref="Stopwatch"/>.
/// </summary>
public sealed class Clock : IDisposable, IClock
{
    private readonly long _maxIdleTime = TimeSpan.FromSeconds(10).Ticks;
    private const long TicksMultiplier = 1000 * TimeSpan.TicksPerMillisecond;

    private readonly ThreadLocal<DateTime> _startTime =
        new ThreadLocal<DateTime>(() => DateTime.UtcNow, false);

    private readonly ThreadLocal<double> _startTimestamp =
        new ThreadLocal<double>(() => Stopwatch.GetTimestamp(), false);


    /// <summary>
    /// Creates an instance of the <see cref="Clock"/>.
    /// </summary>
    public Clock()
    {

    }

    /// <summary>
    /// Gets the date and time in <c>UTC</c>.
    /// </summary>
    public DateTime Now
    {
        get
        {
            double endTimestamp = Stopwatch.GetTimestamp();

            var durationInTicks = (endTimestamp - _startTimestamp.Value) / Stopwatch.Frequency * TicksMultiplier;
            if (durationInTicks >= _maxIdleTime)
            {
                _startTimestamp.Value = Stopwatch.GetTimestamp();
                _startTime.Value = DateTime.UtcNow;
                return _startTime.Value;
            }

            return _startTime.Value.AddTicks((long)durationInTicks);
        }
    }

    /// <summary>
    /// Releases all resources used by the instance of <see cref="Clock"/>.
    /// </summary>
    public void Dispose()
    {
        _startTime.Dispose();
        _startTimestamp.Dispose();
    }
}
