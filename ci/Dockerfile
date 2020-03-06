FROM mcr.microsoft.com/dotnet/core/aspnet:2.2

ADD ./out/app /app/

ENTRYPOINT cd /app && dotnet MyAuth.OAuthPoint.dll
