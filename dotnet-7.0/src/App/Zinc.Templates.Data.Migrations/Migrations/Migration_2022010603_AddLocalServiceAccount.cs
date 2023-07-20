using System;
using System.Diagnostics.CodeAnalysis;
using FluentMigrator;

namespace Zinc.Templates.Data.Migrations.Migrations
{
    /// <summary>
    /// A migration that adds the localservice account.
    /// </summary>
    [Migration(2022010603)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Migrations are name according to our standards.")]
    [Tags(TagBehavior.RequireAny, "Development", "docker-local", "docker-circleci")]
    public class Migration_2022010603_AddLocalServiceAccount : ForwardOnlyMigration
    {
        private static readonly string SchemaName = "authz";

        /// <inheritdoc />
        public override void Up()
        {
            Insert.IntoTable("grant").InSchema(SchemaName)
                .Row(new
                {
                    user_id = "local@redline.services",
                    full_name = "Local Service Account",
                    tenant_id = "*",
                    grant_type = "*",
                    qualifier = "*",
                    expires_on = (DateTimeOffset?)null,
                    granted_by = "system",
                    granted_on = DateTimeOffset.UtcNow,
                });
        }
    }
}
