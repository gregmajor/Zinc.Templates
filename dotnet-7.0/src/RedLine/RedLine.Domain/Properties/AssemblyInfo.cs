using System.Runtime.CompilerServices;

// Give Moq visibility to our internal classes
[assembly: InternalsVisibleTo("RedLine.Application")]
[assembly: InternalsVisibleTo("RedLine.Data")]
[assembly: InternalsVisibleTo("RedLine.UnitTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // Used for mocking
