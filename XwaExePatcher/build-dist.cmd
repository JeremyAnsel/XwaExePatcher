@echo off
setlocal

cd "%~dp0"

For %%a in (
"JeremyAnsel.Xwa.ExePatcher\bin\Release\*.dll"
) do (
xcopy /s /d "%%~a" dist\lib\
)

For %%a in (
"XwaExePatcherConsole\bin\Release\*.dll"
"XwaExePatcherConsole\bin\Release\*.exe"
) do (
xcopy /s /d "%%~a" dist\Console\
)

For %%a in (
"XwaExePatcherWindow\bin\Release\*.dll"
"XwaExePatcherWindow\bin\Release\*.exe"
"XwaExePatcherWindow\bin\Release\*.xml"
"XwaExePatcherWindow\bin\Release\*.xsd"
"XwaExePatcherWindow\bin\Release\*.txt"
) do (
xcopy /s /d "%%~a" dist\Window\
)
