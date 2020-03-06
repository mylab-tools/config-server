echo off

IF [%1]==[] goto noparam

echo "Build project ..."
dotnet publish ..\src\MyLab.ConfigServer\MyLab.ConfigServer.proj -r Release -o .\out\app

echo "Build image '%1' and 'latest'..."
docker build -t ozzyext/mylab-config-server:%1 -t ozzyext/mylab-config-server:latest .

echo "Publish image '%1' ..."
docker push ozzyext/mylab-config-server:%1

echo "Publish image 'latest' ..."
docker push ozzyext/mylab-config-server:latest

goto done

:noparam
echo "Please specify image version"
goto done

:done
echo "Done!"