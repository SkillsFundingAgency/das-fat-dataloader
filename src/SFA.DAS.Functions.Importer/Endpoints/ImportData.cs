using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Functions.Importer.Domain.Interfaces;

namespace SFA.DAS.Functions.Importer.Endpoints
{
    public class ImportData
    {
        private readonly IImportDataService _service;

        public ImportData (IImportDataService service)
        {
            _service = service;
        }
        
        [FunctionName("ImportData")]
        public async Task RunAsync([TimerTrigger("0 0 3 * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"ImportData Timer trigger function executed at: {DateTime.UtcNow}");
            await Task.Run(() => _service.Import());
        }
    }
}