📖 **CloudMine C# Coding Guidelines**


[[_TOC_]]

## Guideline Presentation
The guidelines are organized as simple recommendations using **Do**, **Consider**, **Avoid**, and **Do not**. 
Each guideline describes either a good or bad practice and all have a consistent presentation. 
Good practices have a check (✔️) in front of them, and bad practices have an *x* (❌) in front of them. 
The wording of each guideline also indicates how strong the recommendation is. Below are samples usage of each 
recommendation.

**Do** guideline is one that should be always followed:

✔️	**Do** name custom attribute classes with the suffix `Attribute`.

```csharp
public class ObsoleteAttribute : Attribute { ... }
```

On the other hand, **Consider** guidelines should be generally followed, but if you fully understand the reasoning behind a guideline and have a good reason 
to not follow it anyway, you should not feel bad about breaking the rules:

✔️	**Consider** defining a struct instead of a class if instances of the type are small and commonly short lived or are not commonly embedded in other objects.

Similarly, **Do not** guidelines indicate something you should not do, unless there is a good reason for an exception:

❌	**Do not** use read-only array fields. The field itself is read-only and can't be changed, but elements in the array can be changed. 

Less strong, **Avoid** guidelines, indicate that something is generally not a good idea, but there are known cases where breaking the rule makes sense:

❌	**Avoid** using `ICollection<T>` or `ICollection` as a parameter just to access the `Count` property. 


## Design Guidelines

✔ **Do** follow the guidelines in this document for both **public** and **non-public** code. Applying design guidelines to non-public code yields the same benefits as following them for public code, 
driving consistency across these two domains for people working within the team and parners who consume our services. 

## File Organization

### File Names

❌ **Do not** have more than one public or internal type in a source file, unless they differ only in the number of generic parameters or one is nested in the other. 
For example, it is correct to have `ServiceHost` and `ServiceHost<T>` class in the `ServiceHost.cs` file, but it is not correct to have `ServiceHost` and `IServiceHost` in the same file. 

✔ **Do** name the source file with the name of the type it contains. For example, `String` class should be in `String.cs` file and `List<T>` class should be in List.cs file.

✔ **Do** use the same casing when mapping the type name to file name and take care to ensure that casing is preserved when adding files to source repository.

Correct:

`ServiceUtility.cs` holds the `ServiceUtility` type

Incorrect:

`serviceutility.cs` holds the `ServiceUtility` type

### File Ownership

❌ **Do not** track source file ownership by listing the owner of the file in a comment within the source file. This fine-grained approach has a tendency to fall out of sync with reality. 

### File Content

✔ **Do** use <TBD Namespace> as the root namespace prefix. 

✔ **Do** include the following copyright banner at the beginning of every source file:

```c#
//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------
```

✔ **Do** place the using directives outside the namespace declaration at the top of each source code file:

```csharp
using System;
using System.Text;

namespace Microsoft.CloudMine.<InsertServiceNameHere> 
{
    ...
}
```

✔ **Do** group class members into the following sections in the specified order to improve logical structure and readability: 
-   Static & Instance
-   Constants
-   Fields
-   Constructors
-   Properties
-   Methods and Events
-   Private Nested Types

✔️ **Consider** Alphabetic order for Constants, Fields, Properties, Methods, and Events

✔ **Do** organize method overloads from the simplest to the most complex.

❌	**Do not** use aliases for entire namespaces. For example, do not use:

```csharp
using Xml = System.Xml;
```

The use of aliases for specific class names, particularly to avoid ambiguity, is permitted.

❌	**Do not** declare namespaces not used within the file.  

*Tip*: Use Visual Studio feature `Organize Usings > Remove and Sort` context menu to sort and remove unused namespaces.

✔️ **Consider** placing ‘System’ directives first when sorting usings. 

*Tip*: Check this VS Option under `Text Editor > C# > Advanced`

## Formatting

### Code Comments

✔️ **Do** use code comments to document code. The use of comments is intended to augment the code, reflect its purpose, and enable maintenance of the code over time. For example, consider commenting:

-   Pre-conditions not evident in code, e.g. thread-safety assumptions
-   Complex algorithms
-   Complex flow of control, e.g. chained asynchronous calls
-	Dependencies on global state
-	Security considerations

❌ **Avoid** using comments that **repeat self-commenting information** found in many code structures. For example, avoid commenting:
-	Assertions
-	Well-understood patterns (e.g. enumerators)
-	Clean code

✔️ **Consider** using `//` commenting style for both single and multi-line prose comments. For example:

