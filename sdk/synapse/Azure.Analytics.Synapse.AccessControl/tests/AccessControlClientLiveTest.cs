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

        // [Test]
        // public async Task CreateRoleAssignment()
        // {
            // AccessControlClient client = CreateClient();
            // string roleID = await GetRole (client);
            // string principalId = GenerateGuid ();

            // var assignment = await client.CreateRoleAssignmentAsync(new RoleAssignmentOptions(roleID, principalId));

            // Assert.AreEqual(roleID, assignment.Value.RoleId);
            // Assert.AreEqual(principalId, assignment.Value.PrincipalId);
        // }

        [Test]
        public async Task GetRoleAssignment()
        {
            AccessControlClient client = CreateClient();
            await using DisposableTestClientRole role = await DisposableTestClientRole.Create (client, this.Recording);

            RoleAssignmentDetails roleAssignment = await client.GetRoleAssignmentByIdAsync(role.PrincipalId);

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
            var role = await GetRole (client);
            Response<IReadOnlyList<RoleAssignmentDetails>> roleAssignments = await client.GetRoleAssignmentsAsync();
            foreach (var r in roleAssignments.Value)
            {
                if (role == r.RoleId) {
                    Console.WriteLine ($"Deleting {role} from princ {r.PrincipalId} and assignment {r.Id}");
                    await client.DeleteRoleAssignmentByIdAsync (role);
                }
            }
        }

        private static async Task<string> GetRole (AccessControlClient client)
        {
            AsyncPageable<SynapseRole> roles = client.GetRoleDefinitionsAsync();
            await foreach (SynapseRole role in roles)
            {
                if (role.Name == "Workspace Admin")
                {
                    return role.Id;
                }
            }
            throw new InvalidOperationException ("Unable to find 'Workspace Admin' role in workspace");
        }
    }
}