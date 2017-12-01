﻿#region usings

using System;
using MediaService_BLL_Job.DependencyInjection;
using Microsoft.Azure.WebJobs;

#endregion

namespace MediaService_BLL_Job
{
    internal class Program
    {
        private static void Main()
        {
            var container = SimpleInjectorInitializer.Initialize();

            var config = new JobHostConfiguration
            {
                JobActivator = new JobActivator(container)
            };

            config.Queues.BatchSize = 8;
            config.Queues.MaxDequeueCount = 2;
            config.Queues.MaxPollingInterval = TimeSpan.FromSeconds(15);
            config.Queues.VisibilityTimeout = TimeSpan.FromMinutes(2);

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            var host = new JobHost(config);
            host.RunAndBlock();
        }
    }
}