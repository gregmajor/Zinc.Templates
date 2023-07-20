using System.Diagnostics.CodeAnalysis;
using FluentMigrator;

namespace RedLine.Data.Migrations;

/// <summary>
/// Adds the RedLine Money type.
/// </summary>
[Migration(00000003)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Migrations are named according to our standards.")]
public class Migration_00000003_Money : Migration
{
    /// <inheritdoc />
    public override void Up()
    {
        Execute.Sql(@"
DO $$ BEGIN
CREATE TYPE redline_money AS (amount decimal(19,4), culture_name text);
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;
");
    }

    /// <inheritdoc />
    public override void Down()
    {
        Execute.Sql(@"DROP TYPE redline_money");
    }
}
