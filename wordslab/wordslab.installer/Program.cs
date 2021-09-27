using Spectre.Console;
using System;
using System.Runtime.InteropServices;

namespace wordslab.installer
{
    // /mnt/c/Users/laure/OneDrive/Dev/C#/wordslab/wordslab/wordslab.installer/bin/Release/net6.0/publish/linux-x64

    class Program
    {
        static int Main(string[] args)
        {
            AnsiConsole.MarkupLine("[bold navy on grey93] --------------------------- [/]");
            AnsiConsole.MarkupLine("[bold navy on grey93] Wordslab Platform Installer [/]");
            AnsiConsole.MarkupLine("[bold navy on grey93] --------------------------- [/]");
            AnsiConsole.WriteLine();

            AnsiConsole.MarkupLine("1. [underline]Check operating system[/] :");
            AnsiConsole.WriteLine();

            var arch = RuntimeInformation.ProcessArchitecture;
            if (arch != Architecture.X64)
            {
                AnsiConsole.MarkupLine($"[red]Sorry, the Wordslab Platform is only supported on x64 systems[/] (your architecture = \"{arch}\")");
                AnsiConsole.WriteLine();
                return 1;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {                
                try
                {                    
                    if (!PlatformDetection.IsWindows10Version1903OrGreater)
                    {
                        var winver = Environment.OSVersion.Version;
                        AnsiConsole.MarkupLine($"[red]Sorry, Windows 10 version 1903 (build number > 18362) is required[/] (your version = \"{winver}\")");
                        AnsiConsole.MarkupLine("Please open Windows Update [yellow underline]ms-settings:windowsupdate[/] and check if a new version is available");
                        AnsiConsole.WriteLine();
                        return 1;
                    } 
                    else
                    {                        
                        AnsiConsole.MarkupLine("Windows version [bold green]OK[/]");
                        AnsiConsole.WriteLine();
                    }
                }
                catch(Exception e)
                {
                    AnsiConsole.MarkupLine("[red]Failed to check Windows version[/]");
                    AnsiConsole.WriteLine();
                    return 1;
                }

                return DoWindowsInstall();
            } 
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {                    
                    if (!PlatformDetection.IsUbuntu1804OrHigher)                            
                    {
                        var linuxdistro = PlatformDetection.GetDistroInfo();
                        AnsiConsole.MarkupLine($"[red]Sorry, Ubuntu version >= 18.04 is required[/] (your distribution = \"{linuxdistro.Id} {linuxdistro.VersionId}\")");
                        AnsiConsole.WriteLine();
                        return 1;
                    }
                    else
                    {
                        var linuxver = Environment.OSVersion.Version;
                        if (linuxver.Major < 5 || (linuxver.Major == 5 && linuxver.Minor < 4))
                        {
                            AnsiConsole.MarkupLine($"[red]Sorry, Linux kernel version >= 5.4 is required[/] (your version = \"{linuxver}\")");
                            AnsiConsole.WriteLine();
                            return 1;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("Linux version [bold green]OK[/]");
                            AnsiConsole.WriteLine();
                        }
                    }
                }
                catch (Exception e)
                {
                    AnsiConsole.MarkupLine("[red]Failed to check Linux version[/]");
                    AnsiConsole.WriteLine();
                    return 1;
                }

                return DoLinuxInstall();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                AnsiConsole.MarkupLine("[red]Sorry, OSX is not yet supported[/]");
                AnsiConsole.WriteLine();
                return 1;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                AnsiConsole.MarkupLine("[red]Sorry, FreeBSD is not supported[/]");
                AnsiConsole.WriteLine();
                return 1;
            } 
            else
            {
                AnsiConsole.MarkupLine("[red]Sorry, your operating system is not supported[/]");
                AnsiConsole.WriteLine();
                return 1;
            }
        }

