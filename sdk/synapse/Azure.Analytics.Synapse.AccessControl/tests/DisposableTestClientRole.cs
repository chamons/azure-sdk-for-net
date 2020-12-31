// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using System.Linq;
using Azure.Analytics.Synapse.AccessControl;
using Azure.Analytics.Synapse.AccessControl.Models;
using Azure.Core.TestFramework;

namespace Azure.Analytics.Synapse.Tests
{
    internal class DisposableTestClientRole : IAsyncDisposable
    {
        private readonly AccessControlClient _client;
        public string RoleId;
        public string RoleAssignmentId;
        public string PrincipalId;

        private DisposableTestClientRole (AccessControlClient client, string roleId, string roleAssignmentId, string principalId)
        {
            _client = client;
            RoleId = roleId;
            RoleAssignmentId = roleAssignmentId;
            PrincipalId = principalId;
        }

        public static async ValueTask<DisposableTestClientRole> Create (AccessControlClient client, TestRecording recording)
        {
            string roleID = await GetAdminRoleId (client);
            string principalId = recording.Random.NewGuid().ToString();
            RoleAssignmentDetails assignment = await client.CreateRoleAssignmentAsync(new RoleAssignmentOptions(roleID, principalId));
            return new DisposableTestClientRole (client, roleID, assignment.Id, principalId);
        }

        public async ValueTask DisposeAsync() => await _client.DeleteRoleAssignmentByIdAsync(RoleAssignmentId);

        private static async Task<string> GetAdminRoleId (AccessControlClient client)
        {
            return (await client.GetRoleDefinitionsAsync().ToListAsync()).First (x => x.Name == "Workspace Admin").Id;
        }
    }
}