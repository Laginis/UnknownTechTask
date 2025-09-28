public interface IUIScreen
{
    public UIScreenType UIScreenType { get; }
    public void SetSwitcher(UISwitcher switcher);
    public void Show();
    public void Hide();
}
