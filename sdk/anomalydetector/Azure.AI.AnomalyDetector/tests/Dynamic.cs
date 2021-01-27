// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.AnomalyDetector.Tests.Infrastructure;
using Azure.Core;
using NUnit.Framework;

namespace Azure.AI.AnomalyDetector.Tests
{
    public class Dynamic
    {
        [Test]
        public async Task GetResultForEntireDetect()
        {
            var client = new AnomalyDetectorLowLevelClient();

            var data = TestData.TestPointSeries;
            data.MaxAnomalyRatio = 0.25F;
            data.Sensitivity = 95;

            dynamic request = DynamicJson.Object();
            var result = await client.Invoke(request);

            Assert.AreEqual(TestData.ExpectedEntireDetectResult.ExpectedValues, result.Value.ExpectedValues);
            Assert.AreEqual(TestData.ExpectedEntireDetectResult.UpperMargins, result.Value.UpperMargins);
            Assert.AreEqual(TestData.ExpectedEntireDetectResult.LowerMargins, result.Value.LowerMargins);
            Assert.AreEqual(TestData.ExpectedEntireDetectResult.IsAnomaly, result.Value.IsAnomaly);
            Assert.AreEqual(TestData.ExpectedEntireDetectResult.IsPositiveAnomaly, result.Value.IsPositiveAnomaly);
            Assert.AreEqual(TestData.ExpectedEntireDetectResult.IsNegativeAnomaly, result.Value.IsNegativeAnomaly);
        }
    }
}
