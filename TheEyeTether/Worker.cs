using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TheEyeTether.Data;
using TheEyeTether.Providers;
using TheEyeTether.Types;
using TheEyeTether.Utilities.General;
using TheEyeTether.Utilities.Hypotheses;

namespace TheEyeTether
{
	public class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;

		public Worker(ILogger<Worker> logger)
		{
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var assemblyProvider = new AssemblyProvider();
			var categorySettingsText = Resources.ReadTextResource("CategorySettings.json",
					assemblyProvider);
			var categorySettings = JsonConvert.DeserializeObject<Dictionary<string, CategorySetting>>(
					categorySettingsText); 
			var dataPointSettingsText = Resources.ReadTextResource("DataPointSettings.json",
					assemblyProvider);
			var dataPointSettings = JsonConvert.DeserializeObject<Dictionary<string, DataPointSetting>>(
					dataPointSettingsText);
			var fileSystem = new FileSystem();
			var lua = new Lua(fileSystem);
			var drivesProvider = new DrivesProvider();
			var osPlatformChecker = new OSPlatformChecker();
			var currentDomainBaseDirectoryProvider = new CurrentDomainBaseDirectoryProvider();
			var clock = new Clock();

			while (!stoppingToken.IsCancellationRequested)
			{
				_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
				
				PendingDataConverter.Convert(categorySettings, dataPointSettings, fileSystem, lua,
						drivesProvider, osPlatformChecker, currentDomainBaseDirectoryProvider,
						clock);
				var hypotheses = HypothesesCreator.Create(fileSystem, clock,
						currentDomainBaseDirectoryProvider);
				HypothesesSaver.Save(hypotheses, fileSystem, currentDomainBaseDirectoryProvider);

				await Task.Delay(30000, stoppingToken);
			}
		}
	}
}
