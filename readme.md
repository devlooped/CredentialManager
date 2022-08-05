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


<!-- include docs/footer.md -->
