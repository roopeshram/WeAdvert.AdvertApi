using AdverApi.Services;
//using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AdverApi.HealthCheck
{
    public class StorageHealthCheck : IHealthCheck
    {
        private readonly IadvertStorageService _storageservice;
        public StorageHealthCheck(IadvertStorageService storageservice)
        {
            _storageservice = storageservice;
        }

        public async ValueTask<IHealthCheckResult> CheckAsync(CancellationToken cancellationToken = default)
        {
            var isok = await _storageservice.CheckHealthAsync();
            return HealthCheckResult.FromStatus(isok ? CheckStatus.Healthy : CheckStatus.Unhealthy, "");
            
        }

        
    }
}
