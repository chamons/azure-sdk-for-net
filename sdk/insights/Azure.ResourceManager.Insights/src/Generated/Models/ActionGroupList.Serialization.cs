// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace Azure.ResourceManager.Insights.Models
{
    public partial class ActionGroupList
    {
        internal static ActionGroupList DeserializeActionGroupList(JsonElement element)
        {
            Optional<IReadOnlyList<ActionGroupResource>> value = default;
            Optional<string> nextLink = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("value"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<ActionGroupResource> array = new List<ActionGroupResource>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(ActionGroupResource.DeserializeActionGroupResource(item));
                    }
                    value = array;
                    continue;
                }
                if (property.NameEquals("nextLink"))
                {
                    nextLink = property.Value.GetString();
                    continue;
                }
            }
            return new ActionGroupList(Optional.ToList(value), nextLink.Value);
        }
    }
}
