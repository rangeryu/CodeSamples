REM starting up

SET logPath="C:\Logs\StartupLog.txt"

ECHO Startup.cmd: >> %logPath% 2>&1
ECHO Current date and time: >> %logPath% 2>&1
DATE /T >> %logPath% 2>&1
TIME /T >> %logPath% 2>&1
ECHO Starting up InstallFTP.cmd. >> %logPath% 2>&1

REM   Call the InstallFTP.cmd batch file, redirecting all output to the StartupLog.txt log file.
START /B /WAIT InstallFTP.cmd >> %logPath% 2>&1

REM   Log the completion of Startup.cmd.
ECHO Returned to Startup.cmd. >> %logPath% 2>&1

IF ERRORLEVEL EQU 0 (
   REM   No errors occurred. Exit Startup.cmd normally.
   EXIT /B 0
) ELSE (
   REM   Log the error.
   ECHO An error occurred. The ERRORLEVEL = %ERRORLEVEL%.  >> %logPath% 2>&1
   EXIT %ERRORLEVEL%
)