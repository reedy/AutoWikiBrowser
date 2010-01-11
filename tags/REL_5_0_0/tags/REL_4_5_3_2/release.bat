echo ***********************************************************************
echo You need the free Info-Zip archiver to use this script
echo http://www.info-zip.org/
echo ***********************************************************************

@echo off
set curdir=%cd%
echo %cd%
set dest=%TEMP%\$AWB.tmp

if exist AutoWikiBrowser.zip del AutoWikiBrowser.zip
if exist "%dest%" rmdir /q /s "%dest%"
md "%dest%"
if errorlevel 1 goto ERROR

copy AWB\bin\Release\AutoWikiBrowser.exe "%dest%"
if errorlevel 1 goto ERROR

copy AWB\bin\Release\AutoWikiBrowser.exe.config "%dest%"
if errorlevel 1 goto ERROR

copy AWB\bin\Release\WikiFunctions.dll "%dest%"
if errorlevel 1 goto ERROR

copy AWB\bin\Release\AWBUpdater.exe "%dest%"
if errorlevel 1 goto ERROR

copy AWB\bin\Release\Diff.dll "%dest%"
if errorlevel 1 goto ERROR

md "%dest%\Plugins"

md "%dest%\Plugins\CFD"
copy Plugins\CFD\bin\Release\CFD.dll "%dest%\Plugins\CFD"
if errorlevel 1 goto ERROR

md "%dest%\Plugins\IFD"
copy Plugins\IFD\bin\Release\IFD.dll "%dest%\Plugins\IFD"
if errorlevel 1 goto ERROR

md "%dest%\Plugins\Kingbotk"
copy "Plugins\Kingbotk\AWB Plugin\bin\Release\Kingbotk AWB Plugin.dll" "%dest%\Plugins\Kingbotk"
if errorlevel 1 goto ERROR

copy "Plugins\Kingbotk\Physics generic template.xml" "%dest%\Plugins\Kingbotk"
if errorlevel 1 goto ERROR

copy "Plugins\Kingbotk\Film generic template.xml" "%dest%\Plugins\Kingbotk"
if errorlevel 1 goto ERROR

echo Kingbotk readme
copy "Plugins\Kingbotk\Readme.txt" "%dest%\Plugins\Kingbotk"
if errorlevel 1 goto ERROR

cd "%dest%"

zip -r -9 AutoWikiBrowser.zip *.*
if errorlevel 1 goto ERROR

cd "%curdir%"
move "%dest%\AutoWikiBrowser.zip" .\
if not errorlevel 1 goto SUCCESS


:ERROR

@echo Archive creation failed!
@del AutoWikiBrowser.zip>nul
goto CLEANUP

:SUCCESS
@echo AWB archive successfully created

:CLEANUP

rmdir /q /s "%dest%"


