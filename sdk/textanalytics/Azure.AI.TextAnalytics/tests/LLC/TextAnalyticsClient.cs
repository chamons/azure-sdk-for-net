// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.AI.TextAnalytics;
using Azure.Identity;

namespace Azure.AI.TextAnalytics.Dynamic
{
    public class TextAnalyticsClient
    {
        private TextAnalyticsRestClient _client;
        private const string Endpoint = "https://hamonstext.cognitiveservices.azure.com/";

        public TextAnalyticsClient ()
        {
            _client = Azure.AI.TextAnalytics.TextAnalyticsClient.CreateRestClient (new Uri (Endpoint), new DefaultAzureCredential (), new TextAnalyticsClientOptions ());
        }

        public async Task<Response> Invoke (dynamic req, CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            System.Console.WriteLine ("1");
            var msg = CreateMessage (req);
            System.Console.WriteLine ("2");
            return await _client._pipeline.SendAsync (msg, cancellationToken);
        }

        internal HttpMessage CreateMessage(dynamic req)
        {
            var message = _client._pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Post;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(Endpoint, false);
            uri.AppendRaw("/languages", false);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json, text/json");

            request.Headers.Add("Content-Type", "application/json");
            var content = new Utf8JsonRequestContent();

            // HACK
            if (req is DynamicJson r)
            {
                r.WriteTo (content.JsonWriter);
            }

            request.Content = content;

            return message;
        }
    }
}