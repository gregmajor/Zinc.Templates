namespace RedLine.Data
{
    /// <summary>
    /// Defines the wellknown RedLine connection string names.
    /// </summary>
    public static class ConnectionStringNames
    {
        /// <summary>
        /// The name of the Postgres connection string.
        /// </summary>
        public static readonly string Postgres = nameof(Postgres);

        /// <summary>
        /// The name of the RabbitMQ connection string.
        /// </summary>
        public static readonly string RabbitMQ = nameof(RabbitMQ);

        /// <summary>
        /// The name of the ElasticSearch connection string.
        /// </summary>
        public static readonly string ElasticSearch = nameof(ElasticSearch);
    }
}
