namespace RedLine.UnitTests.Domain.Model.StateMachineTests
{
    public enum TestStateReason
    {
        /// <summary>
        /// The state reason is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The state was activated.
        /// </summary>
        Activated = 2,

        /// <summary>
        /// The state was deactivated.
        /// </summary>
        Deactivated = 3,

        /// <summary>
        /// The state was reactivated.
        /// </summary>
        Reactivated = 4,
    }
}