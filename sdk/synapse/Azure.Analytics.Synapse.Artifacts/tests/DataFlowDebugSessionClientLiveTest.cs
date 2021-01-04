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
    public class DataFlowDebugSessionClientLiveTest: RecordedTestBase<SynapseTestEnvironment>
    {
        internal class DisposableDataFlowDebugSession : IAsyncDisposable
        {
            private readonly DataFlowDebugSessionClient _client;
            public string SessionId;

            private DisposableDataFlowDebugSession (DataFlowDebugSessionClient client, string sessionId)
            {
                _client = client;
                SessionId = sessionId;
            }

            public static async ValueTask<DisposableDataFlowDebugSession> Create (DataFlowDebugSessionClient client, TestRecording recording) =>
                new DisposableDataFlowDebugSession (client, await CreateResource(client, recording));

            public static async ValueTask<string> CreateResource (DataFlowDebugSessionClient client, TestRecording recording)
            {
                // SYNAPSE_API_ISSUE - When do we need to pass DataFlowName?
                DataFlowDebugSessionCreateDataFlowDebugSessionOperation create = await client.StartCreateDataFlowDebugSessionAsync (new CreateDataFlowDebugSessionRequest ());
                // SYNAPSE_API_ISSUE - Why is there is no wrapper to save here?
                return (await create.WaitForCompletionAsync()).Value.SessionId;
            }

            public async ValueTask DisposeAsync()
            {
                // SYNAPSE_API_ISSUE - Why does the ctor not take these? When do we need to pass DataFlowName?
                DeleteDataFlowDebugSessionRequest deleteRequest = new DeleteDataFlowDebugSessionRequest ()  { SessionId = SessionId };
                await _client.DeleteDataFlowDebugSessionAsync (deleteRequest);
            }
        }

        public DataFlowDebugSessionClientLiveTest(bool isAsync) : base(isAsync)
        {
        }

        private DataFlowClient CreateFlowClient()
        {
            return InstrumentClient(new DataFlowClient(
                new Uri(TestEnvironment.EndpointUrl),
                TestEnvironment.Credential,
                InstrumentClientOptions(new ArtifactsClientOptions())
            ));
        }

        private DataFlowDebugSessionClient CreateDebugClient()
        {
            return InstrumentClient(new DataFlowDebugSessionClient(
                new Uri(TestEnvironment.EndpointUrl),
                TestEnvironment.Credential,
                InstrumentClientOptions(new ArtifactsClientOptions())
            ));
        }

        [Test]
        public async Task AddDataFlow()
        {
            DataFlowClient flowClient = CreateFlowClient();
            DataFlowDebugSessionClient debugClient = CreateDebugClient();

            await using DisposableDataFlow flow = await DisposableDataFlow.Create (flowClient, this.Recording);
            await using DisposableDataFlowDebugSession debugSession = await DisposableDataFlowDebugSession.Create (debugClient, this.Recording);

            // SYNAPSE_API_ISSUE - Why do we need to pass in SessionId here?
            DataFlowDebugPackage debugPackage = new DataFlowDebugPackage () { DataFlow = new DataFlowDebugResource (flow.Resource.Properties), SessionId = debugSession.SessionId };
            AddDataFlowToDebugSessionResponse response = await debugClient.AddDataFlowAsync (debugPackage);
            Assert.NotNull (response.JobVersion);
        }

        /*
        public virtual Azure.AsyncPageable<Azure.Analytics.Synapse.Artifacts.Models.DataFlowDebugSessionInfo> QueryDataFlowDebugSessionsByWorkspaceAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public virtual System.Threading.Tasks.Task<Azure.Analytics.Synapse.Artifacts.DataFlowDebugSessionExecuteCommandOperation> StartExecuteCommandAsync(Azure.Analytics.Synapse.Artifacts.Models.DataFlowDebugCommandRequest request, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        */
    }
}
