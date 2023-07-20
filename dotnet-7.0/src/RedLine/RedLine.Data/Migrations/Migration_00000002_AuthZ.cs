using System;
using System.Diagnostics.CodeAnalysis;
using FluentMigrator;

namespace RedLine.Data.Migrations
{
    /// <summary>
    /// Adds the authz schema and tables for managing authorization.
    /// </summary>
    [Migration(00000002)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Migrations are name according to our standards.")]
    public class Migration_00000002_AuthZ : Migration
    {
        private static readonly string SchemaName = "authz";

        /// <inheritdoc />
        public override void Up()
        {
            Create.Schema(SchemaName);

            Create.Table("grant").InSchema(SchemaName)
                .WithColumn("sid").AsInt64().NotNullable().PrimaryKey("grant_pkey").Identity()
                .WithColumn("user_id").AsAnsiString().NotNullable()
                .WithColumn("full_name").AsAnsiString().NotNullable()
                .WithColumn("tenant_id").AsAnsiString().NotNullable()
                .WithColumn("grant_type").AsAnsiString().NotNullable()
                .WithColumn("qualifier").AsAnsiString().NotNullable()
                .WithColumn("expires_on").AsDateTimeOffset().Nullable()
                .WithColumn("granted_by").AsAnsiString().NotNullable()
                .WithColumn("granted_on").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.UniqueConstraint("grant_key")
                .OnTable("grant")
                .WithSchema(SchemaName)
                .Columns(new[] { "user_id", "tenant_id", "grant_type", "qualifier" });

            Create.Table("activity_group")
                .InSchema(SchemaName)
                .WithColumn("tenant_id").AsAnsiString().NotNullable()
                .WithColumn("name").AsAnsiString().NotNullable()
                .WithColumn("activity_name").AsAnsiString().NotNullable()
                ;

            Create.UniqueConstraint("activity_group_key")
                .OnTable("activity_group")
                .WithSchema(SchemaName)
                .Columns(new[] { "tenant_id", "name", "activity_name" });

            Insert.IntoTable("grant").InSchema(SchemaName)
                .Row(new
                {
                    user_id = "zn-templates@redline.services",
                    full_name = "Zinc.Templates Service Account",
                    tenant_id = "*",
                    grant_type = "*",
                    qualifier = "*",
                    expires_on = (DateTimeOffset?)null,
                    granted_by = "system",
                    granted_on = DateTimeOffset.UtcNow,
                });

            Create.Table("grant_history").InSchema(SchemaName)
                .WithColumn("sid").AsInt64().NotNullable().PrimaryKey("grant_history_pkey").Identity()
                .WithColumn("user_id").AsAnsiString().NotNullable().Indexed("grant_history_user_id_idx")
                .WithColumn("full_name").AsAnsiString().NotNullable()
                .WithColumn("tenant_id").AsAnsiString().NotNullable()
                .WithColumn("grant_type").AsAnsiString().NotNullable()
                .WithColumn("qualifier").AsAnsiString().NotNullable()
                .WithColumn("expires_on").AsDateTimeOffset().Nullable()
                .WithColumn("granted_by").AsAnsiString().NotNullable()
                .WithColumn("granted_on").AsDateTimeOffset().NotNullable()
                .WithColumn("revoked_by").AsAnsiString().Nullable()
                .WithColumn("revoked_on").AsDateTimeOffset().Nullable()
                ;
        }

        /// <inheritdoc />
        public override void Down()
        {
            Delete.Table("grant_history").InSchema(SchemaName);
            Delete.Table("grant").InSchema(SchemaName);
            Delete.Table("activity_group").InSchema(SchemaName);
            Delete.Schema(SchemaName);
        }
    }
}
