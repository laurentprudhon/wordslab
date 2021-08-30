using System;

namespace wordslab.installer.Linux
{
    class K3d
    {
        // Executes : k3d version
        // Returns  : 
        // Version object if k3d.io version was correctly parsed
        // null otherwise
        public static Version CheckK3dVersion()
        {
            try
            {
                string output;
                string error;
                int exitcode = Process.Run("k3d", "version", 5, out output, out error);
                if (exitcode == 0 && String.IsNullOrEmpty(error) && !String.IsNullOrEmpty(output))
                {
                    // k3d version v4.4.7
                    // k3s version v1.21.2-k3s1 (default)
                    int versionIndex = output.IndexOf("version");
                    if (versionIndex > 0 && (versionIndex + 9) < output.Length)
                    {
                        versionIndex += 9;
                        int firstDot = output.IndexOf('.', versionIndex);
                        int secondDot = output.IndexOf('.', firstDot + 1);
                        int eol = output.IndexOf('\n', secondDot + 1);
                        if (firstDot > versionIndex && secondDot > firstDot && eol > secondDot)
                        {
                            var major = Int32.Parse(output.Substring(versionIndex, firstDot - versionIndex));
                            var minor = Int32.Parse(output.Substring(firstDot + 1, secondDot - firstDot - 1));
                            var build = Int32.Parse(output.Substring(secondDot + 1, eol - secondDot - 1));
                            return new Version(major, minor, build);
                        }
                    }
                }
            }
            catch (Exception)
            { }
            return null;
        }

        // Executes : k3d cluster list
        // Returns  : true if cluster already exists
        public static bool DoesK3dClusterExist(string clusterName)
        {
            try
            {
                string output;
                string error;
                int exitcode = Process.Run("k3d", "cluster list", 5, out output, out error);
                if (exitcode == 0 && String.IsNullOrEmpty(error) && !String.IsNullOrEmpty(output))
                {
                    // NAME                 SERVERS   AGENTS   LOADBALANCER
                    // cogfactory-cluster   1/1       3/3      true
                    var lines = output.Split('\n');
                    foreach (var line in lines)
                    {
                        if (line.StartsWith(clusterName))
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

        // Executes : k3d cluster create ...
        // Returns  : 
        // null is the cluster was sucessfully created
        // command string if the create command failed
        public static string CreateK3dCluster(string clusterName, int agents = 3, int hostWebPort = 8080,
            bool createRegistry = true, bool exposeYDBPorts = true, bool mapHostPathDirectories = true, bool updateKubeconfig = true)
        {
            var command = $"cluster create {clusterName} --agents {agents} -p {hostWebPort}:80@loadbalancer";
            if (exposeYDBPorts)
            {
                command += " -p 7000:7000@loadbalancer -p 9042:9042@loadbalancer -p 6379:6379@loadbalancer -p 5433:5433@loadbalancer";
            }
            if (createRegistry)
            {
                command += " --registry-create";
            }
            if (mapHostPathDirectories)
            {
                for (int i = 0; i < agents; i++)
                {
                    command += $" --volume /var/lib/{clusterName}/storage/agent{i}:/var/lib/rancher/k3s/storage@agent[{i}]";
                }
            }
            if (updateKubeconfig)
            {
                command += " --kubeconfig-update-default=true";
            }
            try
            {
                string output;
                string error;
                int exitcode = Process.Run("k3d", command, 900, out output, out error);
                if (exitcode == 0 && String.IsNullOrEmpty(error) && !String.IsNullOrEmpty(output))
                {
                    return null;
                }
            }
            catch (Exception)
            { }
            return command;
        }

        // Executes : kubectl label nodes ...
        // Returns  : 
        // null is the nodes were sucessfully labeled
        // output+error string if the create command failed
        public static string LabelK3dClusterNodes(string clusterName, int agents = 3, string label = "disk=local")
        {
            for (int i = 0; i < agents; i++)
            {
                var command = $"label --overwrite nodes k3d-{clusterName}-agent-{i} {label}";
                try
                {
                    string output;
                    string error;
                    int exitcode = Process.Run("kubectl", command, 5, out output, out error);
                    if (exitcode != 0 || !String.IsNullOrEmpty(error))
                    {
                        return output + " | " + error;
                    }
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            return null;
        }

        /* Debug and explore the k3d nodes
          
        k3d node list

NAME                              ROLE           CLUSTER              STATUS
k3d-cogfactory-cluster-agent-0    agent          cogfactory-cluster   running

        docker exec -it k3d-cogfactory-cluster-agent-0 sh

        crictl images

IMAGE                               TAG                 IMAGE ID            SIZE
docker.io/rancher/klipper-lb        v0.2.0              465db341a9e5b       2.71MB
docker.io/rancher/library-busybox   1.32.1              388056c9a6838       768kB
docker.io/rancher/pause             3.1                 da86e6ba6ca19       327kB
docker.io/yugabytedb/yugabyte       2.7.1.1-b1          ad3a83a790f47       737MB

        crictl ps

CONTAINER           IMAGE               CREATED             STATE               NAME                ATTEMPT             POD ID
ffba05b14613e       465db341a9e5b       24 hours ago        Running             lb-port-5433        1                   3d3838c11a252
b43db97b92c5f       465db341a9e5b       24 hours ago        Running             lb-port-9042        1                   3d3838c11a252
71f62cf85b207       ad3a83a790f47       24 hours ago        Running             yb-cleanup          1                   24173b83480fe
01a87033f6100       465db341a9e5b       24 hours ago        Running             lb-port-6379        1                   3d3838c11a252
0290e4e24534f       ad3a83a790f47       24 hours ago        Running             yb-tserver          1                   24173b83480fe
d7c7e74b6c431       465db341a9e5b       24 hours ago        Running             lb-port-443         3                   910a641dcdebf
b9f25a4125000       ad3a83a790f47       24 hours ago        Running             yb-cleanup          1                   0e01b35c758dc
884ef822288bb       465db341a9e5b       24 hours ago        Running             lb-port-80          3                   910a641dcdebf
506d36178a085       ad3a83a790f47       24 hours ago        Running             yb-master           1                   0e01b35c758dc
0de35419e0568       465db341a9e5b       24 hours ago        Running             lb-port-7000        1                   3995cbbdecb95

        crictl info
        
[k3d-cogfactory-cluster-registry:5000]

        crictl stats

CONTAINER           CPU %               MEM                 DISK                INODES
01a87033f6100       0.00                1.962MB             24.58kB             9
0290e4e24534f       10.31               90.71MB             45.06kB             14
0de35419e0568       0.00                1.991MB             24.58kB             9
506d36178a085       0.77                37.49MB             32.77kB             10
71f62cf85b207       0.00                4.03MB              45.06kB             13
884ef822288bb       0.00                2.208MB             24.58kB             9
b43db97b92c5f       0.00                1.688MB             24.58kB             9
b9f25a4125000       0.00                5.358MB             45.06kB             13
d7c7e74b6c431       0.00                1.942MB             24.58kB             9
ffba05b14613e       0.00                1.331MB             24.58kB             9

        */
    }
}
