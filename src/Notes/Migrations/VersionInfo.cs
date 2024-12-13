using FluentMigrator.Runner.VersionTableInfo;

namespace Notes.Migrations
{
	[VersionTableMetaData]
	public class VersionInfo : IVersionTableMetaData
	{
		public bool OwnsSchema => true;

		public string SchemaName => "public";

		public string TableName => "schema";

		public string ColumnName => "version";

		public string DescriptionColumnName => "description";

		public string UniqueIndexName => "ux_schema_version";

		public string AppliedOnColumnName => "applied_on";

		public bool CreateWithPrimaryKey => false;
	}
}