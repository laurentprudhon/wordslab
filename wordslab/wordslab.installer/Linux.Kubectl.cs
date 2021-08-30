using System;

namespace wordslab.installer.Linux
{
    class Kubectl
    {
        // Executes : kubectl version
        // Returns  : 
        // Version object if kubectl version was correctly parsed
        // null otherwise
        public static Version CheckKubectlVersion()
        {
            try
            {
                string output;
                string error;
                int exitcode = Process.Run("kubectl", "version", 5, out output, out error);
                if (!String.IsNullOrEmpty(output))
                {
                    // GitVersion:"v1.21.2"
                    int versionIndex = output.IndexOf("GitVersion");
                    if (versionIndex > 0 && (versionIndex + 13) < output.Length)
                    {
                        versionIndex += 13;
                        int firstDot = output.IndexOf('.', versionIndex);
                        int secondDot = output.IndexOf('.', firstDot + 1);
                        int eol = output.IndexOf('"', secondDot + 1);
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

        /* https://betterprogramming.pub/k8s-tips-give-access-to-your-clusterwith-a-client-certificate-dfb3b71a76fe
          
        openssl genrsa -out cogfactory-install.key 4096
        
        vi csr.cnf
        ---
[ req ]
default_bits = 2048
prompt = no
default_md = sha256
distinguished_name = dn
[ dn ]
CN = cogfactory-install
O = cogfactory
[ v3_ext ]
authorityKeyIdentifier=keyid,issuer:always
basicConstraints=CA:FALSE
keyUsage=keyEncipherment,dataEncipherment
extendedKeyUsage=serverAuth,clientAuth
        ---

        openssl req -config ./csr.cnf -new -key cogfactory-install.key -nodes -out cogfactory-install.csr

        vi csr.yaml
        ---
apiVersion: certificates.k8s.io/v1
kind: CertificateSigningRequest
metadata:
  name: cogfactory-install-csr
spec:
  groups:
  - system:authenticated
  request: ${BASE64_CSR}
  signerName: kubernetes.io/kube-apiserver-client
  usages:
  - digital signature
  - key encipherment
  - server auth
  - client auth
        ---

        export BASE64_CSR=$(cat ./cogfactory-install.csr | base64 | tr -d '\n')

        cat csr.yaml | envsubst | kubectl apply -f -

        [kubectl get csr]

        kubectl certificate approve cogfactory-install-csr

        kubectl get csr cogfactory-install-csr -o jsonpath='{.status.certificate}' | base64 --decode > cogfactory-install.crt

        [openssl x509 -in ./cogfactory-install.crt -noout -text]

        vi role-cogfactory-db-install.yaml
        ---
kind: Role
apiVersion: rbac.authorization.k8s.io/v1
metadata:
 namespace: cogfactory-db
 name: install
rules:
- apiGroups: [""]
  resources: ["pods", "services"]
  verbs: ["create", "get", "update", "list", "delete"]
- apiGroups: ["apps"]
  resources: ["deployments"]
  verbs: ["create", "get", "update", "list", "delete"]
        ---

        kubectl apply -f role-cogfactory-db-install.yaml

        vi rolebinding-cogfactory-install.yaml
        ---
kind: RoleBinding
apiVersion: rbac.authorization.k8s.io/v1
metadata:
 name: cogfactory-install
 namespace: cogfactory-db
subjects:
- kind: User
  name: cogfactory-install
  apiGroup: rbac.authorization.k8s.io
roleRef:
 kind: Role
 name: install
 apiGroup: rbac.authorization.k8s.io
        ---

        kubectl apply -f rolebinding-cogfactory-install.yaml

        vi kubeconfig.tpl
        ---
apiVersion: v1
kind: Config
clusters:
- cluster:
    certificate-authority-data: ${CLUSTER_CA}
    server: ${CLUSTER_ENDPOINT}
  name: ${CLUSTER_NAME}
users:
- name: ${USER}
  user:
    client-certificate-data: ${CLIENT_CERTIFICATE_DATA}
contexts:
- context:
    cluster: ${CLUSTER_NAME}
    user: ${USER}
  name: ${USER}-${CLUSTER_NAME}
current-context: ${USER}-${CLUSTER_NAME}
        ---

        sudo apt install jq

        export USER="cogfactory-install"
        export CLUSTER_NAME=$(kubectl config view --minify -o jsonpath={.current-context})
        export CLIENT_CERTIFICATE_DATA=$(kubectl get csr cogfactory-install-csr -o jsonpath='{.status.certificate}')
        export CLUSTER_CA=$(kubectl config view --raw -o json | jq -r '.clusters[] | select(.name == "'$(kubectl config current-context)'") | .cluster."certificate-authority-data"')
        export CLUSTER_ENDPOINT=$(kubectl config view --raw -o json | jq -r '.clusters[] | select(.name == "'$(kubectl config current-context)'") | .cluster."server"')

        cat kubeconfig.tpl | envsubst > kubeconfig.cogfactory-install

        export KUBECONFIG=$PWD/kubeconfig.cogfactory-install

        kubectl config set-credentials cogfactory-install --client-key=$PWD/cogfactory-install.key --embed-certs=true

        [kubectl get pods ==> FORBIDDEN]
        [kubectl get pods -n cogfactory-db ==> OK]

         */
    }
}
