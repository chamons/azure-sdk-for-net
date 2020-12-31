// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Analytics.Synapse.Artifacts;
using Azure.Analytics.Synapse.Artifacts.Models;
using Azure.Core.TestFramework;
using NUnit.Framework;

namespace Azure.Analytics.Synapse.Tests
{
    public class TriggerRunClientLiveTest: RecordedTestBase<SynapseTestEnvironment>
    {
        public TriggerRunClientLiveTest(bool isAsync) : base(isAsync)
        {
        }

        private TriggerRunClient TriggerRunClient()
        {
            return InstrumentClient(new TriggerRunClient(
                new Uri(TestEnvironment.EndpointUrl),
                TestEnvironment.Credential,
                InstrumentClientOptions(new ArtifactsClientOptions())
            ));
        }

        // [Test]
        // public async Task Foo()
        // {
        // }
    }
}
