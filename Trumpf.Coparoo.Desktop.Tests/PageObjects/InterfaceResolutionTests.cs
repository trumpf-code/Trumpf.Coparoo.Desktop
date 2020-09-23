namespace CoparooInterfaceTest
{
    using NUnit.Framework;
    using Trumpf.Coparoo.Desktop;
    using Trumpf.Coparoo.Desktop.Exceptions;
    using Trumpf.Coparoo.Desktop.WPF;

    [TestFixture]
    public class InterfaceResolutionTests
    {
        [Test]
        public void WhenTryingToFindAControlObjectWithTwoImplementations_AnAmbiguousControlObjectMatchExceptionIsThrown()
            => Assert.Throws<AmbiguousControlObjectMatchException>(() => new Process().Find<IControlObjectWithTwoImplementations>());

        private interface IControlObjectWithTwoImplementations : IControlObject
        {
        }

        private class Process : ProcessObject
        {
        }

        private class Page : PageObject, IChildOf<Process>
        {
            protected override Search SearchPattern => throw new System.NotImplementedException();
        }

        private class FirstImplementation : ControlObject, IControlObjectWithTwoImplementations
        {
            protected override Search SearchPattern => throw new System.NotImplementedException();
        }

        private class SecondImplementation : ControlObject, IControlObjectWithTwoImplementations
        {
            protected override Search SearchPattern => throw new System.NotImplementedException();
        }
    }
}
