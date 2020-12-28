FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

ADD ./out/app /app/

ENTRYPOINT cd /app && dotnet MyLab.ConfigServer.dll
