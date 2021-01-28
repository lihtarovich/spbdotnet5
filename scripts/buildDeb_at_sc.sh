#!/usr/local/bin/bash

rm -rf ../spbdotnet3installer/usr/
dotnet publish --runtime linux-x64 -c Release -p:PublishTrimmed=True --self-contained true -o ../spbdotnet5installer/usr/bin/spbdotnet5.service/ ../SpbDotNetCore5
find ../ -name ".DS_Store" -delete
dpkg-deb --build ../spbdotnet5installer/
mv ../spbdotnet5installer.deb ../spbdotnet5_at_sc.deb
scp ../spbdotnet5_at_sc.deb kirill@spbdotnet.local:/spbdotnet
#ssh -t kirill@spbdotnet.local 'sudo dpkg -i /spbdotnet/spbdotnet5installer.deb' 