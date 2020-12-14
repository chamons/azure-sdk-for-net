# Microsoft.Azure.Synapse

Run `dotnet build /t:GenerateCode` to generate code.

### AutoRest Configuration
> see https://aka.ms/autorest

```yaml
repo: https://github.com/Azure/azure-rest-api-specs/blob/0c7df327b5318cfb32c5ce8b24b89efc2ee929c8
```

``` yaml
public-clients: true
input-file:
    - $(repo)/specification/synapse/data-plane/Microsoft.Synapse/preview/2020-02-01-preview/roleAssignments.json
    - $(repo)/specification/synapse/data-plane/Microsoft.Synapse/preview/2020-02-01-preview/roles.json
```
