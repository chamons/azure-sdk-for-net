// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Azure.Analytics.Synapse.Spark;
using Azure.Analytics.Synapse.Spark.Models;
using Azure.Analytics.Synapse.Tests;
using Azure.Core.TestFramework;
using NUnit.Framework;

namespace Azure.Analytics.Synapse.Spark.Tests
{
    /// <summary>
    /// The suite of tests for the <see cref="SparkBatchClient"/> class.
    /// </summary>
    /// <remarks>
    /// These tests have a dependency on live Azure services and may incur costs for the associated
    /// Azure subscription.
    /// </remarks>
    public class SparkBatchClientLiveTests : RecordedTestBase<SynapseTestEnvironment>
    {
        internal class DisposableSparkBatchOperation : IAsyncDisposable
        {
            private readonly SparkBatchClient _client;
            public SparkBatchJob Resource;

            private DisposableSparkBatchOperation (SparkBatchClient client, SparkBatchJob resource)
            {
                _client = client;
                Resource = resource;
            }

            public int Id => Resource.Id;

            public static async ValueTask<DisposableSparkBatchOperation> Create (SparkBatchClient client, TestRecording recording, SynapseTestEnvironment testEnvironment) =>
                new DisposableSparkBatchOperation (client, await CreateResource(client, recording, testEnvironment));

            public static async ValueTask<SparkBatchJob> CreateResource (SparkBatchClient client, TestRecording recording, SynapseTestEnvironment testEnvironment)
            {
                SparkBatchJobOptions options = CreateSparkJobRequestParameters (recording, testEnvironment);
                SparkBatchOperation createOperation = await client.StartCreateSparkBatchJobAsync (options);
                return await createOperation.WaitForCompletionAsync();
            }

            private static SparkBatchJobOptions CreateSparkJobRequestParameters(TestRecording recording, SynapseTestEnvironment testEnvironment)
            {
                string name = recording.GenerateId("dontnetbatch", 16);
                string file = string.Format("abfss://{0}@{1}.dfs.core.windows.net/samples/java/wordcount/wordcount.jar", testEnvironment.StorageFileSystemName, testEnvironment.StorageAccountName);
                return new SparkBatchJobOptions(name, file)
                {
                    ClassName = "WordCount",
                    Arguments =
                    {
                        string.Format("abfss://{0}@{1}.dfs.core.windows.net/samples/java/wordcount/shakespeare.txt", testEnvironment.StorageFileSystemName, testEnvironment.StorageAccountName),
                        string.Format("abfss://{0}@{1}.dfs.core.windows.net/samples/java/wordcount/result/", testEnvironment.StorageFileSystemName, testEnvironment.StorageAccountName),
                    },
                    DriverMemory = "28g",
                    DriverCores = 4,
                    ExecutorMemory = "28g",
                    ExecutorCores = 4,
                    ExecutorCount = 2
                };
            }

            public async ValueTask DisposeAsync()
            {
                await _client.CancelSparkBatchJobAsync (Id);
            }
        }

        public SparkBatchClientLiveTests(bool isAsync) : base(isAsync)
        {
        }

        private SparkBatchClient CreateClient()
        {
            return InstrumentClient(new SparkBatchClient(
                new Uri(TestEnvironment.EndpointUrl),
                TestEnvironment.SparkPoolName,
                TestEnvironment.Credential,
                InstrumentClientOptions(new SparkClientOptions())
            ));
        }

//   await using DisposableSparkBatchOperation batchOperation = await DisposableSparkBatchOperation.Create (client, Recording, TestEnvironment);
        [Test]
        public async Task Foo()
        {
            SparkBatchClient client = CreateClient();
            SparkBatchJobOptions options = CreateSparkJobRequestParameters (Recording, TestEnvironment);
            SparkBatchOperation createOperation = await client.StartCreateSparkBatchJobAsync (options);
            SparkBatchJob job = await createOperation.WaitForCompletionAsync();
            await client.CancelSparkBatchJobAsync (job.Id);
        }