```csharp
// This method assumes synchronization is done by the caller
Byte[] ReadData( Stream stream )
```

```csharp
// This AsyncResult implementation allows chaining of two
// asynchronous operations. It executes the second
// operation only after the first operation completes.
```

❌ **Avoid** leaving unused code in a file, for example by commenting it out. There are occasions when leaving unused code in a file is useful (for example implementing a single feature over multiple check-ins), but this should be rare, short in duration, and accompanied by an explanatory comment.

❌ **Avoid** using `#if` and `#endif` for purposes other than conditional compliation. 

✔️ **Do** use `TODO` comments as a convenient way to reference issues tracked by bugs in TFS. 

A `TODO` comment should have the following format:

```csharp
//
// TODO, <alias>, [<database>,] <bugNumber>, <comment>
//
```

where `<alias>` is the alias of the person creating the comment, `<database>` is the TFS server name (use when necessary to disambiguate bug numbers from different servers), and `<bugNumber>` is the TFS BugId. 

For example:

```csharp
//
// TODO, dougo, OSGS, 13440, add algorithm suite
//
```

❌ **Do not** use comments to capture ideas for changes and enhancements in future releases of the product. These should be tracked by issues in TFS and design documents. For example, do not include a comment like this one:

```csharp
//
// FUTURE consider supporting RSAOAEP in addition to RSAV1.5
//
```

### Line Breaks and Wrapping

✔️ **Do** limit each line of text in a source file to about 120 columns. 

✔️ **Do** place each parameter in its own line if method calls or declarations are running long. For example:

```csharp
CallMethod(arg1,
           arg2,
           arg3,
           arg4,
           arg5);
```

```csharp
public void SomeMethod(int p1,
                       int p2,
                       int p3,
                       int p4)
{
}
```

### Code Regions

❌ **Do not** use use #region directives to identify blocks of code. 

### Indents and Tabs

✔️ **Do** use 4 consecutive space characters for indents.

❌ **Do not** use the tab character for indents. 

❌ **Do not** use byte order marks at the beginning of files.

✔️ **Do** indent contents of code blocks.

```csharp
if (someExpression)
{
    DoSomething();
}
```

✔️ **Do** indent case blocks even if not using braces.

```csharp
switch (someExpression)
{
   case 0:
       DoSomething();
       break;
      ...
}
```

### Braces

✔️ **Do** use braces with all if, do, else, for, foreach, while,  and lock statements. 

❌ **Avoid** omitting braces, even if the language allows it. Braces should not be considered optional. Even for single statement blocks, you should use braces. This increases code readability and maintainability.

```csharp
for (int i = 0; i < 100; i++)
{
    DoSomething(i);
}
```

An exception to the rule is braces in case statements. These braces can be omitted as the case and break statements indicate the beginning and the start of the block.

```csharp
case 0:
    DoSomething();
    break;
```

✔️ **Do** place braces in their own line. **Do** align the closing brace with the opening brace when the brackets are in their own lines. The exceptions to this guideline are:
-   For single statement blocks of property accessors, consider using opening and closing brace on the same line as the statement.
-   For single accessor properties, consider having all braces on the same line.

Correct:

```csharp
if (someExpression) 
{
    DoSomething();
}
```
Incorrect:

```csharp
if (someExpression) {
    DoSomething();
}
```

Correct:

```csharp
public int Foo
{
    get { return foo; }
    set { foo = value; }
}
```

Correct:

```csharp
public int Foo { get { return foo; } }
```

### Spacing

✔️ **Do** use a single space after a comma between function arguments and in expressions.

Correct:

```csharp
Console.In.Read(myChar, 0, 1);
while (i < some_maximum)
```

Incorrect:

```csharp
Console.In.Read(myChar,0,1);
while (i < some_maximum)
```

❌ **Do not** use spaces between a function name and parenthesis.

Correct: `CreateFoo()`

Incorrect: `CreateFoo ()`

❌ **Do not** use spaces inside brackets.
Correct: `x = dataArray[index];`
Incorrect: `x = dataArray[ index ];`

✔️ **Do** use a single space between flow control keywords and expressions.

Correct: `while (x == y)`

Incorrect: `while(x==y)`

✔️ **Do** use a single space before and after comparison operators

Correct: `if (x == y)`

Incorrect: `if (x==y)`

❌ **Do not** use a single space after type cast parentheses.

Correct: `var foo = (Foo)bar;`

Incorrect: `var foo = (Foo) bar;`

## Syntax

### Visibility Modifiers

✔️ **Do** use use explicit visibility modifiers in all cases (even when the default is correct). 

Incorrect:

