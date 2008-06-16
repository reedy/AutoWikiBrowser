rem ***********************************************************************
rem STILL DOESN'T WORK
rem You need the free Info-Zip archiver to use this script
rem http://www.info-zip.org/
rem ***********************************************************************

@echo off
cls
set curdir=%cd%
echo %cd%
set dest=%TEMP%\$AWB.tmp

if exist AutoWikiBrowser.zip del AutoWikiBrowser.zip
if exist "%dest%" rmdir /q /s "%dest%"
md "%dest%"

if errorlevel goto ERROR

copy AWB\bin\Release\AutoWikiBrowser.exe "%dest%"
if errorlevel goto ERROR
copy AWB\bin\Release\AutoWikiBrowser.exe.config "%dest%"
if errorlevel goto ERROR
copy AWB\bin\Release\WikiFunctions.dll "%dest%"
if errorlevel goto ERROR
copy AWB\bin\Release\AWBUpdater.exe "%dest%"
if errorlevel goto ERROR
copy AWB\bin\Release\Diff.dll "%dest%"
if errorlevel goto ERROR
copy IRCMonitor\bin\Release\IRCMonitor.exe "%dest%"
if errorlevel goto ERROR


md "%dest%\Plugins"

md "%dest%\Plugins\CFD"
copy Plugins\CFD\bin\Release\CFD.dll "%dest%\Plugins\CFD"
if errorlevel goto ERROR

md "%dest%\Plugins\IFD"
copy Plugins\IFD\bin\Release\IFD.dll "%dest%\Plugins\IFD"
if errorlevel goto ERROR


md "%dest%\Plugins\Kingbotk"
copy "Plugins\Kingbotk\Kingbotk AWB Plugin.dll" "%dest%\Plugins\Kingbotk"
if errorlevel goto ERROR

copy "Plugins\Kingbotk\Physics generic template.xml" "%dest%\Plugins\Kingbotk"
if errorlevel goto ERROR

copy "Plugins\Kingbotk\Film generic template.xml" "%dest%\Plugins\Kingbotk"
if errorlevel goto ERROR

copy "Plugins\Kingbotk\Readme.txt" "%dest%\Plugins\Kingbotk"
if errorlevel goto ERROR

cd "%dest%"

zip -r -9 AutoWikiBrowser.zip *.*
if errorlevel goto ERROR

cd "%curdir%"
move "%dest%\AutoWikiBrowser.zip" .\
if errorlevel goto ERROR


rmdir /q /s "%dest%"
if errorlevel = 0 goto EXIT

:ERROR

@echo Archive creation failed!
@del AutoWikiBrowser.exe>nul

:EXIT