using SFA.DAS.Functions.Importer.MockAPIs;

MockApiBuilder.Create(5001).StartEndPoints().Build();
MockApiBuilder.Create(5006).StartEndPoints().Build();
MockApiBuilder.Create(5008).StartEndPoints().Build();

Console.WriteLine("Press any key to stop the servers");
Console.ReadKey();
