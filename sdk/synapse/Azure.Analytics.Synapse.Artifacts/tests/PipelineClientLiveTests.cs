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
    /// The suite of tests for the <see cref="PipelineClient"/> class.
    /// </summary>
    /// <remarks>
    /// These tests have a dependency on live Azure services and may incur costs for the associated
    /// Azure subscription.
    /// </remarks>
    public class PipelineClientLiveTests : RecordedTestBase<SynapseTestEnvironment>
    {
        internal class DisposablePipeline : IAsyncDisposable
        {
            private readonly PipelineClient _client;
            public PipelineResource Resource;

            private DisposablePipeline (PipelineClient client, PipelineResource resource)
            {
                _client = client;
                Resource = resource;
            }

            public string Name => Resource.Name;

            public static async ValueTask<DisposablePipeline> Create (PipelineClient client, TestRecording recording) =>
                new DisposablePipeline (client, await CreateResource(client, recording));

            public static async ValueTask<PipelineResource> CreateResource (PipelineClient client, TestRecording recording)
            {
                string pipelineName = recording.GenerateName("Pipeline");
                PipelineCreateOrUpdatePipelineOperation createOperation = await client.StartCreateOrUpdatePipelineAsync(pipelineName, new PipelineResource());
                return await createOperation.WaitForCompletionAsync();
            }

            public async ValueTask DisposeAsync()
            {
                PipelineDeletePipelineOperation operation = await _client.StartDeletePipelineAsync (Name);
                await operation.WaitForCompletionAsync ();
            }
        }

        public PipelineClientLiveTests(bool isAsync) : base(isAsync)
        {
        }

        private PipelineClient CreateClient()
        {
            return InstrumentClient(new PipelineClient(
                new Uri(TestEnvironment.EndpointUrl),
                TestEnvironment.Credential,
                InstrumentClientOptions(new ArtifactsClientOptions())
            ));
        }

        [Test]
        public async Task TestGetPipeline()
        {
            PipelineClient client = CreateClient ();
            await using DisposablePipeline pipeline = await DisposablePipeline.Create (client, this.Recording);

            AsyncPageable<PipelineResource> pipelines = client.GetPipelinesByWorkspaceAsync();
            Assert.GreaterOrEqual((await pipelines.ToListAsync()).Count, 1);

            await foreach (var expectedPipeline in pipelines)
            {
                PipelineResource actualPipeline = await client.GetPipelineAsync(expectedPipeline.Name);
                Assert.AreEqual(expectedPipeline.Name, actualPipeline.Name);
                Assert.AreEqual(expectedPipeline.Id, actualPipeline.Id);
            }
        }

        [Test]
        public async Task TestDeleteNotebook()
        {
            PipelineClient client = CreateClient();

            // Non-disposable as we'll be deleting it ourselves
            PipelineResource resource = await DisposablePipeline.CreateResource (client, this.Recording);

            PipelineDeletePipelineOperation operation = await client.StartDeletePipelineAsync (resource.Name);
            Response response = await operation.WaitForCompletionAsync ();
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
        public async Task TestRenameLinkedService()
        {
            PipelineClient client = CreateClient();

            // Non-disposable as we'll rename it underneath (and have to clean up ourselves)
            PipelineResource resource = await DisposablePipeline.CreateResource (client, Recording);

            string newPipelineName = Recording.GenerateName("Pipeline");

            PipelineRenamePipelineOperation renameOperation = await client.StartRenamePipelineAsync (resource.Name, new ArtifactRenameRequest () { NewName = newPipelineName } );
            await renameOperation.WaitForCompletionAsync ();

            PipelineResource pipeline = await client.GetPipelineAsync (newPipelineName);
            Assert.AreEqual (newPipelineName, pipeline.Name);

            PipelineDeletePipelineOperation operation = await client.StartDeletePipelineAsync (newPipelineName);
            await operation.WaitForCompletionAsync ();
        }

        [Test]
        public async Task TestPipelineRun()
        {
            PipelineClient client = CreateClient();

            await using DisposablePipeline pipeline = await DisposablePipeline.Create (client, this.Recording);

            CreateRunResponse runResponse = await client.CreatePipelineRunAsync (pipeline.Name);
            Assert.NotNull(runResponse.RunId);
        }
    }
}
