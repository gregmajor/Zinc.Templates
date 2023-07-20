namespace RedLine.Application
{
    /// <summary>
    /// Provides an interface by which the validator can be queried to provide the correct rule set.
    /// </summary>
    /// <typeparam name="T">Type to validate.</typeparam>
    public interface IDetermineValidationRuleSetByConfig<in T>
    {
        /// <summary>
        /// Returns which ruleSets to run per the FluentValidation documentation.
        /// </summary>
        /// <param name="config">Config to validate.</param>
        /// <returns>The rule set(s) to validate.</returns>
        string[] WhichRuleSets(T config);
    }
}
