using Microsoft.EntityFrameworkCore;
using Notes.Core.Models;

namespace Notes.Core.Data
{
	public class DatabaseContext : DbContext
	{
		public DatabaseContext(DbContextOptions options)
			: base(options)
		{ }

		public DbSet<Note> Note { get; set; }

		public DbSet<Tag> Tag { get; set; }

		public DbSet<User> User { get; set; }

		public DbSet<Revision> Revision { get; set; }
	}
}