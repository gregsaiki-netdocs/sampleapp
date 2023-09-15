using sampleapp.Clock;

namespace sampleapp.Endpoints;


/// <summary>
/// Provides runctions that return the current document modification number.
/// </summary>
public class CurrentDocumentModificationNumberProvider
{
    private const string TIME_STAMP_FORMAT = "yyyyMMddHHmmssfff";

    /// <summary>
    /// Clock to get the current time from.  Expected to be in UTC.
    /// </summary>
    private readonly IClock _clock;

    /// <summary>
    /// Used for logging messages
    /// </summary>
    private readonly ILogger<CurrentDocumentModificationNumberProvider> _logger;

    /// <summary>
    /// Default constructionr
    /// </summary>
    /// <param name="clock">IClock instance used to get current time.</param>
    /// <param name="logger">ILogger instance used for logging.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public CurrentDocumentModificationNumberProvider(IClock clock, ILogger<CurrentDocumentModificationNumberProvider> logger)
    {
        this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get current document modification number based on UTC time.
    /// </summary>
    /// <returns></returns>
    public long GetUTC()
    {
        return long.Parse(_clock.Now.ToString(TIME_STAMP_FORMAT));
    }

    /// <summary>
    /// Get current document modification number based on an offset from UTC.
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    public long GetLocalOffset(long offset)
    {
        return long.Parse(_clock.Now.Add(TimeSpan.FromTicks(offset)).ToString(TIME_STAMP_FORMAT));
    }

    /// <summary>
    /// Get current document modification number based on a modified offset that is gradually getting closer to UTC.
    /// </summary>
    /// <param name="offset">Time offset</param>
    /// <param name="begin">When to start migrating offset to 0</param>
    /// <param name="end">When offset should be migrated to 0 by</param>
    /// <returns></returns>
    public long GetMigratingOffset(long offset, long begin, long end)
    {
        DateTime now = _clock.Now;
        var migrationStart = DateTime.FromBinary(begin);
        var migrationEnd = DateTime.FromBinary(end);
        if (now < migrationStart)
            return GetLocalOffset(offset);
        else if (now > migrationEnd)
            return GetUTC();
        TimeSpan migrateOver = migrationEnd - migrationStart;
        TimeSpan whereInMigration = migrationEnd - now;
        TimeSpan timeDifference = TimeSpan.FromTicks(offset);
        long migratedOffset = Convert.ToInt64(((double)whereInMigration.Ticks / (double)migrateOver.Ticks) * timeDifference.Ticks);
        DateTime currentTimeMigrated = now.AddTicks(migratedOffset);
        _logger.LogDebug("Migrating offset {start} {end} {over} {where} {difference} {offset} {current}", migrationStart, migrationEnd, migrateOver, whereInMigration, timeDifference, migratedOffset, currentTimeMigrated);
        return long.Parse(currentTimeMigrated.ToString(TIME_STAMP_FORMAT));
    }
}
