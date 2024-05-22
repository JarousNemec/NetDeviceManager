namespace NetDeviceManager.Web.Components.Layout;

public class NavbarHelper
{
    public delegate void SelectedNavbarChange(string name);

    public string SelectedTabName { get; set; }

    public event SelectedNavbarChange? OnSelectedNavbarChange;

    public NavbarHelper()
    {
        SelectedTabName = string.Empty;
    }

    public void SelectedChange(string name)
    {
        OnSelectedNavbarChange?.Invoke(name);
    }
}