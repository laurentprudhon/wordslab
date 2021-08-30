# Infrastructure and Security

## Concepts

**Cognitive Platform** : Fully integrated set of tools designed to help *companies* build, deploy and maintain cognitive solutions at scale, using an industrialized and secure workflow.
- Companies run very rich and complex IT systems, mostly made of traditional computer programs working on structured data
- The advent of more efficient machine learning techniques gives them an opportunity to automate new tasks, by extending the reach of the IT system to unstructured data (text, images, voice) and more sophisticated predictions
- To stay competitive, companies need to introduce a new type of component in their applications : models trained by example
- The methods, workflows, and tools needed to create and maintain this new family of components are quite different from the traditional developer toolset : many new capabilities need to be built in a coordinated way across the company
- When building cognitive solutions at the scale of a company, it is also critical to be able to share and reuse a lot of building blocks between projects : for example, all projects will work on the same company language, the customers will mention the same topics on all communication channels, etc ...
- New regulations apply to "AI solutions" : in addition to personal data protection issues, auditability, robustness, and fairness need to be explicitly managed. These requirements can only be satisfied if they are embedded by design in an end to end workflow
- That's why companies need a single integrated platform to manage the full lifecycle of their cognitive services : working with custom solutions for each application is not economically viable
- The Cognitive Platform is meant to complement the existing IT system : it should be designed to integrate seamlessly in any kind of corporate environment, and build on existing capabilities 

**Clusters and Namespaces** : All elements of the self-contained Cognitive Platform run on Kubernetes clusters.
- The IT operations team of the company is responsible to provide, secure and manage Kubernetes clusters with API version >= 1.18.
- The number and types of clusters is chosen based on data isolation and resiliency requirements : the entire *Cognitive Platform* can be deployed on a single node or on many muti-zone clusters.
- Types of clusters and resiliency considered in this document : single-node (evaluation), multi-nodes (hardware failure), multi-zones (datacenter failure), multi-region (major natural disaster).
- To make it easier to evaluate the platform, a simple installer is provided to create a single node installation on a Windows or Linux machine with minimal requirements.
- In a corporate IT system, the platform will never require privileges at the cluster level : all platform components are deployed in Kubernetes namespaces.
- These Kubernetes namespaces are created by the IT operations teams, with controlled privileges limited to the scope of the namespace.
- The list of clusters, namespaces and credentials to work in these namespaces are then provided to the platform installation tools.

**Execution Environment** : Logical isolation and resiliency boundary implemented on top of the infrastructure and used to store data and execute components. 
- Each Execution Environment is mapped to either one Kubernetes namespace (single region environment) or two Kubernetes namespaces located on two distant clusters (multi regions environment).
- Execution Environments are reachable through distinct DNS subdomains so that firewall rules can restrict accesses, and distinct privileges need to be granted to access two distinct environments.
- Specific network rules restrict the data flows and service calls allowed between Execution Environments.
- A single distributed database instance is deployed in each Execution Environment : all data and files are stored in this unique database.
- The Execution Environment is the unit of backup and restore in case of a major disaster : because all data is stored in a single database, you only need to restore the database backup to restore the environment.

**Platform Installation** : Set of Execution Environments managed by a *a single IT operations team*. 
- For example : all environments managed by a transversal IT organization inside a multi-companies corporate group.
- A special and unique *Platform Installation Execution Environment* is always associated with a *Platform Installation*
- This special environment centralizes the IT monitoring information and the low level management operations for all the other environments

**Tenant Instances** : A Platform Installation is a collection of strictly separated Tenant Instances. 
- *No data should be shared* between two Tenant Instances.
- Each Tenant Instance can have *distinct scale, security and reliability requirements*.
- For example : each company inside the corporate group is a distinct tenant with its own confidential data and specific policies.
- Only the transversal IT operations team can span Tenant Instances (by definition).

**Lifecycle Environments** : Each Tenant Instance is divided in *4 isolated environments* dedicated to specific steps of the cognitive solutions lifecycle : *dev, train, uat, prod*. The role of each environment is decribed below :
- *dev* : used by IT developers to write and test traditional code for algorithms, components, services, and pipelines (for example : Python, C#).
- *train* : used by business experts to define busines concepts, annotate datasets and logs with labels, train models based the algorithms and pipelines written in the dev environment, and evaluate their performance.
- *uat* : used by future users to perform user acceptance tests on the trained solutions, by executing the end to end business process.
- *prod* : production environment with high availability and strict data isolation - no IT team member can ever see the raw data, the only environment where personal data is allowed.

**Tenant Workspaces** : A workspace groups a set of related experiences to manage a specific concern, its is hosted on a specific DNS subdomain and is dedicated to a specific family of users.
- Each Lifecycle Environment hosts a subset of the following workspaces :
- *operations* : Tenants, Environements, Upgrade, Backup, Monitor, Alert, Security (incl. secrets), Quotas [=> IT operations].
- *admin* : Teams, Roles, Builders, Subscriptions, Provisioning, Usage, Billing [=> Tenant Instance administrators].
- *project* : Projects, Issues, Tasks, Notifications, Dashboards, Tests, Audit [=> Builders cooperation]
- *train* : Datalab, Models, Experiments, Performances, Continuous Improvement, Integrations [=> Data scientists]
- *develop* : Services, Pipelines, Jobs, Sources, Versions, Tests, Package, Deploy, Dependencies [=> IT developers].
- *integrate* : Applications, UI elements, Connections, Data sources, Events, Orchestrate, Components, Plugins [=> IT integrators]
- *data* : Ingestion, Export, Convert, Index, Datalab, Explore, Logs, Stats [=> Data engineers]
- *business* : Orgs, Users, Concepts, Goals, Catalog, Annote, Stats, Dashboards, Knowledge graph [=> Business experts]
- *run* : Deployed apps and services [=> Users]
- *supervision* : Real time stats, Data exploration, Performance audit [=> Business process owners]

**Builders and Users**
- **Builders** are all the experts who collaborate to build and improve a cognitive solution
  - Builders are organized in **Teams** and *Teams of Teams*
  - A **Role** is a set of **Platform Actions** a *Team* is allowed to perform
  - *Roles* are granted to *Teams* only, not individuals, because you should almost always make sure you have a backup for all roles
  - *Roles* are assigned to *Teams* only in the context of *User Solutions*, 
- **Users** are all the clients or employees of the company who are allowed to use a cognitive solution

**Projects**
  - A **Project** is used to track the *initial creation* or a *continuous improvement phase* of a cognitive solution
  - Each *Project* has mandatory *start and end dates* and should have *Business Goals*
  - A **Business Goal** is a set of *Key Indicators* you should measure to make sure your project is a success
  - **Resources** are the artefacts used in the process of building a cognitive solution are different kind of **Resources** : data, models, code, metadata ...
  - *Teams* work on *Projects*
  - *Resources* can only be browsed, modified and deployed in the context of a *Project*
  - Access rights are granted in the context of a *Project*