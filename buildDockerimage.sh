#!/bin/bash

docker build --no-cache -t spbdotnet5:0.0.1 --build-arg dbhost=$(ip -4 addr show docker0 | grep -Po 'inet \K[\d.]+') .
docker run -d --name spbdotnet5 -p 5005:5005 -e ASPNETCORE_URLS="https://+" -e ASPNETCORE_HTTPS_PORT=5005 -e ASPNETCORE_Kestrel__Certificates__Default__Password="123QWEasd" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/usr/bin/spbdotnet5/spbdotnet.pfx  spbdotnet5:0.0.1 