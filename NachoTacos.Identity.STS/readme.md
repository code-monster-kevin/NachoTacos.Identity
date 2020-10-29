# NachoTacos.Identity.STS
STS = Secure Token Service


Creating IdentityServer4 migrations
```
add-migration initialPersistedGrantMigration -Context PersistedGrantDbContext -o Migrations/PersistedGrantDb

script-migration -Context PersistedGrantDbContext

add-migration initialConfigurationMigration -Context ConfigurationDbContext -o Migrations/ConfigurationDb

script-migration -Context ConfigurationDbContext
```