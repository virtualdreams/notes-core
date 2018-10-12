using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using notes.Core.Services;
using notes.Models;
using notes.Filters;

namespace notes.Controllers
{
	[Install]
	public class InstallController : Controller
	{
		private readonly IMapper Mapper;
		private readonly Settings Options;
		private readonly UserService UserService;

		public InstallController(IMapper mapper, Settings settings, UserService user)
		{
			Mapper = mapper;
			Options = settings;
			UserService = user;
		}

		public IActionResult Index()
		{
			return Content("Installer...");
		}
	}
}