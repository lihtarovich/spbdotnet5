#!/usr/local/bin/bash

find ../ -name ".DS_Store" -delete
dotnet publish --runtime linux-x64 -c Release -p:PublishTrimmed=True --self-contained true -o ../spbdotnet5installer/usr/bin/spbdotnet5.service/ ../SpbDotNetCore5
dpkg-deb --build ../spbdotnet5installer/
mv ../spbdotnet5installer.deb ../spbdotnet5installer_trimmed.deb
scp ../spbdotnet5installer_trimmed.deb kirill@spbdotnet.local:/spbdotnet
#ssh -t kirill@spbdotnet.local 'sudo dpkg -i /spbdotnet/spbdotnet5installer.deb' 