```csharp
internal class Car
{
    public Car(string name) 
    {
        // ...
    }

    private BurnGas()
    {
        // ...
    }

    public Repair() 
    {
        // ...
    }
}
```

Incorrect:

```csharp
class Car
{
    public Car(string name) 
    {
        // ...
    }

    BurnGas()
    {
        // ...
    }

    public Repair() 
    {
        // ...
    }
}
```

### var Keyword

✔️ **Consider** using var when declaring variables if the right-hand side assignment is explicit:

```csharp
var customer = new Customer();
```

✔ **Do** use explicitly typed variables when the right side assignment does not explicitly show the type.

Correct:

```csharp
IEnumerable<Purchase> purchases = customer.GetPurchases();
```

Incorrect: 

```csharp
var purchases = customer.GetPurchases();
```

### Interpolated Strings
✔ **Consider** using interpolated strings for improved readability over `string.Format()`. For example:

```csharp
string name = "Foo";
Console.WriteLine($"Name = {name}");
```

## Literals

### String Literals
✔ **Do** use verbatim string literals (`@`) when assigning pathnames to variables or constants.

Correct:

```csharp
string dataDirectory = @"d:\data\";
```

Incorrect:

```csharp
string dataDirectory = "d:\\data\\";
```

## Naming

Follow naming guidelines for all public, private, and internal members. Highlights of these are listed in the following subsections. 

### Members

❌ **Do not** use Hungarian notation 

❌ **Do not** use either public or protected fields per the .NET Framework Guidelines.

✔️ **Do** use either the `_` prefix or "this." for private members variables.

✔️ **Consider** using `readonly` fields when possible

✔️ **Do** use camelCasing for member variables 

✔️ **Do** use camelCasing for parameters 

✔️ **Do** use camelCasing for local variables 

✔️ **Do** use PascalCasing for function, property, event, const fields, and class names 

✔️ **Do** prefix interfaces names with `I` 

❌ **Do not** prefix enums, classes, or delegates with any specific letter or prefix

✔️ **Do** use C# aliases instead of CLR types for declarations. 
For example, use `int` instead of `Int32`, `object` instead of `Object`, and `string` instead of `String`. 
Using C# aliases enables syntax highlighting for these constructs in Visual Studio 
and therefore improves code readability.

✔️ **Consider** using the aliases when invoking static methods on the built-in types. 
For example, use `string.IsNullOrEmpty` rather than `String.IsNullOrEmpty`

❌ **Avoid** using fully qualified type names unless necessary to disambiguate between 
two types with the same name from different namespaces used inside the module. 

Correct:

```csharp
void DoWork(Hashtable channels);
```

Incorrect:

```csharp
void DoWork(System.Collections.Hashtable channels);
```

Correct:

```csharp
void Send(System.Runtime.Remoting.Channels.IChannel channel);
void Send(System.ServiceModel.Channels.IChannel channel);
```

### Generic Type Names
✔ **Do** name generic type parameters with descriptive names, 
unless a single letter name is completely self explanatory and a descriptive name would not add value.

```csharp
public interface ISessionChannel<TSession> { ... }
public delegate TOutput Converter<TInput,TOutput>(TInput from);
public class List<T> { ... }
```

✔️ **Consider** using `T` as the type parameter name for types with one single letter generic type parameter.

```csharp
public int IComparer<T> { ... }
public delegate bool Predicate<T>( T item );
public struct Nullable<T> where T:struct { ... }  
```

✔️ **Do** prefix descriptive type parameter names with `T`.

```csharp
public interface ISessionChannel<TSession> where TSession : ISession
{
   TSession Session { get; }
}
```

✔️ **Consider** indicating constraints placed on a type parameter in the name of parameter. For example, a parameter constrained to `ISession` may be called `TSession`.

## Class Design

### Properties vs Methods
✔ **Do** use properties even when the member is a simple accessor to a logical backing store.

✔ **Do** use methods if:
-   The operation is a conversion (such as `Object.ToString()`). 
-	The getter has an observable side effect. 
-	Calling the member twice in succession results in different results. 
-	Order of execution is important.
-	If member is static but returns a mutable value.
-	The operation may throw an handleable exception.
-	The operation may be significantly slower than a simple field access.

### Properties

✔ **Do** use auto-implemented properties when possible. 

✔ **Consider** using auto-property initializers when no additional logic is required for initialization.

```csharp
class Customer
{
    public double TotalPurchases { get; set; }
    public string Name { get; set; }
    public int CustomerID { get; set; }
    public IList<Product> PurchasedProducts { get; } = new List<Product>();
}
```

