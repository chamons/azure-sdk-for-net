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
    public class DatasetClientLiveTest: RecordedTestBase<SynapseTestEnvironment>
    {
        internal class DisposableDataSet : IAsyncDisposable
        {
            private readonly DatasetClient _client;
            public DatasetResource Resource;

            private DisposableDataSet (DatasetClient client, DatasetResource resource)
            {
                _client = client;
                Resource = resource;
            }

            public static async ValueTask<DisposableDataSet> Create (DatasetClient client, TestRecording recording) =>
                new DisposableDataSet (client, await CreateResource(client, recording));

            public static async ValueTask<DatasetResource> CreateResource (DatasetClient client, TestRecording recording)
            {
                DatasetResource resource = new DatasetResource (new Dataset ());
                DatasetCreateOrUpdateDatasetOperation create = await client.StartCreateOrUpdateDatasetAsync (recording.GenerateAssetName("DataSet-"), resource); 
                return (await create.WaitForCompletionAsync()).Value;
            }

            public async ValueTask DisposeAsync()
            {
                await _client.StartDeleteDatasetAsync (Resource.Name);
            }
        }

        public DatasetClientLiveTest(bool isAsync) : base(isAsync)
        {
        }

        private DatasetClient CreateClient()
        {
            return InstrumentClient(new DatasetClient(
                new Uri(TestEnvironment.EndpointUrl),
                TestEnvironment.Credential,
                InstrumentClientOptions(new ArtifactsClientOptions())
            ));
        }

        // [Test]
        // public async Task Foo()
        // {
        // }
        /*
        public virtual System.Threading.Tasks.Task<Azure.Response<Azure.Analytics.Synapse.Artifacts.Models.DatasetResource>> GetDatasetAsync(string datasetName, string ifNoneMatch = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public virtual Azure.AsyncPageable<Azure.Analytics.Synapse.Artifacts.Models.DatasetResource> GetDatasetsByWorkspaceAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public virtual System.Threading.Tasks.Task<Azure.Analytics.Synapse.Artifacts.DatasetCreateOrUpdateDatasetOperation> StartCreateOrUpdateDatasetAsync(string datasetName, Azure.Analytics.Synapse.Artifacts.Models.DatasetResource dataset, string ifMatch = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public virtual System.Threading.Tasks.Task<Azure.Analytics.Synapse.Artifacts.DatasetDeleteDatasetOperation> StartDeleteDatasetAsync(string datasetName, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public virtual System.Threading.Tasks.Task<Azure.Analytics.Synapse.Artifacts.DatasetRenameDatasetOperation> StartRenameDatasetAsync(string datasetName, Azure.Analytics.Synapse.Artifacts.Models.ArtifactRenameRequest request, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        */
    }
}
