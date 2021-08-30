# Windows

## Windows Subsystem for Linux

### Linux filesystem

C:\Users\laure\AppData\Local\Packages\CanonicalGroupLimited.Ubuntu18.04onWindows_79rhkp1fndgsc\LocalState

ext4.vhdx : **1.5 Go**

### Linux swap

C:\Users\laure\AppData\Local\Temp

swap.vhdx : **66 Mo** (only while VM launched)

### Vmmem process

START Docker

memory : **1.2 Go** (only while VM launched)

STOP Docker

wait exactly ONE minute : process stopped and memory freed

wsl --shutdown

immediate : process stopped and memory freed

## Docker Desktop

**Windows install**

C:\Program Files\Docker : **2.2 Go**

**Linux install**

C:\Users\laure\AppData\Local\Docker\wsl\distro

ext4.vhdx : **105 Mo**

**Linux data**

C:\Users\laure\AppData\Local\Docker\wsl\data

ext4.vhdx : **890 Mo**

# Linux

## k3d install

/usr/local/bin/k3d : **16 Mo**

## k3d 3 nodes cluster

(base) laurent@YOGA720:~$ df
Filesystem     1K-blocks      Used Available Use% Mounted on
/dev/sdb       263174212  36261396 213474660  15% /

Vmmem : 303 Mo

docker system df -v : empty

C:\Users\laure\AppData\Local\Docker\wsl\data : 1582 Mo

1 min 30

Vmmem : 2 Go

4+2 containers runnning
3 images : 45 + 172 + 26 Mo
many volumes created

(base) laurent@YOGA720:~$ df
Filesystem     1K-blocks      Used Available Use% Mounted on
/dev/sdb       263174212  36261428  213474628  15% /

=> /dev/sdd       263174212   2575308//1589860 247160748   2% /mnt/wsl/docker-desktop-data/isocache
none             3190232        12   3190220   1% /mnt/wsl/docker-desktop/shared-sockets/host-services
/dev/sdc       263174212    134168 249601888   1% /mnt/wsl/docker-desktop/docker-desktop-proxy
/dev/loop0        404596    404596         0 100% /mnt/wsl/docker-desktop/cli-tools

C:\Users\laure\AppData\Local\Docker\wsl\data : 2663 Mo
=> + 986 Mo

everything is in vhdx file
this is a separate distribution in wsl
you can access the files from windows with :
\\wsl$\docker-desktop-data\version-pack-data\community\docker

memory (Docker) : 1256 Mo
- registry : 11 Mo
- server-0 : 639 Mo
- agent-0 : 249 Mo
- agent-1 : 211 Mo
- agent-2 : 138 Mo
- serverlb : 8 Mo

memory (top) : 1539 Mo

Vmmem redescendu à 1209 Mo

### Helm

/dev/sdd       263174212  36262092 213473964  15% /
/dev/sdd       263174212  36306152 213429904  15% /

44 Mo

# Kubernetes

## YugabyteDB

k3d cluster create time : 1m02s
YugabyteDB first install time : 7min15s

*** Avant ***

df
/dev/sdd       263174212  36306824 213429232  15% /

docker system df -v

Images space usage:

REPOSITORY          TAG            IMAGE ID       CREATED        SIZE      SHARED SIZE   UNIQUE SIZE   CONTAINERS
rancher/k3d-proxy   v4.4.7         c025ba52d327   10 days ago    44.65MB   0B            44.65MB       1
rancher/k3s         v1.21.2-k3s1   b41b52c9bb59   4 weeks ago    172MB     0B            172MB         4
registry            2              1fd8e1b0bb7e   3 months ago   26.25MB   0B            26.25MB       1

Containers space usage:

CONTAINER ID   IMAGE                      COMMAND                  LOCAL VOLUMES   SIZE      CREATED              STATUS              NAMES
e5c0c29e3cc2   rancher/k3d-proxy:v4.4.7   "/bin/sh -c nginx-pr…"   0               1.64kB    About a minute ago   Up About a minute   k3d-cogfactory-cluster-serverlb
7cce5556b09d   rancher/k3s:v1.21.2-k3s1   "/bin/k3s agent"         5               710B      About a minute ago   Up About a minute   k3d-cogfactory-cluster-agent-2
ac7962ce455a   rancher/k3s:v1.21.2-k3s1   "/bin/k3s agent"         5               710B      About a minute ago   Up About a minute   k3d-cogfactory-cluster-agent-1
459808c5e843   rancher/k3s:v1.21.2-k3s1   "/bin/k3s agent"         5               710B      About a minute ago   Up About a minute   k3d-cogfactory-cluster-agent-0
cd345eb54caa   rancher/k3s:v1.21.2-k3s1   "/bin/k3s server --t…"   5               3.67MB    About a minute ago   Up About a minute   k3d-cogfactory-cluster-server-0
58f0f2285a3e   registry:2                 "/entrypoint.sh /etc…"   1               0B        About a minute ago   Up About a minute   k3d-cogfactory-cluster-registry

