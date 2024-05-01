namespace NetDeviceManager.Database.Tables;

public class OidIntegerLabel
{
    public Guid Id { get; set; }
    public string Oid { get; set; }
    public int Number { get; set; }
    public string Label { get; set; }
}