using System.Globalization;
using FluentAssertions;
using Xunit;

namespace RedLine.UnitTests.Domain.CulturesTests
{
    public class CultureInfoShould
    {
        [Fact]
        public void HaveCurrentCulture()
        {
            CultureInfo.CurrentCulture.Should().NotBeNull();
            CultureInfo.CurrentCulture.Name.Should().NotBeNull();
        }

        /// <summary>
        /// This test verifies that the current environment is using ICU, rather than NLS.
        /// https://docs.microsoft.com/en-us/dotnet/core/extensions/globalization-icu .
        /// </summary>
        [Fact]
        public void UseIcuMode()
        {
            var sortVersion = CultureInfo.InvariantCulture.CompareInfo.Version;
            var bytes = sortVersion.SortId.ToByteArray();
            var version = bytes[3] << 24 | bytes[2] << 16 | bytes[1] << 8 | bytes[0];
            var icuMode = version != 0 && version == sortVersion.FullVersion;

            icuMode.Should().Be(true);
        }

        /// <summary>
        /// This test verifies that the current culture is not invariant.
        /// https://github.com/dotnet/runtime/blob/main/docs/design/features/globalization-invariant-mode.md .
        /// </summary>
        [Fact]
        public void NotBeInvariant()
        {
            (!Equals(CultureInfo.CurrentCulture, CultureInfo.InvariantCulture)).Should().BeTrue();
        }
    }
}