✔️ **Do** preserve the previous value if a property set throws an exception. 

✔️ **Do** allow properties to be set in any order. Properties should be stateless respective to other properties. 

❌ **Do not** use write-only properties.

✔️ **Do** use static const fields instead of properties where the value is a global constant.

```csharp
public static struct Int32
{
    public const int MaxValue = 2147483647;
    public const int MinValue = -2147483648;
}
```

### Indexed Properties

✔️ **Do** use only one indexed property per class and make it the default property for that class.

✔️ **Do** use the name “Item” for indexed properties unless there is obviously a better name. (i.e. `Chars` property on `String`)

✔️ **Do** use indexed properties when the logical backing store is an array.

### Constructors

✔️	**Do** mark a class static if it only contains static members.

✔️	**Do** minimal work in the constructor. Delay the cost of any initialization work for as long as possible. 

✔️	**Do** provide a protected (family) constructor for subclass-able types, if you wish to prevent instances of the base class from being constructed.

### Contracts

✔ **Do** use the `JsonProperty` attribute when defining serializable data contract members, while specifying the name explicitly. For example:

```csharp
[JsonProperty(PropertyName = "id")]
public string Id { get; set; }
```

✔️ **Do** use parameters in constructors as shortcuts for setting properties. This becomes especially important for remoteable or WebService (SOAP) classes. There should be no difference in semantics between using the empty constructor followed by some property sets and using a constructor with multiple arguments.

## Error Handling

### Throwing Exceptions

✔️ **Do** throw exceptions only in exceptional cases. 

✔️ **Do** design classes such that in the normal course of use there will never be an exception thrown. 
For example, a `FileStream` class exposes a way to determine if the end of file has been reached, 
which allows a user to avoid the exception that will be thrown if the developer reads past the end of the file.

❌ **Do not** use exceptions for normal or expected errors. 

❌ **Do not** use exceptions for normal flow of control.

✔️ **Do** use standard exceptions for common error conditions.  For example:
- `InvalidOperationException`: thrown if the property set or method call is not appropriate given the objects current state. 
- `ArgumentException`: thrown when bad parameters are detected. 
- `ArgumentNullException`: thrown when a parameter is passed as null but the method does not allow it. 
- `ArgumentOutOfRangeException`: thrown by a methods which verify arguments are in a given range.

## Performance

❌ **Do not** return a LINQ iterator from a function. This can lead to serious performance issues.

❌ **Do not** use LINQ delayed execution to avoid multiple enumeration. 
Use `ToArray()` or `ToList()` instead for evaluating queries.

# Code Analysis

✔️ **Do** place the `SuppressMessage` attribute where the violation occurs. Exception is given when many suppress attributes decrease readability of code.

# Test Guidelines

This section describes guidelines for authoring unit tests and functional tests. 

## Assembly Factoring

By default, each product assembly should have a corresponding test assembly that contains the tests that verify functional correctness for that assembly. The test assembly should have the same name as the assembly, with the word `Tests` as a suffix. 

## Coding Guidelines

The following coding guidelines should be followed for all tests.

### General

✔️ **Do** follow the product coding guidelines (including FxCop) for all test code.  Ease of long term maintenance is a key.  As with all software, the cost of tests is not in writing but in maintaining for many years to come.

✔️ **Do** name the namespaces by the name of the containing assembly. This is helpful because by just looking at a stack trace one can tell in which test is involved.

✔️ **Do** add the test assembly as a friend assembly to the product assembly if you need to use internal types. For example:

```csharp
[assembly: InternalsVisibleTo( "MyApplication.Test" )]
```

✔️ **Do** add multi-threaded tests for all components that are expected to be thread-safe.

✔️ **Do** prefer functional tests to unit tests, all other things being equal.  Functional tests are more loosely coupled to the implementation details, and therefore will need to change less over time.

❌ **Do not** use reflection for testing purposes, unless there is no other choice.  Use friend assemblies instead. Reflection test code is brittle and hard to keep in sync with product code.

❌ **Do not** share state between multiple test cases even if they are defined as part of the same test class.

❌ **Do not** validate test results based on localizable strings.  All tests should work equally well on all locales.

❌ **Do not** add cross-test build, deployment or execution-time dependencies. 

### Writing Tests

✔️ **Do** use the MSTest `Assert` class to verify invariants and results in your test.

✔️ **Do** throw exceptions or allow exceptions to propagate upwards when test setup failures occur.

✔️ **Consider** providing a message to the Assert methods when the conditions may be confusing (e.g. Asert(true)). This will make it easier to determine the reason for failure from an error in a test log file. Use a short, succinct message.