        private static int DoWindowsInstall()
        {
            AnsiConsole.MarkupLine("2. [underline]Check Windows Subsystem for Linux[/] :");
            AnsiConsole.WriteLine();

            switch(Windows.Wsl.CheckWSLVersion())
            {
                case -1:
                    AnsiConsole.MarkupLine("[red]Windows Subsystem For Linux 2 is not installed[/]");
                    AnsiConsole.WriteLine();
                    if (PlatformDetection.IsWindows11Version2110OrGreater) 
                    {
                        AnsiConsole.MarkupLine("Please execute the command below in Windows Terminal (admin) [[Win+X]] then restart your machine :");
                        AnsiConsole.MarkupLine("[white]wsl --install[/] ");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("Please go to [yellow underline]https://docs.microsoft.com/en-us/windows/wsl/install-win10[/] and follow the instructions");
                        AnsiConsole.MarkupLine("Select the following Linux distribution : [white]Ubuntu 20.04[/]");
                    }
                    AnsiConsole.WriteLine();
                    return 1;
                case 0:
                    AnsiConsole.MarkupLine("Windows Subsystem For Linux 2 seems to be activated, but [red]no Linux distribution is installed[/]");
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("Please go to the URL below and select the following Linux distribution : [white]Ubuntu 20.04[/]");
                    AnsiConsole.MarkupLine("[yellow underline]https://docs.microsoft.com/en-us/windows/wsl/install-win10#step-6---install-your-linux-distribution-of-choice[/]");
                    AnsiConsole.WriteLine();
                    return 1;
                case 1:                
                    AnsiConsole.MarkupLine("Windows Subsystem For Linux 2 seems to be activated, but [red]your default Linux distribution is using WSL 1[/]");
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("Please use one of the commands below to migrate your distribution to WSL 2 or select another distribution :");
                    AnsiConsole.MarkupLine("[white]wsl --set-version <Distribution> 2[/] or  [white]wsl --set-default <Distribution>[/]");
                    AnsiConsole.WriteLine();
                    return 1;
                case 2:                    
                    break;
            }

            var linuxver = Windows.Wsl.CheckKernelVersion();
            if (linuxver.Major < 5 || (linuxver.Major == 5 && linuxver.Minor < 4))
            {
                AnsiConsole.MarkupLine($"[red]Sorry, Linux kernel version >= 5.4 is required in Windows Subsystem For Linux[/] (your version = \"{linuxver}\")");
                AnsiConsole.MarkupLine("Please download and execute the latest update at [yellow underline]https://aka.ms/wsl2kernel[/]");
                AnsiConsole.WriteLine();
                return 1;
            }

            string distrib;
            string distribver;
            if(!Windows.Wsl.CheckUbuntuDistribution(out distrib, out distribver))
            {
                AnsiConsole.MarkupLine($"[red]Sorry, Ubuntu version >= 18.04 is required in Windows Subsystem For Linux[/] (your distribution = \"{distrib} v{distribver}\")");
                AnsiConsole.MarkupLine("Please go to the URL below and select the following Linux distribution : [white]Ubuntu 20.04[/]");
                AnsiConsole.MarkupLine("[yellow underline]https://docs.microsoft.com/en-us/windows/wsl/install-win10#step-6---install-your-linux-distribution-of-choice[/]");
                AnsiConsole.WriteLine();
                return 1;
            }

            AnsiConsole.MarkupLine("Windows Subsystem For Linux [bold green]OK[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("Please note that you can limit the number of processors and the amount of memory assigned to the WSL VM in :");
            AnsiConsole.MarkupLine($"[white]{Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)}\\.wslconfig[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("You can visit the URL below for details :");
            AnsiConsole.MarkupLine("[yellow underline]https://docs.microsoft.com/en-us/windows/wsl/wsl-config#configure-global-options-with-wslconfig[/]");
            AnsiConsole.WriteLine();

            AnsiConsole.MarkupLine("3. [underline]Check Docker Desktop[/] :");
            AnsiConsole.WriteLine();

            if (Windows.Docker.CheckPreviousDockerVersionInWsl())
            {
                AnsiConsole.MarkupLine("[red]A previous version of Docker seems to be installed[/] in Windows Subsystem For Linux");
                AnsiConsole.WriteLine("Please execute the command below in your Linux shell to uninstall it :");
                AnsiConsole.MarkupLine("[white]sudo apt-get remove docker docker-engine docker.io containerd runc[/]");
                AnsiConsole.WriteLine();
                return 1;
            }

            var winDockerVersion = Windows.Docker.CheckWindowsDockerVersion();
            if(winDockerVersion == null)
            {
                AnsiConsole.MarkupLine($"[red]Docker Desktop for Windows is not installed[/]");
                AnsiConsole.MarkupLine("Please go to the URL below to install Docker Desktop with the [white]WSL 2 backend[/] :");
                AnsiConsole.MarkupLine("[yellow underline]https://docs.docker.com/docker-for-windows/install/[/]");
                AnsiConsole.WriteLine();
                return 1;
            } 
            else if (winDockerVersion.Major<20 || (winDockerVersion.Major==20 && winDockerVersion.Minor<10))
            {
                AnsiConsole.MarkupLine($"[red]Sorry, Docker engine version >= 20.10 is required[/] (your version = \"{winDockerVersion}\")");
                AnsiConsole.MarkupLine("Please go to the URL below to learn how to update Docker Desktop :");
                AnsiConsole.MarkupLine("[yellow underline]https://docs.docker.com/docker-for-windows/install/#updates[/]");
                AnsiConsole.WriteLine();
                return 1;
            }

            var linuxDockerVersion = Windows.Docker.CheckWindowsDockerVersion(fromWSL:true);
            if (linuxDockerVersion == null)
            {
                AnsiConsole.MarkupLine($"[red]Docker is not available in Windows Subsystem for Linux[/] because Docker Desktop is not running");
                AnsiConsole.MarkupLine("Please go to the URL below to learn how to start Docker Desktop [white]from the Windows Start menu[/] :");
                AnsiConsole.MarkupLine("[yellow underline]https://docs.docker.com/docker-for-windows/install/#start-docker-desktop[/]");
                AnsiConsole.WriteLine();
                return 1;
            }

            AnsiConsole.MarkupLine("Docker Desktop for Windows [bold green]OK[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("Please note that the Wordslab Platform will :");
            AnsiConsole.WriteLine("- start (and consume memory) as soon as you start Docker Desktop");
            AnsiConsole.WriteLine("- stop (and release memory) a few seconds after you stop Docker Desktop");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("To make sure memory is used only when needed, you can disable the setting \"[white]Start Docker Desktop when you log in[/]\"");
            AnsiConsole.WriteLine("You can visit the URL below for details on how to modify this setting :");
            AnsiConsole.MarkupLine("[yellow underline]https://docs.docker.com/docker-for-windows/#general[/]");
            AnsiConsole.WriteLine();

            AnsiConsole.MarkupLine("4. [underline]Switch to Linux to continue[/] :");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("You are now [bold green]READY[/] to continue the install procedure in Linux");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("Open your Ubuntu command shell in Windows Subsystem for Linux and execute the command below :");
            AnsiConsole.MarkupLine("[white]wget https://www.cognitivefactory.fr/assets/wordslab && chmod u+x wordslab && wordslab[/]");
            AnsiConsole.WriteLine();
            return 0;
        }

        private static int DoLinuxInstall()
        {
            AnsiConsole.MarkupLine("2. [underline]Check Docker version[/] :");
            AnsiConsole.WriteLine();

            var linuxDockerVersion = Linux.Docker.CheckDockerVersion();
            if (linuxDockerVersion == null)
            {
                AnsiConsole.MarkupLine($"[red]Docker is not available[/]");
                if (Linux.Docker.CheckWindowsSubsystemForLinux())
                {
                    AnsiConsole.MarkupLine("Please go to the URL below to learn how to start Docker Desktop [white]from the Windows Start menu[/] :");
                    AnsiConsole.MarkupLine("[yellow underline]https://docs.docker.com/docker-for-windows/install/#start-docker-desktop[/]");
                } 
                else
                {
                    AnsiConsole.MarkupLine("Please go to the URL below to learn how to install Docker on Ubuntu :");
                    AnsiConsole.MarkupLine("[yellow underline]https://docs.docker.com/engine/install/ubuntu/[/]");
                }
                AnsiConsole.WriteLine();
                return 1;
            }
            else if (linuxDockerVersion.Major < 20 || (linuxDockerVersion.Major == 20 && linuxDockerVersion.Minor < 10))
            {
                AnsiConsole.MarkupLine($"[red]Sorry, Docker engine version >= 20.10 is required[/] (your version = \"{linuxDockerVersion}\")");
                if (Linux.Docker.CheckWindowsSubsystemForLinux())
                {
                    AnsiConsole.MarkupLine("Please go to the URL below to learn how to update Docker Desktop :");
                    AnsiConsole.MarkupLine("[yellow underline]https://docs.docker.com/docker-for-windows/install/#updates[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("Please go to the URL below to learn how to upgrade Docker in Ubuntu :");
                    AnsiConsole.MarkupLine("[yellow underline]https://docs.docker.com/engine/install/ubuntu/[/]");
                }
                AnsiConsole.WriteLine();
                return 1;
            }

            AnsiConsole.MarkupLine("Docker engine [bold green]OK[/]");
            AnsiConsole.WriteLine();

            AnsiConsole.MarkupLine("3. [underline]Install k3d.io[/] (Kubernetes clusters in Docker) :");
            AnsiConsole.WriteLine();

            var k3dVersion = Linux.K3d.CheckK3dVersion();
            if (k3dVersion == null)
            {
                AnsiConsole.MarkupLine("[red]k3d is not installed[/]");
                AnsiConsole.WriteLine("Please execute the command below to install it :");
                AnsiConsole.MarkupLine("[white]wget -q -O - https://raw.githubusercontent.com/rancher/k3d/main/install.sh | bash[/]");
                AnsiConsole.WriteLine();
                return 1;
            }
            else if (k3dVersion.Major < 4 || (k3dVersion.Major == 4 && k3dVersion.Minor < 4))
            {
                AnsiConsole.MarkupLine($"[red]Sorry, k3d version >= 4.4 is required[/] (your version = \"{k3dVersion}\")");
                AnsiConsole.WriteLine("Please execute the command below to update k3d to the latest version :");
                AnsiConsole.MarkupLine("[white]wget -q -O - https://raw.githubusercontent.com/rancher/k3d/main/install.sh | bash[/]");
                AnsiConsole.WriteLine();
                return 1;
            }

            AnsiConsole.MarkupLine("k3d version [bold green]OK[/]");
            AnsiConsole.WriteLine();

            AnsiConsole.MarkupLine("4. [underline]Check kubectl[/] (Kubernetes client) :");
            AnsiConsole.WriteLine();

            var kubectlVersion = Linux.Kubectl.CheckKubectlVersion();
            if (kubectlVersion == null)
            {
                AnsiConsole.MarkupLine("[red]kubectl is not installed[/]");
                AnsiConsole.WriteLine("Please execute the two commands below to install it :");
                AnsiConsole.MarkupLine("[white]curl -LO \"https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl\"[/]");
                AnsiConsole.MarkupLine("[white]sudo install -o root -g root -m 0755 kubectl /usr/local/bin/kubectl[/]");
                AnsiConsole.WriteLine();
                return 1;
            }

            AnsiConsole.MarkupLine("kubectl [bold green]OK[/]");
            AnsiConsole.WriteLine();

            AnsiConsole.MarkupLine("5. [underline]Check Helm[/] (package manager for Kubernetes) :");
            AnsiConsole.WriteLine();

            var helmVersion = Linux.Helm.CheckHelmVersion();
            if (helmVersion == null || helmVersion.Major < 3)
            {
                AnsiConsole.MarkupLine("[red]Helm 3 is not installed[/]");
                AnsiConsole.WriteLine("Please execute the command below to install it :");
                AnsiConsole.MarkupLine("[white]curl https://raw.githubusercontent.com/helm/helm/master/scripts/get-helm-3 | bash[/]");
                AnsiConsole.WriteLine();
                return 1;
            }

            AnsiConsole.MarkupLine("Helm version [bold green]OK[/]");
            AnsiConsole.WriteLine();

            AnsiConsole.MarkupLine("6. [underline]Create Kubernetes cluster[/] :");
            AnsiConsole.WriteLine();

            var clusterName = "wordslab-cluster";
            int agents = 3;
            if(!Linux.K3d.DoesK3dClusterExist(clusterName))
            {
                AnsiConsole.WriteLine($"Creating cluster {clusterName} ... (this may take several minutes)");
                var commandIfError = Linux.K3d.CreateK3dCluster(clusterName: clusterName, agents: agents, hostWebPort: 8080);
                if (commandIfError != null)
                {
                    AnsiConsole.MarkupLine("[red]Failed to create k3d cluster[/]");
                    AnsiConsole.WriteLine("Please execute the command below to create it manually and fix errors :");
                    AnsiConsole.MarkupLine($"[white]{commandIfError}[/]");
                    AnsiConsole.WriteLine();
                    return 1;
                }

                AnsiConsole.WriteLine($"Labeling cluster nodes ...");
                var messageIfError = Linux.K3d.LabelK3dClusterNodes(clusterName: clusterName, agents: agents);
                if (messageIfError != null)
                {
                    AnsiConsole.MarkupLine("[red]Failed to label k3d cluster nodes[/]");
                    AnsiConsole.MarkupLine($"{messageIfError}");
                    AnsiConsole.WriteLine();
                    return 1;
                }
                else
                {
                    AnsiConsole.WriteLine();
                }
            }

            AnsiConsole.MarkupLine($"Cluster {clusterName} [bold green]OK[/]");
            AnsiConsole.WriteLine();

            /*
            AnsiConsole.MarkupLine("7. [underline]Install Yugabyte database[/] :");
            AnsiConsole.WriteLine();

            var installName = "wordslab-db";
            AnsiConsole.WriteLine($"Installing database {installName} ... (this may take several minutes)");
            var diagnosticIfError = Linux.Yugabyte.InstallYugabyteDB(clusterName, installNamespace: installName, installName: installName);
            if (diagnosticIfError != null)
            {
                AnsiConsole.MarkupLine("[red]Failed to install Yugabyte database[/]");
                AnsiConsole.WriteLine("Installation error diagnostic :");
                AnsiConsole.WriteLine(diagnosticIfError);
                AnsiConsole.WriteLine();
                return 1;
            }
            else
            {
                AnsiConsole.WriteLine();
            }
            */

            AnsiConsole.MarkupLine("7. [underline]Install Postgresql database[/] :");
            AnsiConsole.WriteLine();

            var installName = "wordslab-db";
            int postgresqlPort = 5432; 
            var sqlUser = "wordslab";
            var sqlPassword = "wordsl@b2021";
            AnsiConsole.WriteLine($"Installing database {installName} ... (this may take several minutes)");
            var diagnosticIfError = Linux.Postgresql.InstallPostgresql(databaseName : installName, installNamespace: installName, helmInstallName: installName,
                sqlUser: sqlUser, sqlPassword: sqlPassword, storageClass: "local-path", port: postgresqlPort);
            if (diagnosticIfError != null)
            {
                AnsiConsole.MarkupLine("[red]Failed to install Postgresql database[/]");
                AnsiConsole.WriteLine("Installation error diagnostic :");
                AnsiConsole.WriteLine(diagnosticIfError);
                AnsiConsole.WriteLine();
                return 1;
            }
            else
            {
                AnsiConsole.WriteLine();
                AnsiConsole.WriteLine($"PostgreSQL can be accessed via port {postgresqlPort} on the following DNS names from within your cluster:");
                AnsiConsole.MarkupLine($"   [white]{installName}-postgresql.{installName}.svc.cluster.local[/]");
                AnsiConsole.WriteLine("To get the password for \"postgres\" run:");
                AnsiConsole.MarkupLine($"   [white]export POSTGRES_PASSWORD=$(kubectl get secret --namespace {installName} {installName}-postgresql -o jsonpath=\"{{.data.postgresql-password}}\" | base64 --decode)[/]");
                AnsiConsole.WriteLine("To connect to your database from outside the cluster execute the following commands:");
                AnsiConsole.MarkupLine($"   [white]kubectl port-forward --namespace {installName} svc/{installName}-postgresql {postgresqlPort}:{postgresqlPort}[/]");
                AnsiConsole.WriteLine();
            }

            AnsiConsole.MarkupLine($"Database {installName} [bold green]OK[/]");
            AnsiConsole.WriteLine();

            return 0;
        }
    }
}
