# das-fat-dataloader

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/das-fat-dataloader?repoName=SkillsFundingAgency%2Fdas-fat-dataloader&branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2244&repoName=SkillsFundingAgency%2Fdas-fat-dataloader&branchName=master)

# Requirements

DotNet Core 3.1 and any supported IDE for DEV running.

Azure Storage Emulator

## About

The data loader is responsible for calling the data load endpoints required for [das-courses-api](https://github.com/SkillsFundingAgency/das-courses-api), [das-coursedelivery-api](https://github.com/SkillsFundingAgency/das-coursedelivery-api) and [das-location-api](https://github.com/SkillsFundingAgency/das-location-api) . It in turns calls each `ops/dataload` endpoint to invoke the dataload process.

## Local running

You must have the Azure Storage emulator running, and in that a table created called `Configuration` in that table add the following:

PartitionKey: LOCAL

RowKey: SFA.DAS.DataLoader.Function_1.0

Data:
```
{
  "Importer": {
    "DataLoaderBaseUrlsAndIdentifierUris":"https://localhost:5001/|https://zzz.onmicrosoft.com/[app_service-ar],https://localhost:5006/|https://zzz.onmicrosoft.com/[app_service-ar],https://localhost:5008/|https://zzz.onmicrosoft.com/[app_service-ar]"
  }
}

```

The configuration item is a comma separated list of baseurl endpoints and the identifier used for managed identity authentication. it is assumed that each endpoint has a `ops/dataload` controller action. The function can be invoked manually via a http endpoint, or it is configured to run at 3am every day.