// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Analytics.Synapse.Monitoring;
using Azure.Analytics.Synapse.Monitoring.Models;
using Azure.Core.TestFramework;
using NUnit.Framework;

namespace Azure.Analytics.Synapse.Tests
{
    public class MonitoringClientLiveTests : RecordedTestBase<SynapseTestEnvironment>
    {
        public MonitoringClientLiveTests(bool isAsync) : base(isAsync)
        {
        }

        private MonitoringClient CreateClient()
        {
            return InstrumentClient(new MonitoringClient(
                new Uri(TestEnvironment.EndpointUrl),
                TestEnvironment.Credential,
                InstrumentClientOptions(new MonitoringClientOptions())
            ));
        }
    }
}
