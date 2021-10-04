using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wordslab.installer.Linux
{
    class Postgresql
    {
        // Executes : kubectl get services
        // Returns  : true if database is already installed
        public static bool IsPostgresqlInstalled(string installName, string installNamespace)
        {
            try
            {
                string output;
                string error;
                int exitcode = Process.Run("kubectl", $"get services -n {installNamespace}", 5, out output, out error);
                if (exitcode == 0 && String.IsNullOrEmpty(error) && !String.IsNullOrEmpty(output))
                {
                    // NAME                              TYPE        CLUSTER-IP     EXTERNAL-IP   PORT(S)    AGE
                    // wordslab-db-postgresql            ClusterIP   ...
                   var lines = output.Split('\n');
                    foreach (var line in lines)
                    {
                        if (line.StartsWith($"{installName}-postgresql"))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            { }
            return false;
        }

        // Executes : helm install bitnami/postgresql
        // Returns : 
        // null if install was successful
        // diagnostic string if one of the commands failed
        public static string InstallPostgresql(string helmInstallName, string installNamespace, 
            string databaseName, string sqlUser, string sqlPassword, 
            string storageClass = "local-path", int port = 5432)
        {
            // https://github.com/bitnami/charts/tree/master/bitnami/postgresql/#installing-the-chart

            // 1. helm repo add bitnami https://charts.bitnami.com/bitnami
            try
            {
                string output;
                string error;
                int exitcode = Process.Run("helm", "repo add bitnami https://charts.bitnami.com/bitnami", 5, out output, out error);
                if (exitcode != 0 || !String.IsNullOrEmpty(error))
                {
                    return $"helm repo add : exitcode={exitcode}, output=\"{output}\", error=\"{error}\"";
                }
            }
            catch (Exception e)
            {
                return $"helm repo add : exception=\"{e.Message}\"";
            }

            // 2. helm repo update
            try
            {
                string output;
                string error;
                int exitcode = Process.Run("helm", "repo update", 30, out output, out error);
                if (exitcode != 0 || !String.IsNullOrEmpty(error))
                {
                    return $"helm repo update : exitcode={exitcode}, output=\"{output}\", error=\"{error}\"";
                }
            }
            catch (Exception e)
            {
                return $"helm repo update : exception=\"{e.Message}\"";
            }

            // 3. kubectl create namespace
            if (installNamespace != "default")
            {
                try
                {
                    string output;
                    string error;
                    int exitcode = Process.Run("kubectl", $"create namespace {installNamespace}", 5, out output, out error);
                    if (!(!String.IsNullOrEmpty(error) && error.Contains("AlreadyExists")) && (exitcode != 0 || !String.IsNullOrEmpty(error)))
                    {
                        return $"kubectl create namespace : exitcode={exitcode}, output=\"{output}\", error=\"{error}\"";
                    }
                }
                catch (Exception e)
                {
                    return $"kubectl create namespace : exception=\"{e.Message}\"";
                }
            }

            // 4. helm install bitnami/postgresql
            // ,
            try
            {
                string output;
                string error;
                string args = $"install {helmInstallName} bitnami/postgresql --namespace={installNamespace}";
                args += " --set image.tag=11.13.0-debian-10-r40,";
                args += $"postgresqlDatabase={databaseName},postgresqlUsername={sqlUser},postgresqlPassword={sqlPassword},";
                args += $"service.port={port},";
                args += $"persistence.storageClass={storageClass},volumePermissions.enabled=true,primary.nodeSelector.disk=local";
                args += " --wait --timeout 5m";
                int exitcode = Process.Run("helm", args, 300, out output, out error);
                if (exitcode != 0)
                {
                    return $"helm install bitnami/postgresql : exitcode={exitcode}, output=\"{output}\", error=\"{error}\"";
                }
            }
            catch (Exception e)
            {
                return $"helm install bitnami/postgresql : exception=\"{e.Message}\"";
            }
            return null;

            /*
helm install wordslab-db bitnami/postgresql --set postgresqlDatabase=wordslab,postgresqlUsername=wrdadmin,postgresqlPassword=wrdadmin,service.port=5432,persistence.storageClass=local-path,volumePermissions.enabled=true,primary.nodeSelector.disk=local
             
              
PostgreSQL can be accessed via port 5432 on the following DNS names from within your cluster:

    wordslab-db-postgresql.default.svc.cluster.local - Read/Write connection

To get the password for "postgres" run:

    export POSTGRES_ADMIN_PASSWORD=$(kubectl get secret --namespace default wordslab-db-postgresql -o jsonpath="{.data.postgresql-postgres-password}" | base64 --decode)

To get the password for "wrdadmin" run:

    export POSTGRES_PASSWORD=$(kubectl get secret --namespace default wordslab-db-postgresql -o jsonpath="{.data.postgresql-password}" | base64 --decode)

To connect to your database run the following command:

    kubectl run wordslab-db-postgresql-client --rm --tty -i --restart='Never' --namespace default --image docker.io/bitnami/postgresql:11.13.0-debian-10-r40 --env="PGPASSWORD=$POSTGRES_PASSWORD" --command -- psql --host wordslab-db-postgresql -U wrdadmin -d wordslab -p 5432



To connect to your database from outside the cluster execute the following commands:

    kubectl port-forward --namespace default svc/wordslab-db-postgresql 5432:5432 &
    PGPASSWORD="$POSTGRES_PASSWORD" psql --host 127.0.0.1 -U wrdadmin -d wordslab -p 5432
             */
        }
    }
}
