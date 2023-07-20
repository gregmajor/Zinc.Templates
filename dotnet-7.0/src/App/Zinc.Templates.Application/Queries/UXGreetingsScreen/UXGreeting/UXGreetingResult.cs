using System;

namespace Zinc.Templates.Application.Queries.UXGreetingsScreen.UXGreeting
{
    /// <summary>
    /// result of the ux query.
    /// </summary>
    public record UXGreetingResult
    {
        /// <summary>
        /// UX screen permission.
        /// </summary>
        public bool CanUpdateGreetings { get; init; }

        /// <summary>
        /// The Message.
        /// </summary>
        public string Message { get; init; }

        /// <summary>
        /// The Timestamp.
        /// </summary>
        public DateTimeOffset Timestamp { get; init; }

        /// <summary>
        /// Another value that was queried.
        /// </summary>
        public string ETag { get; init; }
    }
}