Local Volumes space usage:

VOLUME NAME                                                        LINKS     SIZE
8de2ba003a0019a39b85f5003b3668a4def2122a529a671e8451d621f73ae32c   1         719B
57f099b0f68a213f40e7e889eb3d7b88dbeea1c11701c981aedea6c2cab4bc8e   1         223MB
66ed0c86385a5c2454d7ea54ccb7073ba27935926ef396a1f89babe60d15ef55   1         2.876kB
06248cbe55ff4df5bb6fc38122cd82ade25c71296bfbd14f3f7cc27b17260485   1         10.23MB
85089c38c121da364d6445e5cd3b2ae104d6d92e724f219223eaa9cbcee764f4   1         0B
6f36f9bef06776bd68a8433e2ad6d3a569efc60b6bd8e735c38e5e419b8e5ce0   1         16.9kB
d1eca229ac4339c274ad424dfaf2c2fef4fd925cfe51c4e4b47f5f2e71e7521a   1         2.876kB
182192dae207fcf59e6d1c1871c62e6d4559558031904893c958c84b10b628d0   1         4.287kB
fbf13f1ca07f761a88676952e4bdbc9508caffc66632e0ac2fc113ac8c5b2e4e   1         1.738kB
38175e1ceff4554c04eb405a1cd35c40b9daa370724ba20e19688851e53f61b3   1         3.542kB
07f878bddf5e1f1b8f17ac862656918cf846c1820e1f8aeee3d0280a9333d743   1         173.4MB
68a4a1cbf6d8e1d3bf404addb0ed6268eadeef002b845b1e5ca8983ad428acfd   1         10.52kB
f87e8ca756130cd7762d572668dd03748fe11267a76c15c04ee1b9565e22577e   1         129.7MB
k3d-cogfactory-cluster-images                                      4         0B
a8882c64e10f1960606740439a1117514e90e1e123669980c0f74cc547632cbf   1         5.457kB
532042b2eab12101ee276aae5f4a7175c937cd0748ef8e5f98a87830b6757d2a   1         492B
3993fefc0351552e5b0cd1dae38b149688baea42daeb76edad968426a8a443ab   1         2.762kB
0666dd78224533f475cf6438062788e16926b4a2ab89f6dc491bb63d2d693c75   1         275B             1fd8e1b0bb7e   3 months ago   26.25MB   0B            26.25MB       0

C:\Users\laure\AppData\Local\Docker\wsl\data : 2 840 576

ls -al /var/lib/cogfactory-cluster/storage/agent0 : empty

Vmmem : 2543,5 Mo

*** Après ***

/dev/sdd       263174212  36422132//36306824 213313924  15% /
=> + 115 Mo

(base) laurent@YOGA720:~$ ls -al /var/lib/cogfactory-cluster/storage/agent0

du -s -BM /var/lib/cogfactory-cluster/storage/agent1/pvc-3d5f38f8-6483-4c15-93b1-6e97ac15cf3c_
cogfactory-db_datadir1-yb-tserver-1/
=> master : 28 Mo
=> tserver : 28 Mo
x 3

Vmmem : 1540 Mo / 1930 Mo / 3186 Mo

C:\Users\laure\AppData\Local\Docker\wsl\data : 15 738 926 // 2 840 576
=> + 12.9 Go !!

docker system df -v

(base) laurent@YOGA720:~$ docker system df -v
Images space usage:

REPOSITORY          TAG            IMAGE ID       CREATED        SIZE      SHARED SIZE   UNIQUE SIZE   CONTAINERS
rancher/k3d-proxy   v4.4.7         c025ba52d327   10 days ago    44.65MB   0B            44.65MB       1
rancher/k3s         v1.21.2-k3s1   b41b52c9bb59   4 weeks ago    172MB     0B            172MB         4
registry            2              1fd8e1b0bb7e   3 months ago   26.25MB   0B            26.25MB       1

