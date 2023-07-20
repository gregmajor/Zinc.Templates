using System.Runtime.CompilerServices;
using Xunit;

// Give Moq visibility to our internal classes
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]

// CollectionBehavior.CollectionPerAssembly ensures all tests in the assembly are run in serial.
// If only some tests are required to run in serial, you may remove the first parameter here
// and define those tests in a separate collection.
// See "Custom Test Collections" section in https://xunit.net/docs/running-tests-in-parallel.html
