namespace CoparooInterfaceTest
{
    using Trumpf.Coparoo.Desktop.WPF;

    public interface IDataTemplateObjectBase : IControlObject
    {
    }

    public class DataTemplateObjectBase<T> : ViewControlObject<T>, IDataTemplateObjectBase where T : FrameworkElement
    {
        /// <inheritdoc />
        protected override Search SearchPattern => new Search();
    }
}
