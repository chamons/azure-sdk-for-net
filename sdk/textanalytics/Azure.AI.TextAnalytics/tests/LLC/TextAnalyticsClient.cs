// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Azure.Core;
using Azure.AI.TextAnalytics;
using Azure.Identity;

namespace Azure.AI.TextAnalytics.Dynamic
{
    public class TextAnalyticsClient
    {
        private TextAnalyticsRestClient _client;

        public TextAnalyticsClient ()
        {
            _client = Azure.AI.TextAnalytics.TextAnalyticsClient.CreateRestClient (new Uri ("https://hamonstext.cognitiveservices.azure.com/"), new DefaultAzureCredential (), new TextAnalyticsClientOptions ());
        }
    }
}