![Icon](https://raw.githubusercontent.com/devlooped/CredentialManager/main/assets/images/gcm.png) Git Credential Manager Lib
============

Packages the [GitCredentialManager](https://github.com/GitCredentialManager/git-credential-manager) cross-platform implementation for 
Windows, macOS and Linux for use as a library.

This repository provides virtually no code whatesoever, it all comes from the GCM Core. 

Release version numbers track the [GCM releases](https://github.com/GitCredentialManager/git-credential-manager/releases) themselves.

[![Version](https://img.shields.io/nuget/vpre/Devlooped.CredentialManager.svg?color=royalblue)](https://www.nuget.org/packages/Devlooped.CredentialManager.Css)
[![Downloads](https://img.shields.io/nuget/dt/Devlooped.CredentialManager.svg?color=green)](https://www.nuget.org/packages/Devlooped.CredentialManager.Css)
[![License](https://img.shields.io/github/license/devlooped/CredentialManager.svg?color=blue)](https://github.com/devlooped/CredentialManager/blob/main/license.txt)
[![Build](https://github.com/devlooped/CredentialManager/workflows/build/badge.svg?branch=main)](https://github.com/devlooped/CredentialManager/actions)

## Usage

The only code in this repository is a helper factory to create the credential store 
appropriate to the current platform:

```csharp
using GitCredentialManager;
...

ICredentialStore store = CredentialManager.Create("myapp");
```

The namespace for the `CredentialManager` static factory class is the same as GCM itself 
for convenience: `GitCredentialManager`.

The optional *namespace* argument (`myapp` above) can be used to scope credential 
operations to your own app/service.


<!-- include https://github.com/devlooped/sponsors/raw/main/footer.md -->
# Sponsors 

<!-- sponsors.md -->
[![Clarius Org](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/clarius.png "Clarius Org")](https://github.com/clarius)
[![Christian Findlay](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/MelbourneDeveloper.png "Christian Findlay")](https://github.com/MelbourneDeveloper)
[![C. Augusto Proiete](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/augustoproiete.png "C. Augusto Proiete")](https://github.com/augustoproiete)
[![Kirill Osenkov](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/KirillOsenkov.png "Kirill Osenkov")](https://github.com/KirillOsenkov)
[![MFB Technologies, Inc.](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/MFB-Technologies-Inc.png "MFB Technologies, Inc.")](https://github.com/MFB-Technologies-Inc)
[![SandRock](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/sandrock.png "SandRock")](https://github.com/sandrock)
[![Andy Gocke](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/agocke.png "Andy Gocke")](https://github.com/agocke)
[![Shahzad Huq](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/shahzadhuq.png "Shahzad Huq")](https://github.com/shahzadhuq)


<!-- sponsors.md -->

[![Sponsor this project](https://raw.githubusercontent.com/devlooped/sponsors/main/sponsor.png "Sponsor this project")](https://github.com/sponsors/devlooped)
&nbsp;

[Learn more about GitHub Sponsors](https://github.com/sponsors)

<!-- https://github.com/devlooped/sponsors/raw/main/footer.md -->
