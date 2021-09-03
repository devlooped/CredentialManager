![Icon](https://raw.githubusercontent.com/devlooped/CredentialManager/main/assets/images/gcm.png) Git Credential Manager Lib
============

Packages the [Microsoft.Git.CredentialManager](https://github.com/microsoft/Git-Credential-Manager-Core/tree/main/src/shared/Microsoft.Git.CredentialManager) cross-platform implementation for Windows, macOS and Linux for use as a library.

This repository provides virtually no code whatesoever, it all comes from the GCM Core. 

Release version numbers track the [GCM releases](https://github.com/microsoft/Git-Credential-Manager-Core/releases) themselves.

[![Version](https://img.shields.io/nuget/vpre/Devlooped.CredentialManager.svg?color=royalblue)](https://www.nuget.org/packages/Devlooped.CredentialManager.Css)
[![Downloads](https://img.shields.io/nuget/dt/Devlooped.CredentialManager.svg?color=green)](https://www.nuget.org/packages/Devlooped.CredentialManager.Css)
[![License](https://img.shields.io/github/license/devlooped/CredentialManager.svg?color=blue)](https://github.com/devlooped/CredentialManager/blob/main/license.txt)
[![Build](https://github.com/devlooped/CredentialManager/workflows/build/badge.svg?branch=main)](https://github.com/devlooped/CredentialManager/actions)

## Usage

The only code in this repository is a helper factory to create the credential store 
appropriate to the current platform:

```csharp
using Microsoft.Git.CredentialManager;
...

ICredentialStore store = CredentialStore.Create("myapp");
```

The namespace for the `CredentialStore` static factory class is the same as GCM itself 
for convenience: `Microsoft.Git.CredentialManager`.

The optional *namespace* argument (`myapp` above) can be used to scope credential 
operations to your own app/service.




## Sponsors

[![sponsored](https://raw.githubusercontent.com/devlooped/oss/main/assets/images/sponsors.svg)](https://github.com/sponsors/devlooped) [![clarius](https://raw.githubusercontent.com/clarius/branding/main/logo/byclarius.svg)](https://github.com/clarius)[![clarius](https://raw.githubusercontent.com/clarius/branding/main/logo/logo.svg)](https://github.com/clarius)

*[get mentioned here too](https://github.com/sponsors/devlooped)!*
