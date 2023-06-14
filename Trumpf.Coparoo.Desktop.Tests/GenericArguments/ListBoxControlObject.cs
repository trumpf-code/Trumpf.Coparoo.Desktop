namespace CoparooInterfaceTest
{
    using Trumpf.Coparoo.Desktop;
    using Trumpf.Coparoo.Desktop.WPF;

    public interface IListBoxControlObject<T> : IControlObject where T : IControlObject
    {
        T Value { get; }
    }

    public class ListBoxControlObject<T> : ViewControlObject<ListBox>, IListBoxControlObject<T> where T : IControlObject
    {
        public T Value => this.Find<T>();
    }
}
