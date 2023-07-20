namespace RedLine.UnitTests.Domain.Model.StateMachineTests
{
    public enum TestState
    {
        /// <summary>
        /// The state is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The state is active.
        /// </summary>
        Active = 1,

        /// <summary>
        /// The state is inactive.
        /// </summary>
        Inactive = 2,
    }
}