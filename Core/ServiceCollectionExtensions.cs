using Microsoft.Extensions.DependencyInjection;
using notes.Core.Interfaces;
using notes.Core.Internal;
using notes.Core.Services;
using notes.FluentValidation;
using notes.Provider;

namespace notes.Core
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddNoteServices(this IServiceCollection services, DatabaseProvider provider)
		{
			services.AddTransient<INoteService, NoteService>();
			services.AddTransient<IUserService, UserService>();
			services.AddTransient<IRevisionService, RevisionService>();
			services.AddSearchServices(provider);
			services.AddTransient<IMailService, MailService>();
			services.AddTransient<ITokenService, TokenService>();
			services.AddSingleton<IPasswordPolicy>(new PasswordPolicy { MinimumNonAlphaCharacters = 0, MinimumUpperCaseCharacters = 0 });

			return services;
		}

		private static IServiceCollection AddSearchServices(this IServiceCollection services, DatabaseProvider provider)
		{
			switch (provider)
			{
				case DatabaseProvider.MySql:
					services.AddTransient<ISearchService, Core.Services.MySql.SearchService>();
					break;
				case DatabaseProvider.PgSql:
					services.AddTransient<ISearchService, Core.Services.PgSql.SearchService>();
					break;
			}

			return services;
		}
	}
}