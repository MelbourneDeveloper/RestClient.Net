using Xunit.Abstractions;
using Xunit.Sdk;

#pragma warning disable CS8509 // C# compiler doesn't understand nested discriminated unions - use EXHAUSTION001 instead
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line


namespace NucliaDbClient.Tests;

public class PriorityOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase =>
        testCases.OrderBy(tc =>
            tc.TestMethod.Method.GetCustomAttributes(
                    typeof(TestPriorityAttribute).AssemblyQualifiedName!
                )
                .FirstOrDefault()
                ?.GetNamedArgument<int>("Priority") ?? 0
        );
}
