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
    public class SparkSessionClientLiveTests: RecordedTestBase<SynapseTestEnvironment>
    {
        public SparkSessionClientLiveTests(bool isAsync) : base(isAsync)
        {
        }

        private SparkSessionClient TriggerRunClient()
        {
            return InstrumentClient(new SparkSessionClient(
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
