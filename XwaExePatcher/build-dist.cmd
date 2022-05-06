@echo off
setlocal

cd "%~dp0"

For %%a in (
"JeremyAnsel.Xwa.ExePatcher\bin\Release\net48\*.dll"
) do (
xcopy /s /d "%%~a" dist\lib\
)

For %%a in (
"XwaExePatcherConsole\bin\Release\net48\*.dll"
"XwaExePatcherConsole\bin\Release\net48\*.exe"
"XwaExePatcherConsole\bin\Release\net48\*.config"
) do (
xcopy /s /d "%%~a" dist\Console\
)

For %%a in (
"XwaExePatcherWindow\bin\Release\net48\*.dll"
"XwaExePatcherWindow\bin\Release\net48\*.exe"
"XwaExePatcherWindow\bin\Release\net48\*.config"
"XwaExePatcherWindow\bin\Release\net48\*.xml"
"XwaExePatcherWindow\bin\Release\net48\*.xsd"
"XwaExePatcherWindow\bin\Release\net48\*.txt"
) do (
xcopy /s /d "%%~a" dist\Window\
)
