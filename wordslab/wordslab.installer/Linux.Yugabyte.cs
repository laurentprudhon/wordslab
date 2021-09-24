using System;

namespace wordslab.installer.Linux
{
    class Yugabyte
    {
        // Executes : helm install yugabytedb/yugabyte
        // Returns : 
        // null if install was successful
        // diagnostic string if one of the commands failed
        public static string InstallYugabyteDB(string clusterName, string installNamespace="wordslab-db", string installName="wordslab-db", 
            string masterCpuRequest="0.5", string masterMemoryRequest= "0.5Gi", string masterStorageClass="local-path",
            string tserverCpuRequest = "0.5", string tserverMemoryRequest = "0.5Gi", string tserverStorageClass="local-path" )
        {
            // https://docs.yugabyte.com/latest/deploy/kubernetes/single-zone/oss/helm-chart/

            // 1. helm repo add yugabytedb https://charts.yugabyte.com
            try
            {
                string output;
                string error;
                int exitcode = Process.Run("helm", "repo add yugabytedb https://charts.yugabyte.com", 5, out output, out error);
                if (exitcode != 0 || !String.IsNullOrEmpty(error))
                {
                    return $"helm repo add : exitcode={exitcode}, output=\"{output}\", error=\"{error}\"";
                }
            }
            catch(Exception e)
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

            // 3. kubectl create namespace wordslab-db
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

            var sqlUser = "yugabyte";
            var sqlPassword = "wordsl@b2021";

            var cqlUser = "cassandra";
            var cqlPassword = "wordsl@b2021";

            // 4. helm install yugabytedb/yugabyte
            try
            {
                string output;
                string error;
                string args = $"install {installName} yugabytedb/yugabyte --namespace {installNamespace}";
                args += $" --set Image.repository=k3d-{clusterName}-registry:5000/yugabyte,Image.tag=2.9.0.0-b4,";
                args += $"resource.master.requests.cpu={masterCpuRequest},resource.master.requests.memory={masterMemoryRequest},storage.master.storageClass={masterStorageClass},";
                args += $"resource.tserver.requests.cpu={tserverCpuRequest},resource.tserver.requests.memory={tserverMemoryRequest},storage.tserver.storageClass={tserverStorageClass},";
                args += "nodeSelector.disk=local,";
                args += $"authCredentials.ysql.user={sqlUser},authCredentials.ysql.password={sqlPassword},";
                args += $"authCredentials.ycql.user={cqlUser},authCredentials.ycql.password={cqlPassword}";
                args += " --wait --timeout 30m";
                int exitcode = Process.Run("helm", args, 1800, out output, out error);
                if (exitcode != 0)
                {
                    return $"helm install yugabytedb/yugabyte : exitcode={exitcode}, output=\"{output}\", error=\"{error}\"";
                }
            }
            catch (Exception e)
            {
                return $"helm install yugabytedb/yugabyte : exception=\"{e.Message}\"";
            }
            return null;

            /*
             Optimize image pull :

            docker pull yugabytedb/yugabyte:2.9.0.0-b4

            docker images

REPOSITORY            TAG            IMAGE ID       CREATED        SIZE
rancher/k3d-proxy     v4.4.7         c025ba52d327   2 weeks ago    44.7MB
rancher/k3s           v1.21.2-k3s1   b41b52c9bb59   4 weeks ago    172MB
yugabytedb/yugabyte   2.9.0.0-b4     ad3a83a790f4   8 weeks ago    1.95GB
registry              2              1fd8e1b0bb7e   3 months ago   26.2MB

            docker ps -f name=k3d-wordslab-cluster-registry

CONTAINER ID   IMAGE        COMMAND                  CREATED      STATUS        PORTS                     NAMES
5d4673889bb5   registry:2   "/entrypoint.sh /etc…"   3 days ago   Up 26 hours   0.0.0.0:45033->5000/tcp   k3d-wordslab-cluster-registry

            docker tag yugabytedb/yugabyte:2.9.0.0-b4 127.0.0.1:45033/yugabyte:2.9.0.0-b4
            docker push 127.0.0.1:45033/yugabyte:2.9.0.0-b4

            docker images 127.0.0.1:45033
            => KO

            docker image rm -f ad3a83a790f4            

            [helm => --set Image.repository=k3d-wordslab-cluster-registry:5000/yugabyte,Image.tag=2.9.0.0-b4]

             */

            /* Then check install  / upgrade / uninstall / delete PVC :

            kubectl get pvc,po,sts,svc -n wordslab-db

            helm status wordslab-db -n wordslab-db

            PGSQL connection : Host=localhost;Port=5433;Database=yugabyte;Username=yugabyte;Password=yugabyte

            helm upgrade wordslab-db yugabytedb/yugabyte --set Image.tag= 2.1.6.0-b17 --wait -n wordslab-db

            helm uninstall wordslab-db -n wordslab-db

            kubectl delete pvc --namespace wordslab-db --all
            */

            /* Initial CONFIG
             
            kubectl exec --namespace wordslab-db -it yb-tserver-0 -- /home/yugabyte/bin/ysqlsh -h yb-tserver-0.yb-tservers.wordslab-db

            kubectl exec --namespace wordslab-db -it yb-tserver-0 -- /home/yugabyte/bin/ycqlsh yb-tserver-0.yb-tservers.wordslab-db -u cassandra

            yugabyte=# \l
                                   List of databases
      Name       |  Owner   | Encoding | Collate |    Ctype    |   Access privileges
-----------------+----------+----------+---------+-------------+-----------------------
 postgres        | postgres | UTF8     | C       | en_US.UTF-8 |
 system_platform | postgres | UTF8     | C       | en_US.UTF-8 |
 template0       | postgres | UTF8     | C       | en_US.UTF-8 | =c/postgres          +
                 |          |          |         |             | postgres=CTc/postgres
 template1       | postgres | UTF8     | C       | en_US.UTF-8 | =c/postgres          +
                 |          |          |         |             | postgres=CTc/postgres
 yugabyte        | postgres | UTF8     | C       | en_US.UTF-8 |

            yugabyte=# \du
                                   List of roles
 Role name |                         Attributes                         | Member of
-----------+------------------------------------------------------------+-----------
 postgres  | Superuser, Create role, Create DB, Replication, Bypass RLS | {}
 yugabyte  | Superuser, Create role, Create DB, Replication, Bypass RLS | {}

            yugabyte=# select 'Database : ' ||current_database()||', '||'User : '|| user db_details;
              db_details
--------------------------------------
 Database : yugabyte, User : yugabyte

            yugabyte=# \dn
  List of schemas
  Name  |  Owner
--------+----------
 public | postgres

            yugabyte=# \dt
Did not find any relations.

            yugabyte=# show search_path;
   search_path
-----------------
 "$user", public

            => same in postgres, system_platform databases

             */
        }
    }
}
