using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;

namespace RedLine.Extensions.Hosting.Messaging.Behaviors
{
    /// <summary>
    /// Replaces the qualified assembly name with the full type name only.
    /// </summary>
    internal class OutgoingPublishFullTypeNameOnlyBehavior : Behavior<IOutgoingPhysicalMessageContext>
    {
        /// <summary>
        /// This behavior the qualified assembly name with the full type name.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <param name="next">The next behavior in the pipeline.</param>
        /// <returns>A <see cref="Task"/> used to await the operation.</returns>
        public override async Task Invoke(IOutgoingPhysicalMessageContext context, Func<Task> next)
        {
            if (context.Headers.TryGetValue(Headers.EnclosedMessageTypes, out var types))
            {
                var enclosedTypes = types.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < enclosedTypes.Length; i++)
                {
                    var index = enclosedTypes[i].IndexOf(',');
                    if (index > -1)
                    {
                        enclosedTypes[i] = enclosedTypes[i].Substring(0, index);
                    }
                }

                context.Headers[Headers.EnclosedMessageTypes] = string.Join(";", enclosedTypes);
            }

            await next().ConfigureAwait(false);
        }
    }
}
