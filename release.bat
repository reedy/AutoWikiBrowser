rem This is an attempt to replace AwbPackager with something easier, it's too buggy yet

cls
set dest=%TEMP%\$AWB.tmp
del AutoWikiBrowser.zip
@echo Created directory "%dest%"
rmdir /q /s "%dest%"
if errorlevel goto ERROR

md "%dest%"
if errorlevel goto ERROR

copy AWB\bin\Release\AutoWikiBrowser.exe "%dest%"
copy AWB\bin\Release\AutoWikiBrowser.exe.config "%dest%"
copy AWB\bin\Release\WikiFunctions.dll "%dest%"
copy AWB\bin\Release\AWBUpdater.exe "%dest%"
copy AWB\bin\Release\Diff.dll "%dest%"
copy IRCMonitor\bin\Release\IRCMonitor.exe "%dest%"


md "%dest%\Plugins"

md "%dest%\Plugins\CFD"
copy Plugins\CFD\bin\Release\CFD.dll "%dest%\Plugins\CFD"

md "%dest%\Plugins\IFD"
copy Plugins\IFD\bin\Release\IFD.dll "%dest%\Plugins\IFD"


md "%dest%\Plugins\Kingbotk"
copy "Plugins\Kingbotk\Kingbotk AWB Plugin.dll" "%dest%\Plugins\Kingbotk"
copy "Plugins\Kingbotk\Physics generic template.xml" "%dest%\Plugins\Kingbotk"
copy "Plugins\Kingbotk\Film generic template.xml" "%dest%\Plugins\Kingbotk"
copy "Plugins\Kingbotk\Readme.txt" "%dest%\Plugins\Kingbotk"

cd "%dest%"
zip -r -9 AutoWikiBrowser.zip *.*

if errorlevel 0 goto EXIT

:ERROR

@echo Archive creation failed!
@del AutoWikiBrowser.exe>nul

:EXIT