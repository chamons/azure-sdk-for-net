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
            string roleID = await GetRole (client);
            string principalId = GenerateGuid (recording);
            RoleAssignmentDetails assignment = await client.CreateRoleAssignmentAsync(new RoleAssignmentOptions(roleID, principalId));
            return new DisposableTestClientRole (client, roleID, assignment.Id, principalId);
        }

        public async ValueTask DisposeAsync() => await _client.DeleteRoleAssignmentByIdAsync(RoleAssignmentId);

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

        private static string GenerateGuid (TestRecording recording)
        {
            var bytes = new byte[16];
            recording.Random.NextBytes (bytes);
            return new Guid (bytes).ToString();
        }
    }
}