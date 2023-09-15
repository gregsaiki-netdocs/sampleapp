namespace sampleapp.Clock;

/// <summary>
/// A clock returns the current time.
/// </summary>
public interface IClock
{
    DateTime Now { get; }
}