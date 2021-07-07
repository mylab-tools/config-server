@echo off

IF [%1]==[] goto noparam

rem echo "Build project ..."
rem dotnet publish ..\src\MyLab.ConfigServer\Client\MyLab.ConfigServer.Client.csproj -c Release -o .\out\app

echo "Build image '%1' and 'latest'..."
docker build --progress=plain -f .\Dockerfile -t mylabtools/config-server-ui:%1 -t mylabtools/config-server-ui:latest ..\..

echo "Publish image '%1' ..."
rem docker push mylabtools/config-server-ui:%1

echo "Publish image 'latest' ..."
rem docker push mylabtools/config-server-ui:latest

goto done

:noparam
echo "Please specify image version"
goto done

:done
echo "Done!"