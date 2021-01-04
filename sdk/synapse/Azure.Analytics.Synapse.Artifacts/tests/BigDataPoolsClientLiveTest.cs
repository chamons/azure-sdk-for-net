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
    public class BigDataPoolsClientLiveTest: RecordedTestBase<SynapseTestEnvironment>
    {
        public BigDataPoolsClientLiveTest(bool isAsync) : base(isAsync)
        {
        }

        private BigDataPoolsClient CreateClient()
        {
            return InstrumentClient(new BigDataPoolsClient(
                new Uri(TestEnvironment.EndpointUrl),
                TestEnvironment.Credential,
                InstrumentClientOptions(new ArtifactsClientOptions())
            ));
        }

        [Ignore("This test case cannot be automated due to the inability to configure infrastructure to test against.")]
        [Test]
        public async Task ListPools()
        {
            BigDataPoolsClient client = CreateClient();
            BigDataPoolResourceInfoListResult pools = await client.ListAsync ();
            Assert.GreaterOrEqual(1, pools.Value.Count);
        }

        [Ignore("This test case cannot be automated due to the inability to configure infrastructure to test against.")]
        [Test]
        public async Task GetPool()
        {
            const string PoolName = "sparkchhamosyna";
            BigDataPoolsClient client = CreateClient();
            BigDataPoolResourceInfo pool = await client.GetAsync (PoolName);
            Assert.AreEqual(PoolName, pool.Name);
        }
    }
}
