using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using notes.Areas.Admin.Models;
using notes.Core.Models;
using notes.Helper;
using notes.Models;
using System.Linq;

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
					.ForMember(d => d.Tags, map => map.MapFrom(s => s.Tags.Select(i => i.Name)))
					.ForMember(d => d.TagsString, map => map.MapFrom(s => String.Join(" ", s.Tags.Select(i => i.Name))))
					.ForMember(d => d.Age, map => map.Ignore())
					.ForMember(d => d.Version, map => map.Ignore())
					.AfterMap((s, d) =>
					{
						d.Created = s.Created?.ToLocalTime();
						d.Modified = s.Modified?.ToLocalTime();
						d.Age = s.Created?.ToLocalTime().ToMinutes().ToWords();
					});

				config.CreateMap<User, UserModel>()
					.AfterMap((s, d) =>
					{
						d.Created = s.Created?.ToLocalTime();
					});

				config.CreateMap<DistinctAndCount, DistinctAndCountModel>();
			});

			_autoMapperConfig.AssertConfigurationIsValid();

			services.AddSingleton<IMapper>(am => _autoMapperConfig.CreateMapper());

			return services;
		}
	}
}