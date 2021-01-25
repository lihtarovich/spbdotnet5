#!/usr/local/bin/bash

find ../ -name ".DS_Store" -delete
dotnet publish --runtime linux-x64 -c Release --self-contained true -o ../spbdotnet5installer/usr/bin/spbdotnet5.service/ ../SpbDotNetCore5
dpkg-deb --build ../spbdotnet5installer/
mv ../spbdotnet5installer.deb ../spbdotnet5installer_full.deb
scp ../spbdotnet5installer_full.deb kirill@spbdotnet.local:/spbdotnet
#ssh -t kirill@spbdotnet.local 'sudo dpkg -i /spbdotnet/spbdotnet5installer.deb' 