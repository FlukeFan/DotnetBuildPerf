
<pre>

DotnetBuildPerf
===============

And test that compares the build/compile speed of legacy .csproj formats to the new 'dotnet.exe' tooling.

To run the test, open CommandPrompt.bat, then type:
    dotnet run

To build the example project from the command line, open CommandPrompt_Out.bat, then type:

legacy:                     clean & dotnet build Out_legacy.csproj
net461:                     clean & dotnet restore Out_net461.csproj & dotnet build Out_net461.csproj
netcoreapp2.0:              clean & dotnet restore Out_netcoreapp2.0.csproj & dotnet build Out_netcoreapp2.0.csproj
net461_consume_standard:    clean & dotnet restore Out_net461_consume_standard.csproj & dotnet build Out_net461_consume_standard.csproj

