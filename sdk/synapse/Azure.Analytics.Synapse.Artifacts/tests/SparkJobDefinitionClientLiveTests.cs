// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Analytics.Synapse.Artifacts;
using Azure.Analytics.Synapse.Artifacts.Models;
using Azure.Analytics.Synapse.Tests;
using Azure.Core.TestFramework;
using NUnit.Framework;

namespace Azure.Analytics.Synapse.Artifacts.Tests
{
    /// <summary>
    /// The suite of tests for the <see cref="SparkJobDefinitionClient"/> class.
    /// </summary>
    /// <remarks>
    /// These tests have a dependency on live Azure services and may incur costs for the associated
    /// Azure subscription.
    /// </remarks>
    // [Ignore("This test case cannot be automated due to the inability to configure infrastructure to test against.")]
    public class SparkJobDefinitionClientLiveTests : RecordedTestBase<SynapseTestEnvironment>
    {
        internal class DisposableSparkJobDefinition : IAsyncDisposable
        {
            private readonly SparkJobDefinitionClient _client;
            public SparkJobDefinitionResource Resource;

            private DisposableSparkJobDefinition (SparkJobDefinitionClient client, SparkJobDefinitionResource resource)
            {
                _client = client;
                Resource = resource;
            }

            public string Name => Resource.Name;

            public static async ValueTask<DisposableSparkJobDefinition> Create (SparkJobDefinitionClient client, TestRecording recording, string storageFileSystemName, string storageAccountName) =>
                new DisposableSparkJobDefinition (client, await CreateResource(client, recording, storageFileSystemName, storageAccountName));

            public static async ValueTask<SparkJobDefinitionResource> CreateResource (SparkJobDefinitionClient client, TestRecording recording, string storageFileSystemName, string storageAccountName)
            {
                string jobName = recording.GenerateName("SparkJobDefinition");

                string file = string.Format("abfss://{0}@{1}.dfs.core.windows.net/samples/net/wordcount/wordcount.zip", storageFileSystemName, storageAccountName);
                SparkJobProperties jobProperties = new SparkJobProperties (file, "28g", 4, "28g", 4, 2);
                SparkJobDefinition jobDefinition = new SparkJobDefinition (new BigDataPoolReference (BigDataPoolReferenceType.BigDataPoolReference, "sparkchhamosyna"), jobProperties);
                SparkJobDefinitionResource resource = new SparkJobDefinitionResource (jobDefinition);
                return await client.CreateOrUpdateSparkJobDefinitionAsync(jobName, resource);
            }

            public async ValueTask DisposeAsync()
            {
                await _client.DeleteSparkJobDefinitionAsync (Name);
            }
        }

        public SparkJobDefinitionClientLiveTests(bool isAsync) : base(isAsync)
        {
        }

        private SparkJobDefinitionClient CreateClient()
        {
            return InstrumentClient(new SparkJobDefinitionClient(
                new Uri(TestEnvironment.EndpointUrl),
                TestEnvironment.Credential,
                InstrumentClientOptions(new ArtifactsClientOptions())
            ));
        }

        [Test]
        public async Task TestGetSparkJob()
        {
            SparkJobDefinitionClient client = CreateClient ();
            await using DisposableSparkJobDefinition sparkJobDefinition = await DisposableSparkJobDefinition.Create (client, Recording, TestEnvironment.StorageFileSystemName, TestEnvironment.StorageAccountName);

            AsyncPageable<SparkJobDefinitionResource> jobs = client.GetSparkJobDefinitionsByWorkspaceAsync();
            Assert.GreaterOrEqual((await jobs.ToListAsync()).Count, 1);

            await foreach (var expectedJob in jobs)
            {
                SparkJobDefinitionResource actualJob = await client.GetSparkJobDefinitionAsync(expectedJob.Name);
                Assert.AreEqual(expectedJob.Name, actualJob.Name);
                Assert.AreEqual(expectedJob.Id, actualJob.Id);
            }
        }

        [Test]
        public async Task TestDeleteSparkJob()
        {
            SparkJobDefinitionClient client = CreateClient();

            // Non-disposable as we'll be deleting it ourselves
            SparkJobDefinitionResource resource = await DisposableSparkJobDefinition.CreateResource (client, Recording, TestEnvironment.StorageFileSystemName, TestEnvironment.StorageAccountName);

            Response response = await client.DeleteSparkJobDefinitionAsync (resource.Name);
            switch (response.Status) {
                case 200:
                case 204:
                    break;
                default:
                    Assert.Fail($"Unexpected status ${response.Status} returned");
                    break;
            }
        }

        [Test]
        public async Task TestRenameSparkJob()
        {
            SparkJobDefinitionClient client = CreateClient();

            // Non-disposable as we'll rename it underneath (and have to clean up ourselves)
            SparkJobDefinitionResource resource = await DisposableSparkJobDefinition.CreateResource (client, this.Recording, TestEnvironment.StorageFileSystemName, TestEnvironment.StorageAccountName);

            string newSparkJobName = Recording.GenerateName("Pipeline2");

            SparkJobDefinitionRenameSparkJobDefinitionOperation renameOperation = await client.StartRenameSparkJobDefinitionAsync (resource.Name, new ArtifactRenameRequest () { NewName = newSparkJobName } );
            await renameOperation.WaitForCompletionAsync ();

            SparkJobDefinitionResource sparkJob = await client.GetSparkJobDefinitionAsync (newSparkJobName);
            Assert.AreEqual (newSparkJobName, sparkJob.Name);

            client.DeleteSparkJobDefinition (newSparkJobName);
        }

        /*
        public virtual System.Threading.Tasks.Task<Azure.Analytics.Synapse.Artifacts.SparkJobDefinitionDebugSparkJobDefinitionOperation> StartDebugSparkJobDefinitionAsync(Azure.Analytics.Synapse.Artifacts.Models.SparkJobDefinitionResource sparkJobDefinitionAzureResource, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public virtual System.Threading.Tasks.Task<Azure.Analytics.Synapse.Artifacts.SparkJobDefinitionExecuteSparkJobDefinitionOperation> StartExecuteSparkJobDefinitionAsync(string sparkJobDefinitionName, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        */
    }
}
