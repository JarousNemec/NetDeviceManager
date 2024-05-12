namespace NetDeviceManager.Web.Components.Layout;

public class NavbarHelper
{
    public delegate void SelectedNavbarChange(string name);

    public event SelectedNavbarChange OnSelectedNavbarChange;

    public void SelectedChange(string name)
    {
        OnSelectedNavbarChange.Invoke(name);
    }
}