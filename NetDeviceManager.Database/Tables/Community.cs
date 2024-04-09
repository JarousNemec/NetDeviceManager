namespace NetDeviceManager.Database.Tables;

public class Community
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string CommunityStringValue { get; set; }
}