FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/MyLab.ConfigServer/Client/", "MyLab.ConfigServer/Client/"]
COPY ["src/MyLab.ConfigServer/Shared/", "MyLab.ConfigServer/Shared/"]

WORKDIR "/src/MyLab.ConfigServer/Client"

RUN dotnet restore "MyLab.ConfigServer.Client.csproj"
RUN dotnet publish "MyLab.ConfigServer.Client.csproj" -c Release -o /app/publish

FROM nginx:1.21
EXPOSE 80

ARG version=undefined

COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
ADD ./ci/client/nginx.conf /etc/nginx/conf.d/default.conf

