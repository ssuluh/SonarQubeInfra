using Pulumi;
using Pulumi.Kubernetes.Helm.V3;
using Pulumi.Kubernetes.Types.Inputs.Helm.V3;
using Pulumi.Kubernetes.Types.Outputs.Helm.V3;

namespace SonarQube;

public class PostgresResource : ComponentResource
{
    public PostgresResource(string name) : base("sonarqube-postgresql", name)
    {
        var cfg = new Config();
        var adminUser = cfg.Require("dbusername");
        var adminPassword = cfg.RequireSecret("dbpassword");
        
        var postgrestDb = new Release(
            name,
            new ReleaseArgs()
            {
                RepositoryOpts = new RepositoryOptsArgs()
                {
                    Repo = "https://charts.bitnami.com/bitnami",
                    
                },
                Atomic = true,
                Chart = "keycloak",
                WaitForJobs = true,
                
                Values = new InputMap<object>()
                {
                    ["auth.username"] = adminUser,
                    ["auth.password"] = adminPassword,
                    ["architecture"] = "replication",
                    ["readReplicas.replicaCount"] = 3
                }
            }
        );
        Status = postgrestDb.Status.Apply(s => s);

    }

    public Output<ReleaseStatus> Status { get; private set; }
}