using System;
using System.Diagnostics.CodeAnalysis;
using FluentMigrator;

namespace Zinc.Templates.Data.Migrations.Migrations
{
    /// <summary>
    /// Migration for sample code.
    /// </summary>
    [Migration(2022010602)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Migrations are name according to our standards.")]
    public class Migration_2022010602_Greetings : Migration
    {
        private static readonly string SchemaName = "app";
        private static readonly string TableName = "greetings";

        /// <summary>
        /// Performs the migration.
        /// </summary>
        public override void Up()
        {
            Create.Schema(SchemaName);

            Create.Table(TableName).InSchema(SchemaName)
                .WithColumn("greeting_id").AsGuid().PrimaryKey($"{TableName}_pkey")
                .WithColumn("message").AsAnsiString().NotNullable()
                .WithColumn("tenantid").AsAnsiString().NotNullable()
                .WithColumn("etag").AsAnsiString().NotNullable()
                .WithColumn("timestamp").AsDateTimeOffset().NotNullable();

            Insert.IntoTable(TableName).InSchema(SchemaName)
                .Row(new
                {
                    tenantid="ExampleTenant",
                    greeting_id = Guid.NewGuid(),
                    message = "Hello!",
                    etag = Guid.NewGuid().ToString(),
                    timestamp = DateTimeOffset.UtcNow,
                })
                .Row(new
                {
                    tenantid="ExampleTenant",
                    greeting_id = Guid.NewGuid(),
                    message = "¡Hola!",
                    etag = Guid.NewGuid().ToString(),
                    timestamp = DateTimeOffset.UtcNow,
                })
                .Row(new
                {
                    tenantid="ExampleTenant",
                    greeting_id = Guid.NewGuid(),
                    message = "Bonjour!",
                    etag = Guid.NewGuid().ToString(),
                    timestamp = DateTimeOffset.UtcNow,
                })
                .Row(new
                {
                    tenantid="ExampleTenant",
                    greeting_id = Guid.NewGuid(),
                    message = "Olá!",
                    etag = Guid.NewGuid().ToString(),
                    timestamp = DateTimeOffset.UtcNow,
                })
                .Row(new
                {
                    tenantid="ExampleTenant",
                    greeting_id = Guid.NewGuid(),
                    message = "Ciao!",
                    etag = Guid.NewGuid().ToString(),
                    timestamp = DateTimeOffset.UtcNow,
                })
                ;
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
