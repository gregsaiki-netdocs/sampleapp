using sampleapp.Clock;
using sampleapp.Endpoints;
using Moq;
using FluentAssertions;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace sampleapp.Tests.UnitTests;

[Trait("Category", "Unit")]
/// <summary>
/// Tests all functions on CurrentDocumentModificationNumber class
/// </summary>
public class Testsampleapp
{
    private readonly DateTime now = DateTime.ParseExact("2022-11-25 01:23:45.678", "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
    private const long nowAsLong = 20221125012345678;
    /// <summary>
    /// Ensure GetUTC returns exactly what is provided
    /// </summary>
    [Fact]
    public void TestGetUTC()
    {
        var clock = new Mock<IClock>();
        clock.Setup(clock => clock.Now).Returns(now);
        var logger = new Mock<ILogger<CurrentDocumentModificationNumberProvider>>();
        var endpoint = new CurrentDocumentModificationNumberProvider(clock.Object, logger.Object);
        endpoint.GetUTC().Should().Be(nowAsLong);
    }

    /// <summary>
    /// Ensure GetLocalOffset functions correctly
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="expected"></param>
    [Theory]
    [InlineData(216000000000, 20221125072345678)]
    [InlineData(-216000000000, 20221124192345678)]
    public void TestGetLocalOffset(long offset, long expected)
    {
        var clock = new Mock<IClock>();
        clock.Setup(clock => clock.Now).Returns(now);
        var logger = new Mock<ILogger<CurrentDocumentModificationNumberProvider>>();
        var endpoint = new CurrentDocumentModificationNumberProvider(clock.Object, logger.Object);
        endpoint.GetLocalOffset(offset).Should().Be(expected);
    }

    /// <summary>
    /// Validate that expected values are returned from GetMigratingOffset
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="begin"></param>
    /// <param name="end"></param>
    /// <param name="expected"></param>
    [Theory]
    [InlineData(216000000000, 638048448000000000, 638050176000000000, 20221125041317468)]
    [InlineData(-216000000000, 638048448000000000, 638050176000000000, 20221124223413887)]
    [InlineData(216000000000, 638050176000000000, 638052768000000000, 20221125072345678)] // Before
    [InlineData(-216000000000, 638044992000000000, 638048448000000000, nowAsLong)] // After
    public void TestGetMigratingOffset(long offset, long begin, long end, long expected)
    {
        var clock = new Mock<IClock>();
        clock.Setup(clock => clock.Now).Returns(now);
        var logger = new Mock<ILogger<CurrentDocumentModificationNumberProvider>>();
        var endpoint = new CurrentDocumentModificationNumberProvider(clock.Object, logger.Object);
        endpoint.GetMigratingOffset(offset, begin, end).Should().Be(expected);
    }

    /// <summary>
    /// Validate that GetMigratingOffset behaves correctly.
    /// If now is before start date then it should have the full offset
    /// If now is after the end date then there should be no offset
    /// Inbetween those dates the offset should continue to get closer to zero
    ///     the closer you get to the end date.
    /// </summary>
    /// <param name="offset"></param>
    [Theory]
    [InlineData(648000000000)]
    [InlineData(-648000000000)]
    public void TestGetMigratingOffsetDynamic(long offset)
    {
        //long offset = 216000000000;
        DateTime currentDocModNumDate = now;
        var clock = new Mock<IClock>();
        clock.Setup(clock => clock.Now).Returns(() => currentDocModNumDate);
        var logger = new Mock<ILogger<CurrentDocumentModificationNumberProvider>>();
        var currentDocModNum = new CurrentDocumentModificationNumberProvider(clock.Object, logger.Object);
        
        DateTime yesterday = now.Date - TimeSpan.FromDays(1);
        long yesterdayBinary = yesterday.ToBinary();
        currentDocModNumDate = yesterday;
        long yesterdayOffset = currentDocModNum.GetLocalOffset(offset);

        DateTime tomorrow = now.Date + TimeSpan.FromDays(1);
        long tomorrowBinary = tomorrow.ToBinary();
        currentDocModNumDate = tomorrow;
        long tomorrowOffset = currentDocModNum.GetLocalOffset(offset);

        currentDocModNumDate = now - TimeSpan.FromDays(3);
        long last = currentDocModNum.GetMigratingOffset(offset, yesterdayBinary, tomorrowBinary);
        // Because our modification numbers are based on a timestamp there will be large
        // jumps so we have to calculate the difference based on ticks which are contiguous.
        long lastDiff = Math.Abs(last.ToDateTime().Ticks - currentDocModNumDate.Ticks);
        last.Should().Be(currentDocModNum.GetLocalOffset(offset));
        for (DateTime current = now.Date - TimeSpan.FromDays(2); current < now.Date + TimeSpan.FromDays(2); current = current + TimeSpan.FromMilliseconds(10+Random.Shared.Next(60)))
        {
            currentDocModNumDate = current;
            long currentModNum = currentDocModNum.GetMigratingOffset(offset, yesterdayBinary, tomorrowBinary);
            currentModNum.Should().BeGreaterThan(last);
            long currentDiff = Math.Abs(currentModNum.ToDateTime().Ticks - current.Ticks);
            currentDiff.Should().BeLessThanOrEqualTo(lastDiff);
            if(current < yesterday)
            {
                currentModNum.Should().Be(currentDocModNum.GetLocalOffset(offset));
            }
            else if (current > tomorrow)
            {
                currentModNum.Should().Be(currentDocModNum.GetUTC());
            }
            else
            {
                currentModNum.Should().BeGreaterThan(yesterdayOffset);
                if (offset > 0)
                {
                    currentModNum.Should().BeGreaterThan(currentDocModNum.GetUTC());
                }
                else if(offset < 0)
                {
                    currentModNum.Should().BeLessThan(currentDocModNum.GetUTC());
                }
            }
            last = currentModNum;
            lastDiff = currentDiff;
        }
    }
}

public static class ModNumConvert
{
    /// <summary>
    /// Helper function to convert a long generated from a datetime back into a datetime
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DateTime ToDateTime(this long value)
    {
        return DateTime.ParseExact(value.ToString(), "yyyyMMddHHmmssfff", System.Globalization.CultureInfo.InvariantCulture);
    }
}