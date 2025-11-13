using Xunit.Abstractions;
using Xunit.Sdk;

namespace BrainBay.IntegrationTests.Infrastructure
{
    public class PriorityOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            string assemblyName = typeof(TestPriorityAttribute).AssemblyQualifiedName!;
            var sorted = new SortedDictionary<int, List<TTestCase>>();

            foreach (var testCase in testCases)
            {
                var priority = testCase.TestMethod.Method
                    .GetCustomAttributes(assemblyName)
                    .FirstOrDefault()
                    ?.GetNamedArgument<int>("Priority") ?? 0;

                if (!sorted.TryGetValue(priority, out var list))
                {
                    list = new List<TTestCase>();
                    sorted.Add(priority, list);
                }

                list.Add(testCase);
            }

            return sorted.SelectMany(x => x.Value);
        }
    }
}
