if "%1" == "Debug" call windows.debug.x86.copy.bat %2

if "%1" == "Release" call windows.release.x86.copy.bat %2

xcopy /y program\bin\win32\release\*.dll %2\*.*