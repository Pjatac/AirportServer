using CommonLibruary;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Simulator
{
	class Program
	{
		private static Timer Generate { get; set; }
		private static Random rnd = new Random();
		private static HttpClient httpClient = new HttpClient();
		static int count = Settings.NumberToGenerete;
		static void Main(string[] args)
		{
			TimerCallback timeCB = new TimerCallback(SendNewFlightAsync);
			Generate = new Timer(timeCB, null, Settings.StartSimulationDelay, Settings.GenerateNewFlight);
			Console.WriteLine($"Press any key to stop");
			Console.ReadLine();
		}
		public static void SendNewFlightAsync(object param)
		{
			if (count > 0)
			{
				count--;
				Flight flight = new Flight { Number = Faker.Name.First().Substring(0, 2).ToUpper() + rnd.Next(100, 999), Direction = rnd.Next(1, 3), QueryDate = DateTime.Now + TimeSpan.FromSeconds(rnd.Next(Settings.SecondsStartFlyOperatingDelayFrom, Settings.SecondsStartFlyOperatingDelayTo)) };
				httpClient.PostAsync($"https://localhost:44397/api/values", new StringContent(JsonConvert.SerializeObject(flight), Encoding.UTF8, "application/json"));
			}
		}
	}
}
