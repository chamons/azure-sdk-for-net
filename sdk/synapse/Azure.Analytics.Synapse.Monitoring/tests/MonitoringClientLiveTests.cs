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
        /// <summary>
        /// Initializes a new instance of the <see cref="MonitoringClientLiveTests"/> class.
        /// </summary>
        /// <param name="isAsync">A flag used by the Azure Core Test Framework to differentiate between tests for asynchronous and synchronous methods.</param>
        public MonitoringClientLiveTests(bool isAsync) : base(isAsync)
        {
        }

        [Test]
        public async Task TestListSparkApplications()
        {
            SparkJobListViewResponse sparkJobList = await MonitoringClient.GetSparkJobListAsync();
            Assert.NotNull(sparkJobList);
            CollectionAssert.IsNotEmpty(sparkJobList.SparkJobs);
        }

        [Test]
        public async Task TestSqlQuery()
        {
            SqlQueryStringDataModel sqlQuery = await MonitoringClient.GetSqlJobQueryStringAsync();
            Assert.NotNull(sqlQuery);
            Assert.IsNotNull(sqlQuery.Query);
        }
    }
}
