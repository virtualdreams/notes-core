using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using notes.Areas.Admin.Models;
using notes.Core.Models;
using notes.Helper;
using notes.Models;

namespace notes.Extensions
{
	static public class AutoMapperExtensions
	{
		static public IServiceCollection AddAutoMapper(this IServiceCollection services)
		{
			var _autoMapperConfig = new MapperConfiguration(config =>
			{
				config.AllowNullCollections = false;

				config.CreateMap<Note, NoteModel>()
					.ForMember(d => d.TagsString, map => map.MapFrom(s => String.Join(" ", s.Tags ?? new string[] { })))
					.ForMember(d => d.Age, map => map.Ignore())
					.AfterMap((s, d) =>
					{
						d.Age = s.Created?.ToMinutes().ToWords();
					});

				config.CreateMap<User, UserModel>();

				config.CreateMap<DistinctAndCountResult, DistinctAndCountModel>();
			});

			_autoMapperConfig.AssertConfigurationIsValid();

			services.AddSingleton<IMapper>(am => _autoMapperConfig.CreateMapper());

			return services;
		}
	}
}