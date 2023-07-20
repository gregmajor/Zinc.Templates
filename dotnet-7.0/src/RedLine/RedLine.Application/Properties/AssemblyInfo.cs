using System.Runtime.CompilerServices;

// Give our tests visibility to our internal classes because we have to test them.
[assembly: InternalsVisibleTo("RedLine.UnitTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // Used for mocking
