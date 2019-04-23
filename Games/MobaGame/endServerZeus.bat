taskkill /f /t /im Zeus.exe
ping -n 2 127.0.0.1 > nul
taskkill /f /t /im RegServer.exe
taskkill /f /t /im DataServer.exe
taskkill /f /t /im HallServer.exe
taskkill /f /t /im GateServer.exe
taskkill /f /t /im PathFindServer.exe
taskkill /f /t /im ComServer.exe
taskkill /f /t /im LogServer.exe