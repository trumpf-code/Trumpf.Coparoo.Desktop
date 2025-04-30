# Trumpf.Coparoo.Desktop Library for .NET 
![logo]
[![appVeyorBuildStatus]](https://ci.appveyor.com/project/trumpf-code/trumpf-coparoo-desktop)

## Description
*Trumpf.Coparoo.Desktop is a .NET library for C# that helps you write fast, maintainable, robust and fluent TestLeft-driven UI tests based on the **co**ntrol/**pa**ge/**ro**ot-**o**bject (Coparoo) pattern.*

The library supports all kinds of UI and UI-automation technologies including `WPF`, `WinForms`, `Web` and `UIA`.
In order to run tests you must ensure that the commercial [SmartBear's TestLeft®](https://smartbear.com/product/testleft) product is installed (free trials are available).
For building test projects, e.g. on CI build agents, technically, no license or installation is required.

The following sign-in/out test scenario illustrates how the framework facilitates writing, e.g., a Web test in a "natural" way:
    
    var app = new GitHubWebDriver();                    // create the test driver
    app.Open();                                         // open the github page in a new browser tab
    app.On<Header>().SignIn.Click();                    // click the sign-in button
    app.On<SignInForm>().SignIn("myUser", "abc");       // enter the user credentials ...
    app.On<Header>().Profile.Click();                   // open the user profile
    app.On<ProfileDrowndown>().SignOut.Click();         // sign out

Just in the same way then the other UI technologies can be tested, and even interact with each other, e.g. for testing WPF apps that integrate with the web.

If you are interested in Web tests only, the following project may be of interest to you: [Trumpf.Coparoo.Web](https://github.com/trumpf-code/Trumpf.Coparoo.Web).

## NuGet Package Information
To make it easier for you to develop with the *Trumpf Coparoo Desktop* library we release it as NuGet package. The latest library is available on [https://www.nuget.org/packages/Trumpf.Coparoo.Desktop](https://www.nuget.org/packages/Trumpf.Coparoo.Desktop).
To install, just type `Install-Package Trumpf.Coparoo.Desktop` in the [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console).

## Getting Started
This library is a derivate of the [Trumpf.Coparoo.Web](https://github.com/trumpf-code/Trumpf.Coparoo.Web) project and can essentially be obtained by replacing *Selenium API* calls by SmartBear's *TestLeft API* calls, plus adding wrapper classes for the various UI technologies, like `Trumpf.Coparoo.Desktop.WinForms.ViewPageObject` and `Trumpf.Coparoo.Desktop.WPF.ViewPageObject`.
Therefore, if you want to get the concepts behind this framework the documentation of the former project is a good starting point: <https://github.com/trumpf-code/Trumpf.Coparoo.Web/blob/master/README.md>.

The following example shows a simple test of the `DemoApp` test project; source file [Tests.cs](./Trumpf.Coparoo.Desktop.DemoApp.Tests/Tests.cs).
```
    IDemoApp app = ProcessObject.Resolve<IDemoApp>();
    app.Configuration.WaitTimeout = TimeSpan.FromMinutes(1);
    app.Configuration.PositiveWaitTimeout = TimeSpan.FromMilliseconds(500);
    app.On<IMainWindow>().VisibleOnScreen.WaitFor(TimeSpan.FromMinutes(1));
    app.On<IMainWindow>().ResetButton.Click();
    app.On<IMainWindow>().IncrementButton.Text.WaitFor(text => text == "0", "Text is '0'");
    
    foreach (var i in Enumerable.Range(1, 4))
    {
        app.On<IMainWindow>().IncrementButton.Click();
        app.On<IMainWindow>().IncrementButton.Text.WaitFor(text => text == i.ToString(), "Caption should be " + i);
    }
    
    app.On<IMainWindow>().ResetButton.Click();
    app.On<IMainWindow>().IncrementButton.Text.WaitFor(text => text == "0", "Text is '0'");
```

## Contributors
Main development by Alexander Kaiser (alexander.kai...@trumpf.com or alexander.kai...@cs.ox.ac.uk).

Ideas and contributions by many more including
- Daniel Knorreck, Gerald Waldherr / *Additive Manufacturing, TRUMPF Laser- und Systemtechnik GmbH, Ditzingen*
- Jochen Lange, Matthias Wetzel, Markus Ament, Bernd Gschwind, Bernd Theissler, Andreas Alavi, Sebastian Mayer, Daniel Boeck / *TRUMPF Werkzeugmaschinen GmbH + Co. KG, Ditzingen*
- Igor Mikhalev / *Trumpf Laser Marking Systems AG, Schweiz*
- Thanikaivel Natarajan / *India Metamation Software P. Ltd., India*
- Nol Zefaj, Nils Engelbach, Phi Dang, Mattanja Kern, Felix Eisele / *AXOOM GmbH, Karlsruhe*
- Manuel Pfemeter / *AIT – Applied Information Technologies GmbH & Co. KG, Stuttgart*
- Marie Jeutter / *Hochschule Karlsruhe*

## License
Copyright (c) TRUMPF Werkzeugmaschinen GmbH + Co. KG. All rights reserved. 2016, 2017, 2018.

Licensed under the [Apache License Version 2.0](LICENSE) License.

Coparoo uses the [Stashbox](https://github.com/z4kn4fein/stashbox) dependency injection framework (MIT license) by Peter Csajtai, and SmartBear's TestLeft package from https://www.nuget.org/packages/SmartBear.TestLeft; see term of use: <https://smartbear.com/terms-of-use>.

[logo]: ./Resources/logo.png "coparoo dektop logo"
[appVeyorBuildStatus]: https://ci.appveyor.com/api/projects/status/github/trumpf-code/Trumpf.Coparoo.Desktop "Build Status (AppVeyor)"
