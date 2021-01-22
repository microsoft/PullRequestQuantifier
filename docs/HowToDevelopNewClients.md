Three steps​

1. Load the context, if available​
1. Call Quantifier​
1. Output the results

```csharp
// 1. point to the context file (with behavior specification)​
var contextFile = "path/to/context/file/.prquantifier";​
​
// 2. quantify local git repository​

var quantifyClient = new QuantifyClient(contextFile);​
var quantifierResult = await quantifyClient.Compute("path/to/local/git/repo");​
​
// 3. output the results​
Console.WriteLine(quantifierResult.Label);​
Console.WriteLine(quantifierResult.QuantifiedLinesAdded);​
Console.WriteLine(quantifierResult.QuantifiedLinesDeleted);
```