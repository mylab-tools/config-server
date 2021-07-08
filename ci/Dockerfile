FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

ARG PROJ_VER=1.0.0

RUN echo $PROJ_VER

WORKDIR /src

COPY ["./src/MyLab.ConfigServer/Server/", "MyLab.ConfigServer/Server/"]
COPY ["./src/MyLab.ConfigServer/Shared/", "MyLab.ConfigServer/Shared/"]
COPY ["./src/MyLab.ConfigServer/Client/", "MyLab.ConfigServer/Client/"]

WORKDIR "/src/MyLab.ConfigServer/Server"

RUN dotnet restore "MyLab.ConfigServer.Server.csproj"
RUN dotnet publish "MyLab.ConfigServer.Server.csproj" -c Release -o /app/publish -p:Version=$PROJ_VER

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyLab.ConfigServer.Server.dll"]

