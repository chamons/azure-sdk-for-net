// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

using Azure.Core.TestFramework;
using Azure.Learn.AppConfig.Tests;
using NUnit.Framework;
using Azure.Core;
using Azure.Learn.AppConfig;
using Azure.Identity;

namespace Azure.Learn.AppConfig.Samples
{
    public class Sample1_HelloWorld : SamplesBase<LearnAppConfigTestEnvironment>
    {
        [Test]
        public void GetConfigurationSetting()
        {
            string endpoint = "http://example.azconfig.io";
            ConfigurationClient client = new ConfigurationClient(new Uri(endpoint), new DefaultAzureCredential());

        }
    }
}
