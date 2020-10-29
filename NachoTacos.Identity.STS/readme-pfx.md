# Generate an OpenSSL certificate
1. Setup OpenSSL for Windows (if necessary)
2. Create a new pfx file
3. Change DeveloperSigningCredential to SigningCredential

## Setup OpenSSL for Windows
1. Download [OpenSSL-Win64](https://slproweb.com/products/Win32OpenSSL.html) and install.
2. Set the path to the bin\openssl.exe
3. Check OpenSSL version

At the command prompt:
```
>openssl version
```

## Create a new pfx file
This will create a key and certificate valid for 365 days, then bundles them into a pfx file.
1. Make sure you have a folder in D:\rsakeys or change the folder to your prefered path.
2. From the command prompt:
```
>openssl req -newkey rsa:2048 -nodes -keyout d:\rsakeys\nachotacos.key -x509 -days 365 -out d:\rsakeys\nachotacos.cer

```
3. Fill in the certificate details
4. Bundle the certificate and key into a single pfx file
```
>openssl pkcs12 -export -in d:\rsakeys\nachotacos.cer -inkey d:\rsakeys\nachotacos.key -out d:\rsakeys\nachotacos.pfx
```
5. Enter the password for this certificate

## Add Signin Credential to Identity Server
1. Add the certificate details in appsettings.json
```
{
  ...

  "SignInCredentials": {
    "PFXFile": "D:\\rsakeys\\nachotacos.pfx",
    "Password": "nachocheese"
  }

  ...
}
```
2. Add IConfiguration to Startup
```
public class Startup
{
  ...

  private IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
      _configuration = configuration;
  }

  ...
}
```

3. Change AddDeveloperSigningCredential to AddSigningCredential
```
public void ConfigureServices(IServiceCollection services)
{
  ...

  string pfxFilePath = _configuration.GetValue<string>("SignInCredentials:PFXFile");
  string pfxFilePass = _configuration.GetValue<string>("SignInCredentials:Password");

  services.AddIdentityServer()
          .AddSigningCredential(new X509Certificate2(pfxFilePath, pfxFilePass))
          .AddTestUsers(InMemoryConfiguration.TestUsers().ToList())
          .AddInMemoryClients(InMemoryConfiguration.Clients())
          .AddInMemoryApiResources(InMemoryConfiguration.ApiResources());

  ...
}
```