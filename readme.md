# CredentialManager

Packages the [Microsoft.Git.CredentialManager](https://github.com/microsoft/Git-Credential-Manager-Core/tree/main/src/shared/Microsoft.Git.CredentialManager) cross-platform implementation for Windows, macOS and Linux for use as a library.

This repository provides virtually no code whatesoever, it all comes from the GCM Core. 

Release version numbers track the [GCM releases](https://github.com/microsoft/Git-Credential-Manager-Core/releases) themselves.

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