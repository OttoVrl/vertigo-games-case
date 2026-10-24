namespace Runtime.Interface
{
    public interface IUIWindow
    {
        void Show();
        void Hide();
        bool IsActivated { get; }
    }
}
