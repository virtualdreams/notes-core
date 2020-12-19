using Microsoft.Extensions.DependencyInjection;
using notes.Core.Interfaces;
using notes.Core.Internal;
using notes.Core.Services;
using notes.FluentValidation;

namespace notes.Core
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddNoteServices(this IServiceCollection services)
		{
			services.AddTransient<INoteService, NoteService>();
			services.AddTransient<IUserService, UserService>();
			services.AddTransient<IRevisionService, RevisionService>();
			services.AddTransient<ISearchService, SearchService>();
			services.AddTransient<IMailService, MailService>();
			services.AddTransient<ITokenService, TokenService>();
			services.AddSingleton<IPasswordPolicy>(new PasswordPolicy { MinimumNonAlphaCharacters = 0, MinimumUpperCaseCharacters = 0 });

			return services;
		}
	}
}