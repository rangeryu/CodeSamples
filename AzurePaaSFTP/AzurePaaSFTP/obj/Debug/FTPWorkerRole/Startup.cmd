REM   Startup.cmd calls the main startup batch file, Startup2.cmd, redirecting all output 
REM   to the StartupLog.txt log file.

REM   Log the startup date and time.
ECHO Startup.cmd: >> "%TEMP%\StartupLog.txt" 2>&1
ECHO Current date and time: >> "%TEMP%\StartupLog.txt" 2>&1
DATE /T >> "%TEMP%\StartupLog.txt" 2>&1
TIME /T >> "%TEMP%\StartupLog.txt" 2>&1
ECHO Starting up InstallFTP.cmd. >> "%TEMP%\StartupLog.txt" 2>&1

REM   Call the InstallFTP.cmd batch file, redirecting all output to the StartupLog.txt log file.
START /B /WAIT InstallFTP.cmd >> "%TEMP%\StartupLog.txt" 2>&1

REM   Log the completion of Startup.cmd.
ECHO Returned to Startup.cmd. >> "%TEMP%\StartupLog.txt" 2>&1

IF ERRORLEVEL EQU 0 (
   REM   No errors occurred. Exit Startup.cmd normally.
   EXIT /B 0
) ELSE (
   REM   Log the error.
   ECHO An error occurred. The ERRORLEVEL = %ERRORLEVEL%.  >> "%TEMP%\StartupLog.txt" 2>&1
   EXIT %ERRORLEVEL%
)