namespace NetDeviceManager.Database.Tables;

public class CredentialsData
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ConnString { get; set; }
    public string? Key { get; set; }
}