Containers space usage:

CONTAINER ID   IMAGE                      COMMAND                  LOCAL VOLUMES   SIZE      CREATED          STATUS          NAMES
e5c0c29e3cc2   rancher/k3d-proxy:v4.4.7   "/bin/sh -c nginx-pr…"   0               1.64kB    58 minutes ago   Up 57 minutes   k3d-cogfactory-cluster-serverlb
7cce5556b09d   rancher/k3s:v1.21.2-k3s1   "/bin/k3s agent"         5               710B      58 minutes ago   Up 58 minutes   k3d-cogfactory-cluster-agent-2
ac7962ce455a   rancher/k3s:v1.21.2-k3s1   "/bin/k3s agent"         5               710B      58 minutes ago   Up 58 minutes   k3d-cogfactory-cluster-agent-1
459808c5e843   rancher/k3s:v1.21.2-k3s1   "/bin/k3s agent"         5               710B      58 minutes ago   Up 58 minutes   k3d-cogfactory-cluster-agent-0
cd345eb54caa   rancher/k3s:v1.21.2-k3s1   "/bin/k3s server --t…"   5               3.67MB    58 minutes ago   Up 58 minutes   k3d-cogfactory-cluster-server-0
58f0f2285a3e   registry:2                 "/entrypoint.sh /etc…"   1               0B        58 minutes ago   Up 57 minutes   k3d-cogfactory-cluster-registry

Local Volumes space usage:

VOLUME NAME                                                        LINKS     SIZE
85089c38c121da364d6445e5cd3b2ae104d6d92e724f219223eaa9cbcee764f4   1         0B
6f36f9bef06776bd68a8433e2ad6d3a569efc60b6bd8e735c38e5e419b8e5ce0   1         695.5kB
d1eca229ac4339c274ad424dfaf2c2fef4fd925cfe51c4e4b47f5f2e71e7521a   1         13.82kB
182192dae207fcf59e6d1c1871c62e6d4559558031904893c958c84b10b628d0   1         227.7kB
fbf13f1ca07f761a88676952e4bdbc9508caffc66632e0ac2fc113ac8c5b2e4e   1         1.026MB
38175e1ceff4554c04eb405a1cd35c40b9daa370724ba20e19688851e53f61b3   1         4.274kB
07f878bddf5e1f1b8f17ac862656918cf846c1820e1f8aeee3d0280a9333d743   1         2.862GB
68a4a1cbf6d8e1d3bf404addb0ed6268eadeef002b845b1e5ca8983ad428acfd   1         18.89kB
f87e8ca756130cd7762d572668dd03748fe11267a76c15c04ee1b9565e22577e   1         2.818GB
k3d-cogfactory-cluster-images                                      4         0B
a8882c64e10f1960606740439a1117514e90e1e123669980c0f74cc547632cbf   1         13.83kB
532042b2eab12101ee276aae5f4a7175c937cd0748ef8e5f98a87830b6757d2a   1         1.197kB
3993fefc0351552e5b0cd1dae38b149688baea42daeb76edad968426a8a443ab   1         462.3kB
0666dd78224533f475cf6438062788e16926b4a2ab89f6dc491bb63d2d693c75   1         1.241kB
8de2ba003a0019a39b85f5003b3668a4def2122a529a671e8451d621f73ae32c   1         1.685kB
57f099b0f68a213f40e7e889eb3d7b88dbeea1c11701c981aedea6c2cab4bc8e   1         3.287GB
66ed0c86385a5c2454d7ea54ccb7073ba27935926ef396a1f89babe60d15ef55   1         13.82kB
06248cbe55ff4df5bb6fc38122cd82ade25c71296bfbd14f3f7cc27b17260485   1         2.699GB

Build cache usage: 0B

CACHE ID   CACHE TYPE   SIZE      CREATED   LAST USED   USAGE     SHARED

!!! VOLUMES !!!

Windows : \\wsl$\docker-desktop-data\version-pack-data\community\docker\volumes

10 Go
- 3 x 3 Go (docker volumes in each docker volume=node)
  - Yugabyte : 1.2 Go
  - Linux base : 436 Mo
  - Python / Perl ... : 190 Mo
  - K3S agent ...
- 1 x 700 Mo
  - git-core : 340 Mo
  - Helm : 115 Mo
  - coredns : 41 Mo


### Docker images

Pulling the image takes a while : **612 Mo** 

### Memory

?? Vmmem : 12 Go after install ??