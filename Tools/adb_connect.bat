@echo off
    set ADB="D:\Program Files\Unity\Hub\Editor\2022.3.62f2\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb"
set IP_ADDRESS="127.0.0.1"
set /p PORT=Port Number: 

%ADB% kill-server
%ADB% disconnect
%ADB% connect %IP_ADDRESS%:%PORT%
%ADB% start-server
