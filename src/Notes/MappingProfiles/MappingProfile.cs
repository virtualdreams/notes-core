using AutoMapper;
using Notes.Areas.Admin.Models;
using Notes.Core.Models;
using Notes.Models;
using System.Linq;
using System;

namespace Notes.MappingProfiles
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			#region domain -> view
			CreateMap<Note, NoteModel>()
				.ForMember(d => d.Tags, map => map.MapFrom(s => s.Tags.Select(i => i.Name)))
				.ForMember(d => d.TagsString, map => map.MapFrom(s => String.Join(" ", s.Tags.Select(i => i.Name))))
				.ForMember(d => d.Created, map => map.MapFrom(s => s.Created.ToLocalTime()))
				.ForMember(d => d.Modified, map => map.MapFrom(s => s.Modified.ToLocalTime()));

			CreateMap<User, UserModel>()
				.ForMember(d => d.Created, map => map.MapFrom(s => s.Created.ToLocalTime()))
				.ForMember(d => d.Modified, map => map.MapFrom(s => s.Modified.ToLocalTime()));

			CreateMap<Revision, RevisionModel>()
				.ForMember(d => d.Created, map => map.MapFrom(s => s.Created.ToLocalTime()))
				.ForMember(d => d.Modified, map => map.MapFrom(s => s.Modified.ToLocalTime()));

			CreateMap<DistinctAndCount, DistinctAndCountModel>();
			#endregion
		}
	}
}