using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System;
using notes.Areas.Admin.Models;
using notes.Core.Models;
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
					.ForMember(d => d.Tags, map => map.MapFrom(s => s.Tags.Select(i => i.Name)))
					.ForMember(d => d.TagsString, map => map.MapFrom(s => String.Join(" ", s.Tags.Select(i => i.Name))))
					.ForMember(d => d.Created, map => map.MapFrom(s => s.Created.ToLocalTime()))
					.ForMember(d => d.Modified, map => map.MapFrom(s => s.Modified.ToLocalTime()));

				config.CreateMap<User, UserModel>()
					.ForMember(d => d.Created, map => map.MapFrom(s => s.Created.ToLocalTime()));

				config.CreateMap<Revision, RevisionModel>()
					.ForMember(d => d.Created, map => map.MapFrom(s => s.Created.ToLocalTime()))
					.ForMember(d => d.Modified, map => map.MapFrom(s => s.Modified.ToLocalTime()));

				config.CreateMap<DistinctAndCount, DistinctAndCountModel>();
			});

			_autoMapperConfig.AssertConfigurationIsValid();

			services.AddSingleton<IMapper>(am => _autoMapperConfig.CreateMapper());

			return services;
		}
	}
}