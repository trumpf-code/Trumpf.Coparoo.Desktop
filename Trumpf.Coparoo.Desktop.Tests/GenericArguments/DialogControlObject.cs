
namespace CoparooInterfaceTest
{
    using System.Windows;
    using Trumpf.Coparoo.Desktop.WPF;

    public interface IDialogControlObject : IControlObject
    {
    }

    public class DialogControlObject<T> : ViewControlObject<FrameworkElement>, IDialogControlObject where T : ContentControl
    {
        public void TestMethod1()
        {
        }

        /// <inheritdoc />
        protected override Search SearchPattern => new Search();
    }
}
