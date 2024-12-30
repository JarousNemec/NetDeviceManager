using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Utils;

namespace UtilsTest;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void FilterOldSyslogsTest()
    {
        var today = new DateTime(2024, 12, 12, 12, 12, 12);
        var syslog1 = new SyslogRecord()
        {
            Id = DatabaseUtil.GenerateId(),
            ProcessedDate = new DateTime(2024, 12, 1, 12, 12, 12),
        };
        var syslog2 = new SyslogRecord()
        {
            Id = DatabaseUtil.GenerateId(),
            ProcessedDate = today,
        };
        var syslogs = new List<SyslogRecord> { syslog1, syslog2 };
        var oldSyslogs = SyslogUtil.FilterOlderSyslogs(syslogs, today, 7);
        Assert.That(oldSyslogs.Count, Is.EqualTo(1));
        Assert.That(oldSyslogs.First().Id, Is.EqualTo(syslog1.Id));
    }
}