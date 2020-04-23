using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace notes.Extensions
{
	static public class AutoMapperExtensions
	{
		static public IServiceCollection AddAutoMapper(this IServiceCollection services)
		{
			var mappingConfiguration = new MapperConfiguration(config =>
			{
				config.AllowNullCollections = false;

				config.AddMaps(typeof(Startup));
				// config.AddProfile<MappingProfile>();
			});

			mappingConfiguration.AssertConfigurationIsValid();

			services.AddSingleton(mappingConfiguration.CreateMapper());

			return services;
		}
	}
}