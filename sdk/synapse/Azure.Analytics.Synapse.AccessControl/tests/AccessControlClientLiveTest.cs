// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Analytics.Synapse.AccessControl;
using Azure.Analytics.Synapse.AccessControl.Models;
using Azure.Core.TestFramework;
using NUnit.Framework;

namespace Azure.Analytics.Synapse.Tests
{
    public class AccessControlClientLiveTest: RecordedTestBase<SynapseTestEnvironment>
    {
        public AccessControlClientLiveTest(bool isAsync) : base(isAsync)
        {
        }

        private AccessControlClient CreateClient()
        {
            return InstrumentClient(new AccessControlClient(
                new Uri(TestEnvironment.EndpointUrl),
                TestEnvironment.Credential,
                InstrumentClientOptions(new AccessControlClientOptions())
            ));
        }

        [Test]
        public async Task CreateRoleAssignment()
        {
            AccessControlClient client = CreateClient();
            await using DisposableTestClientRole role = await DisposableTestClientRole.Create (client, this.Recording);

            Assert.NotNull(role.RoleId);
            Assert.NotNull(role.RoleAssignmentId);
            Assert.NotNull(role.PrincipalId);
        }

        [Test]
        public async Task GetRoleAssignment()
        {
            AccessControlClient client = CreateClient();
            await using DisposableTestClientRole role = await DisposableTestClientRole.Create (client, this.Recording);

            RoleAssignmentDetails roleAssignment = await client.GetRoleAssignmentByIdAsync(role.RoleAssignmentId);

            Assert.AreEqual(role.RoleId, roleAssignment.RoleId);
            Assert.AreEqual(role.PrincipalId, roleAssignment.PrincipalId);
        }

        [Test]
        public async Task ListRoleAssignments()
        {
            AccessControlClient client = CreateClient();
            await using DisposableTestClientRole role = await DisposableTestClientRole.Create (client, this.Recording);

            Response<IReadOnlyList<RoleAssignmentDetails>> roleAssignments = await client.GetRoleAssignmentsAsync();

            Assert.GreaterOrEqual(roleAssignments.Value.Count, 1);
        }

        [Test]
        public async Task DeleteRoleAssignments()
        {
            AccessControlClient client = CreateClient();

            await using DisposableTestClientRole role = await DisposableTestClientRole.Create (client, this.Recording);

            Response response = await client.DeleteRoleAssignmentByIdAsync (role.RoleAssignmentId);
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
        public async Task GetCallerRoleAssignment()
        {
            AccessControlClient client = CreateClient();
            Response<IReadOnlyList<string>> assignments = await client.GetCallerRoleAssignmentsAsync ();
            Assert.GreaterOrEqual(assignments.Value.Count, 1);
        }
    }
}
