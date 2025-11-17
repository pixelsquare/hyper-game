@echo off
set ADB="D:\Program Files\Unity\Hub\Editor\2022.3.62f2\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb"

%ADB% logcat -c
%ADB% logcat 