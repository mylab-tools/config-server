@echo off

IF [%1]==[] goto noparam

echo "Build image '%1' and 'latest'..."
docker build --progress=plain -f .\Dockerfile --build-arg PROJ_VER=%1 -t mylabtools/config-server:%1 -t mylabtools/config-server:latest ..

echo "Publish image '%1' ..."
docker push mylabtools/config-server:%1

echo "Publish image 'latest' ..."
docker push mylabtools/config-server:latest

goto done

:noparam
echo "Please specify image version"
goto done

:done
echo "Done!"