using Microsoft.EntityFrameworkCore;
using notes.Core.Models;

namespace notes.Core.Services
{
	public class MySqlContext : DbContext
	{
		public MySqlContext(DbContextOptions options)
			: base(options)
		{ }

		public DbSet<Note> Note { get; set; }

		public DbSet<Tag> Tag { get; set; }

		public DbSet<User> User { get; set; }

		public DbSet<Token> Token { get; set; }

		public DbSet<Revision> Revision { get; set; }
	}
}