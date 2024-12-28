namespace NetDeviceManager.Lib.Utils;

public static class DatabaseUtil
{
    public static Guid GenerateId()
    {
        return Guid.NewGuid();
    }
}