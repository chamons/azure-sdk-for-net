// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Analytics.Synapse.ManagedPrivateEndpoints;
using Azure.Analytics.Synapse.ManagedPrivateEndpoints.Models;
using Azure.Core.TestFramework;
using NUnit.Framework;

namespace Azure.Analytics.Synapse.Tests
{
    public class ManagedPrivateEndpointsClientLiveTest: RecordedTestBase<SynapseTestEnvironment>
    {
        public ManagedPrivateEndpointsClientLiveTest(bool isAsync) : base(isAsync)
        {
        }

        private ManagedPrivateEndpointsClient TriggerRunClient()
        {
            return InstrumentClient(new ManagedPrivateEndpointsClient(
                new Uri(TestEnvironment.EndpointUrl),
                TestEnvironment.Credential,
                InstrumentClientOptions(new ManagedPrivateEndpointsClientOptions())
            ));
        }

        // [Test]
        // public async Task Foo()
        // {
        // }
    }
}
