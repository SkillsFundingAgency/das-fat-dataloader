using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Functions.Importer.Domain.Interfaces;

namespace SFA.DAS.Functions.Importer.Endpoints;

public class ImportData(IImportDataService _service, ILogger<ImportData> _logger)
{
    [Function("ImportData")]
    public async Task RunAsync([TimerTrigger("0 0 3 * * *", RunOnStartup = true)] TimerInfo timer)
    {
        _logger.LogInformation("ImportData Timer trigger function executed at: {DateTime}", DateTime.UtcNow);
        await Task.Run(() => _service.Import());
    }
}