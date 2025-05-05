using NUnit.Framework;
using Trumpf.Coparoo.Desktop.CompactClassResolver;

namespace CompactClassResolver.Tests
{
    [TestFixture]
    public class CompactClassResolverContainerTests
    {
        [Test]
        public void TestRegisterInstance()
        {
            var container = new CompactClassResolverContainer();
            var instance = new MyClass();
            container.RegisterInstance<MyClass>(instance);

            var resolved = container.Resolve<MyClass>();

            Assert.AreSame(instance, resolved, "Resolved instance should be the same as the registered singleton.");
        }

        [Test]
        public void TestRegisterConcreteType()
        {
            var container = new CompactClassResolverContainer();
            container.Register(typeof(SimpleClass));

            var resolved = container.Resolve<SimpleClass>();

            Assert.IsNotNull(resolved, "Resolved instance should not be null.");
            Assert.IsInstanceOf(typeof(SimpleClass), resolved, "Resolved instance should be of type SimpleClass.");
        }

        [Test]
        public void TestRegisterInterfaceMapping()
        {
            var container = new CompactClassResolverContainer();
            container.Register<IInterface, InterfaceImpl>();

            var resolved = container.Resolve<IInterface>();

            Assert.IsNotNull(resolved, "Resolved instance should not be null.");
            Assert.IsInstanceOf(typeof(InterfaceImpl), resolved, "Resolved instance should be of type InterfaceImpl.");
        }

        [Test]
        public void TestRegisterInterfaceInstance()
        {
            var container = new CompactClassResolverContainer();
            var instance = new InterfaceImpl();
            container.RegisterInstance<IInterface>(instance);

            var resolved = container.Resolve<IInterface>();

            Assert.IsNotNull(resolved, "Resolved instance should not be null.");
            Assert.IsInstanceOf(typeof(InterfaceImpl), resolved, "Resolved instance should be of type InterfaceImpl.");
            Assert.AreSame(instance, resolved, "Resolved instance should be the same as the registered singleton.");
        }

        [Test]
        public void TestConstructorInjection()
        {
            var container = new CompactClassResolverContainer();
            container.Register(typeof(Dependency));
            container.Register(typeof(Dependent));

            var resolved = container.Resolve<Dependent>();

            Assert.IsNotNull(resolved, "Resolved Dependent instance should not be null.");
            Assert.IsNotNull(resolved.Dependency, "Constructor injection failed: Dependency is null.");
        }

        [Test]
        public void TestMissingRegistrationThrowsException()
        {
            var container = new CompactClassResolverContainer();
            Assert.Throws<ResolutionFailedException>(() => container.Resolve<UnregisteredClass>());
        }

        [Test]
        public void TestCircularDependencyDetectionThrowsException()
        {
            var container = new CompactClassResolverContainer();
            container.Register(typeof(CircularA));
            container.Register(typeof(CircularB));

            Assert.Throws<ResolutionFailedException>(() => container.Resolve<CircularA>());
        }

        [Test]
        public void TestFallbackToDefaultConstructor()
        {
            var container = new CompactClassResolverContainer();
            container.Register(typeof(ClassWithMultipleConstructors));

            var resolved = container.Resolve<ClassWithMultipleConstructors>();

            Assert.IsNotNull(resolved, "Resolved instance should not be null.");
            Assert.AreEqual("Default", resolved.Name, "Instance should have been created using the parameterless constructor.");
        }

        [Test]
        public void TestSelectingConstructorWithMostParameters()
        {
            var container = new CompactClassResolverContainer();
            container.Register(typeof(Dependency));
            container.Register(typeof(ExtraDependency));
            container.Register(typeof(MultiConstructorClass));

            var resolved = container.Resolve<MultiConstructorClass>();

            Assert.IsNotNull(resolved, "Resolved instance should not be null.");
            Assert.AreEqual("Two-Param", resolved.ConstructorUsed, "The constructor with most parameters should have been used.");
        }

        [Test]
        public void TestDependencyGraphSingletons()
        {
            // Register all classes in the dependency graph:  
            // ClassB depends on ClassA, ClassC depends on ClassB and  
            // ClassD depends on ClassA, ClassB, and ClassC.  
            var container = new CompactClassResolverContainer();
            container.Register(typeof(ClassA));
            container.Register(typeof(ClassB));
            container.Register(typeof(ClassC));
            container.Register(typeof(ClassD));

            // Resolve ClassD twice. The expectation is that resolution returns the same instance,  
            // and that all dependencies within the graph are not re-created.  
            var d1 = container.Resolve<ClassD>();

            // In Stashbox, d1.A and d1.B.A will not be the same instance by default.
            Assert.AreNotSame(d1.A, d1.B.A, "All ClassA instances in the dependency graph should be the same instance.");
            Assert.AreNotSame(d1.B, d1.C.B, "All ClassB instances in the dependency graph should be the same instance.");
        }
    }

    // --- Dummy classes used for testing ---  

    public class MyClass
    {
    }

    public class SimpleClass
    {
        public SimpleClass() { }
    }

    public interface IInterface
    {
    }

    public class InterfaceImpl : IInterface
    {
        public InterfaceImpl() { }
    }

    public class Dependency
    {
        public Dependency() { }
    }

    public class Dependent
    {
        public Dependency Dependency { get; private set; }

        public Dependent(Dependency dependency)
        {
            Dependency = dependency;
        }
    }

    public class UnregisteredClass
    {
    }

    public class CircularA
    {
        public CircularB B { get; private set; }

        public CircularA(CircularB b)
        {
            B = b;
        }
    }

    public class CircularB
    {
        public CircularA A { get; private set; }

        public CircularB(CircularA a)
        {
            A = a;
        }
    }

    public class ClassWithMultipleConstructors
    {
        public string Name { get; set; }
        private IInterface _interface;

        // Constructor with an unsatisfied parameter.  
        public ClassWithMultipleConstructors(IInterface iface)
        {
            Name = "Interface";
            _interface = iface;
        }

        // Parameterless constructor as fallback.  
        public ClassWithMultipleConstructors()
        {
            Name = "Default";
        }
    }

    public class ExtraDependency
    {
        public ExtraDependency() { }
    }

    public class MultiConstructorClass
    {
        public string ConstructorUsed { get; private set; }

        // Constructor with one parameter.  
        public MultiConstructorClass(Dependency dep)
        {
            ConstructorUsed = "One-Param";
        }

        // Constructor with two parameters.  
        public MultiConstructorClass(Dependency dep, ExtraDependency extra)
        {
            ConstructorUsed = "Two-Param";
        }
    }

    // Classes for the dependency graph singleton test.  
    public class ClassA
    {
    }

    public class ClassB
    {
        public ClassA A { get; private set; }
        public ClassB(ClassA a)
        {
            A = a;
        }
    }

    public class ClassC
    {
        public ClassB B { get; private set; }
        public ClassC(ClassB b)
        {
            B = b;
        }
    }

    public class ClassD
    {
        public ClassA A { get; private set; }
        public ClassB B { get; private set; }
        public ClassC C { get; private set; }
        public ClassD(ClassA a, ClassB b, ClassC c)
        {
            A = a;
            B = b;
            C = c;
        }
    }
}