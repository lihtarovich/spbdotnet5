#!/usr/local/bin/bash

ssh -t kirill@spbdotnet.local 'mkdir -p /spbdotnet/docker/net5/'
scp ../Dockerfile kirill@spbdotnet.local:/spbdotnet/docker/net5/


