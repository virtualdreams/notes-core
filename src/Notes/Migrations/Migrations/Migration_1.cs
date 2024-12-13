using FluentMigrator;

namespace Notes.Migrations.Migrations
{
	[Migration(1, "Create schema.")]
	public class Migration_1 : Migration
	{
		public override void Up()
		{
			IfDatabase("postgresql")
				.Execute.EmbeddedScript("Migrations.Schema.Postgres.migration_1.sql");

			IfDatabase("mysql")
				.Execute.EmbeddedScript("Migrations.Schema.MySql.migration_1.sql");
		}

		public override void Down()
		{
			throw new System.NotImplementedException();
		}
	}
}