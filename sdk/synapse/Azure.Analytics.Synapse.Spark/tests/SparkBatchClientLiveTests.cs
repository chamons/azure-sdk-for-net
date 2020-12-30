// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Analytics.Synapse.Spark;
using Azure.Analytics.Synapse.Spark.Models;
using Azure.Core.TestFramework;
using NUnit.Framework;

namespace Azure.Analytics.Synapse.Tests
{
    public class SparkBatchClientLiveTests: RecordedTestBase<SynapseTestEnvironment>
    {
        public SparkBatchClientLiveTests(bool isAsync) : base(isAsync)
        {
        }

        private SparkBatchClient TriggerRunClient()
        {
            return InstrumentClient(new SparkBatchClient(
                new Uri(TestEnvironment.EndpointUrl),
                TestEnvironment.SparkPoolName,
                TestEnvironment.Credential,
                InstrumentClientOptions(new SparkClientOptions())
            ));
        }

        // [Test]
        // public async Task Foo()
        // {
        // }
    }
}