✔️ **Do** use `System.Diagnostics` for all debug tracing. This will ensure that the target trace listener can be changed via configuration.

### Code Sharing

✔️ **Do** use NuGet for cross-service code dependencies. 

❌ **Do not** use any NuGet Gallery other than the approved packages feeds.

❌ **Do not** check in NuGet packages. Use NuGet restore.

### Performance

✔ **Consider** grouping tests which require similar setup and cleanup tasks in the same test class.

❌ **Avoid** consolidating tests for performance reasons if this will also reduce maintainability and/or increase coupling between tests.

❌ **Do not** use active waiting in the tests (e.g. `Thread.Sleep`). Use other thread or process synchronization mechanisms instead (e.g. `AutoResetEvent`).

### Reliability

✔ **Do** clean up all side-effects for a test after execution has completed. This will help avoid cascading test failures and will make tests easily re-runnable.

### Test Class Naming and Initialization

✔ **Do** write only one test class per file and it should have the same name as the file name. This makes it easy to find the file for a given test name. 

✔ **Do** add `Tests` suffix to the test class names.  This makes it easy to spot files which contain test cases as opposed to other classes used in the tests.

Correct: `ConfigurationCrudTests`

Incorrect: `TestConfigurationCrud`

✔️ **Do** name the test class initialize and cleanup methods `ClassInitialize` and `ClassCleanup`.

✔️ **Do** make the test class initialize and cleanup methods public static.

✔️ **Do** name individual test case initialization and cleanup methods `TestInitialize` and `TestCleanup`.

✔️ **Do** place initialize and clean-up methods before all other methods in the following order:
- `ClassInitialize`
- `ClassCleanup`
- `TestInitialize`
- `TestCleanup`

✔️ **Do** make test case initialization and cleanup methods public non-static. 

✔ **Do** give your test a descriptive name because they explicitly express the intent of the 
test when analyzing test results. The name of your test should consist of three parts:
- The name of the method being tested
- The scenario under which it's being tested
- The expected behavior when the scenario is invoked

Correct:
```csharp
[TestMethod]
public void Add_SingleNumber_ReturnsSameNumber()
{
    var stringCalculator = new StringCalculator();
    int actual = stringCalculator.Add("0");
    Assert.Equal(0, actual);
}
```

Incorrect: 
```csharp
[TestMethod]
public void Test_Single()
{
    var stringCalculator = new StringCalculator();
    int actual = stringCalculator.Add("0");
    Assert.Equal(0, actual);
}
```

### Test Case Attributes

✔ **Consider** using attributes to group tests based on their assigned categories. For example:

```csharp
[TestCategory("Nightly")]
```

✔ **Consider** adding a `Description` attribute for each test method with a one line description of what the test is doing.

## Platform Interoperability Classes

✔️ **Do** use `NativeMethods` as the name of the class and file which contains interop wrapper methods 
(i.e. decorated with `DllImportAttribute`) around native APIs for which the CAS stack walk is to be performed. 
These wrappers are transparent, i.e. can be freely used anywhere in the code base because no permission modification 
occurs and no stack walk adjustments are made. 

```csharp
public static class NativeMethods 
{
    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();
}
```

✔️	**Do** use `UnsafeNativeMethods` as the name of the class and file which contains interop wrapper methods 
(i.e. decorated with `DllImportAttribute`) around potentially dangerous native APIs for which 
`UnmanagedCodePermission` check is suppressed. Since the APIs are potentially dangerous, 
the usage of the wrappers from `UnsafeNativeMethods` should undergo special security scrutiny for potential threats. 

```csharp
[SuppressUnmanagedCode]
public static class UnsafeNativeMethods 
{	
    [DllImport("kernel32.dll")]
    static extern bool CopyFile(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);
}
```

✔️ **Do** use `SafeNativeMethods` as the name of the class and file which contains interop wrapper methods 
(i.e. decorated with `DllImportAttribute`) around safe native APIs for which `UnmanagedCodePermission` 
check is suppressed. Since the APIs are considered safe, the usage of the wrappers from `SafeNativeMethods` 
need not undergo special security scrutiny for potential threats even through the stack walk is not performed. 

```csharp
[SuppressUnmanagedCode]
public class SafeNativeMethods 
{
    private SafeNativeMethods() {}

    [DllImport(“user32”)]
    internal static extern void MessageBox(string text);
}
```

✔️ **Do** make interop classes internal. 

✔️ **Do** make all methods within interop classes internal. 

✔️ **Do** declare interop classes as static.
