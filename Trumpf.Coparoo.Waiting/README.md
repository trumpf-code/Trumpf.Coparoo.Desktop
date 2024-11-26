# Trumpf.Coparoo.Waiting

![logo]

Trumpf.Coparoo.Waiting is a .NET library for C# that simplifies the process of waiting for conditions in automated tests. It provides a visual dialog to indicate the current state of the condition being evaluated, making it easier for testers to understand what is happening during test execution. The dialog will display in different colors to indicate whether the condition is true, false, or requires manual intervention.

## Features

- Polls conditions and shows a visual waiting dialog.
- **Red** dialog indicates a false condition.
- **Green** dialog indicates a true condition.
- **Grey** dialog for manual intervention (e.g., safety-critical acknowledgements).
- Supports automatic and semi-automated testing.
- Improves test analysis and video clarity by showing real-time feedback.

## Installation

You can install the package via NuGet:

```bash
dotnet add package Trumpf.Coparoo.Waiting
```

## Usage

### Waiting for a Condition

You can use the `ConditionDialog.For()` method to wait for a condition to become true. It displays a dialog showing the condition's state.

```csharp
ConditionDialog.For(() => true, "Condition is true", TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(100));
```

This will wait until the condition is true or the timeout is reached. The dialog will show the current state of the condition (red for false, green for true).

### Handling Timeout and Exceptions

If the condition is false when the timeout is reached, a `WaitForTimeoutException` will be thrown with a descriptive message.

```csharp
try
{
    ConditionDialog.For(() => false, "Condition is false", TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(100));
}
catch (WaitForTimeoutException ex)
{
    Console.WriteLine(ex.Message); // Will print the timeout message
}
```

### Example with Manual Intervention

For tests that require manual intervention (e.g., user approval), the dialog will show grey with instructions:

```csharp
ConditionDialog.For(() => false, "Manual intervention required", TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(100));
```

### Repeating Conditions

You can repeat the condition check for a number of iterations, making it easy to track ongoing changes:

```csharp
Enumerable.Range(0, 50).ToList().ForEach(_ => 
    ConditionDialog.For(() => true, "Repeated condition check", TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(100))
);
```

## Test Cases

The library includes a series of tests to validate its functionality, including:

- **Positive cases**: Waiting until the condition is true without errors.
- **Negative cases**: Testing conditions where the timeout is exceeded or the condition is null/false.
- **Manual interventions**: Using the grey dialog to prompt the user for actions.
- **Nested waits**: Handling conditions with multiple nested waiting dialogs.

### Example Test Cases

- **Basic Test**: Wait until a condition is true, then continue.
  
  ```csharp
  [Test]
  public void IfTheConditionIsTrue_ThenNoExceptionIsThrown()
  {
      ConditionDialog.For(() => true, "Condition is true", TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(100));
  }
  ```

- **Timeout Test**: Timeout occurs if the condition remains false.

  ```csharp
  [Test]
  public void IfTheConditionIsFalse_ExceptionIsThrown_WithExceptionMessage()
  {
      Assert.Throws<WaitForTimeoutException>(() =>
      {
          ConditionDialog.For(() => false, "Condition is false", TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(100));
      });
  }
  ```

- **Manual Intervention Test**: Show a grey dialog for user intervention.

  ```csharp
  [Test]
  public void IfTheConditionRequiresManualIntervention_ThenGreyDialogIsShown()
  {
      ConditionDialog.For(() => false, "Manual intervention required", TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(100));
  }
  ```

## Contributing

Feel free to submit pull requests or report issues via the GitHub repository. Contributions are welcome!

## License

This library is licensed under the Apache License, Version 2.0. See the [LICENSE](LICENSE) file for more information.

[logo]: ./Resources/logo.png "coparoo waiting logo"