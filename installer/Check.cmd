@echo off
powershell.exe -STA -NoProfile -ExecutionPolicy Bypass -File "%~dp0Manage.ps1" -Mode Check
if "%errorlevel%"=="2" (echo Cancelled. No game files changed.) else if errorlevel 1 (echo FAILED. Read the error above.) else (echo Done.)
pause
