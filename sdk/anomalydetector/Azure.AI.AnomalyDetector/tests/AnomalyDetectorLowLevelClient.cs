// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.AI.AnomalyDetector.Models;
using Azure.Core;
using Azure.Identity;

namespace Azure.AI.AnomalyDetector.Tests
{
    public class AnomalyDetectorLowLevelClient
    {
        private AnomalyDetectorRestClient _client;
        private const string Endpoint = "https://chhamoanomalydetector-anomalydetector.cognitiveservices.azure.com/";

        public AnomalyDetectorLowLevelClient()
        {
            _client = AnomalyDetectorClient.CreateRestClient(new Uri(Endpoint), new DefaultAzureCredential(), new AnomalyDetectorClientOptions());
        }

        public async Task<Response> Invoke(dynamic req, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpMessage message = CreateDetectEntireSeriesRequest(req);
            await _client._pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            return message.Response;
        }

        internal HttpMessage CreateDetectEntireSeriesRequest(dynamic req)
        {
            var message = _client._pipeline.CreateMessage();
            var request = message.Request;
            request.Method = RequestMethod.Post;
            var uri = new RawRequestUriBuilder();
            uri.AppendRaw(Endpoint, false);
            uri.AppendRaw("/anomalydetector/v1.0", false);
            uri.AppendPath("/timeseries/entire/detect", false);
            request.Uri = uri;
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Content-Type", "application/json");
            var content = new Utf8JsonRequestContent();

            if (req is DynamicJson r)
            {
                r.WriteTo(content.JsonWriter);
            }

            request.Content = content;
            return message;
        }
    }
}
