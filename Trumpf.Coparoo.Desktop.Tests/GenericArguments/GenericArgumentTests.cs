namespace CoparooInterfaceTest
{
    using NUnit.Framework;
    using Trumpf.Coparoo.Desktop;

    [TestFixture]
    public class GenericArgumentTests
    {
        [Test]
        public void WhenTryingToFindControlObjectWithGenericArgument_CorrectTypeIsReturned()
        {
            // during resolve it should not hit unrelated types like DialogControlObject
            IUIObject processObject = (IUIObject)new ProcessObject();
            
            var expected = processObject.Find<ListBoxControlObject<IListDataTemplateObject>>();
            
            
            var proc = new ProcessObject().Find<IListBoxControlObject<IListDataTemplateObject>>();
            Assert.AreEqual(expected.GetType(), proc.GetType());
            Assert.AreEqual(typeof(ListDataTemplateObject), proc.Value.GetType());
        }

        [Test]
        public void WhenTryingToFindControlObjectWithGenericArgument_NoExceptionIsThrown()
        {
            Assert.DoesNotThrow(() => new ProcessObject().Find<IListBoxControlObject<IListDataTemplateObject>>());
        }
    }
}
