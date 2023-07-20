using System.Diagnostics.CodeAnalysis;
using FluentMigrator;

namespace RedLine.Data.Migrations
{
    /// <summary>
    /// A migration that add the Outbox schema and tables.
    /// </summary>
    [Migration(00000001)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Migrations are name according to our standards.")]
    public class Migration_00000001_Outbox : Migration
    {
        private static readonly string SchemaName = "outbox";
        private static readonly string TableName = "outbox";

        /// <summary>
        /// Performs the migration.
        /// </summary>
        public override void Up()
        {
            Create.Schema("outbox");

            Create.Table(TableName).InSchema(SchemaName)
                .WithColumn("sid").AsInt64().NotNullable().PrimaryKey("outbox_pkey").Identity()
                .WithColumn("id").AsGuid().NotNullable().Indexed("outbox_key").Unique()
                .WithColumn("messages").AsCustom("jsonb").NotNullable()
                .WithColumn("dispatcher_id").AsAnsiString().Nullable()
                .WithColumn("dispatcher_timeout").AsDateTimeOffset().Nullable();
        }

        /// <summary>
        /// Reverses the migration.
        /// </summary>
        public override void Down()
        {
            Delete.Table(TableName).InSchema(SchemaName);
            Delete.Schema(SchemaName);
        }
    }
}
