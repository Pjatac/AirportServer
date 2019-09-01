using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Airport.BL;
using Airport.Hubs;
using Airport.Infra;
using Airport.Models;
using CommonLibruary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Airport.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ValuesController : ControllerBase
	{
		private readonly IDispatcherService _dispatcherService;
		public ValuesController(IDispatcherService dispatcherService)
		{
			_dispatcherService = dispatcherService;
		}
		[HttpGet]
		[Route("schedule")]
		public ActionResult<IEnumerable<Flight>> GetSchedule()
		{	
			return _dispatcherService.GetSchedule();
		}

		// POST api/values
		[HttpPost]
		public void AddFlight([FromBody] Flight flight)
		{
			try
			{
				_dispatcherService.AddNewPlane(flight);
			}
			catch (Exception ex)
			{
				
			}
		}
	}
}