        private static SparkBatchJobOptions CreateSparkJobRequestParameters(TestRecording recording, SynapseTestEnvironment testEnvironment)
        {
            string name = recording.GenerateId("dontnetbatch", 16);
            string file = string.Format("abfss://{0}@{1}.dfs.core.windows.net/samples/java/wordcount/wordcount.jar", testEnvironment.StorageFileSystemName, testEnvironment.StorageAccountName);
            return new SparkBatchJobOptions(name, file)
            {
                ClassName = "WordCount",
                Arguments =
                {
                    string.Format("abfss://{0}@{1}.dfs.core.windows.net/samples/java/wordcount/shakespeare.txt", testEnvironment.StorageFileSystemName, testEnvironment.StorageAccountName),
                    string.Format("abfss://{0}@{1}.dfs.core.windows.net/samples/java/wordcount/result/", testEnvironment.StorageFileSystemName, testEnvironment.StorageAccountName),
                },
                DriverMemory = "28g",
                DriverCores = 4,
                ExecutorMemory = "28g",
                ExecutorCores = 4,
                ExecutorCount = 2
            };
        }

        [Test]
        [Ignore("https://github.com/Azure/azure-sdk-for-net/issues/18080 - This test case cannot pass due to backend limitations for service principals.")]
        public async Task TestSparkBatchJob()
        {
            SparkBatchClient client = CreateClient();

            // Submit the Spark job
            SparkBatchJobOptions createParams = SparkTestUtilities.CreateSparkJobRequestParameters(Recording, TestEnvironment);
            SparkBatchOperation createOperation = await client.StartCreateSparkBatchJobAsync(createParams);
            SparkBatchJob jobCreateResponse = await createOperation.WaitForCompletionAsync();

            // Verify the Spark batch job completes successfully
            Assert.True("success".Equals(jobCreateResponse.State, StringComparison.OrdinalIgnoreCase) && jobCreateResponse.Result == SparkBatchJobResultType.Succeeded,
                string.Format(
                    "Job: {0} did not return success. Current job state: {1}. Actual result: {2}. Error (if any): {3}",
                    jobCreateResponse.Id,
                    jobCreateResponse.State,
                    jobCreateResponse.Result,
                    string.Join(", ", jobCreateResponse.Errors ?? new List<SparkServiceError>())
                )
            );

            // Get the list of Spark batch jobs and check that the submitted job exists
            List<SparkBatchJob> listJobResponse = await SparkTestUtilities.ListSparkBatchJobsAsync(client);
            Assert.NotNull(listJobResponse);
            Assert.IsTrue(listJobResponse.Any(job => job.Id == jobCreateResponse.Id));
        }

        [Test]
        public async Task TestGetSparkBatchJob()
        {
            SparkBatchClient client = CreateClient();

            SparkBatchJobCollection sparkJobs = (await client.GetSparkBatchJobsAsync()).Value;
            foreach (SparkBatchJob expectedSparkJob in sparkJobs.Sessions)
            {
                try
                {
                    SparkBatchJob actualSparkJob = await client.GetSparkBatchJobAsync(expectedSparkJob.Id);
                    ValidateSparkBatchJob(expectedSparkJob, actualSparkJob);
                }
                catch (Azure.RequestFailedException)
                {
                }
            }
        }

       internal void ValidateSparkBatchJob(SparkBatchJob expectedSparkJob, SparkBatchJob actualSparkJob)
       {
            Assert.AreEqual(expectedSparkJob.Name, actualSparkJob.Name);
            Assert.AreEqual(expectedSparkJob.Id, actualSparkJob.Id);
            Assert.AreEqual(expectedSparkJob.AppId, actualSparkJob.AppId);
            Assert.AreEqual(expectedSparkJob.SubmitterId, actualSparkJob.SubmitterId);
            Assert.AreEqual(expectedSparkJob.ArtifactId, actualSparkJob.ArtifactId);
        }
        /*
public virtual System.Threading.Tasks.Task<Azure.Response<Azure.Analytics.Synapse.Spark.Models.SparkBatchJob>> GetSparkBatchJobAsync(int batchId, bool? detailed = default(bool?), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
public virtual System.Threading.Tasks.Task<Azure.Response<Azure.Analytics.Synapse.Spark.Models.SparkBatchJobCollection>> GetSparkBatchJobsAsync(int? from = default(int?), int? size = default(int?), bool? detailed = default(bool?), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        */
    }
}
