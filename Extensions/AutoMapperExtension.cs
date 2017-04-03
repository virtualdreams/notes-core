using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using notes.Core.Models;
using notes.Helper;
using notes.Models;

namespace notes.Extensions
{
    static public class AutoMapperExtensions
	{
		static public IServiceCollection AddAutoMapper(this IServiceCollection services)
		{
			var _autoMapperConfig = new MapperConfiguration(config =>{
				config.CreateMap<Note, NoteModel>()
					.ForMember(d => d.TagsString, map => map.MapFrom(s => String.Join(" ", s.Tags ?? new string[] {})))
					.ForMember(d => d.Age, map => map.MapFrom(s => s.Id.CreationTime.ToLocalTime().ToMinutes().ToWords()));

				config.CreateMap<User, UserModel>()
					.ForMember(d => d.Created, map => map.MapFrom(s => String.Format(new System.Globalization.CultureInfo("en-US"), "{0:MMM dd, yyyy}", s.Id.CreationTime)));
			});

			_autoMapperConfig.AssertConfigurationIsValid();

			services.AddSingleton<IMapper>(am => _autoMapperConfig.CreateMapper());

			return services;
		}
	}
}