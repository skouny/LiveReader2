@ECHO OFF
:: PATHS
SET OneDrivePath=C:\Users\Yiannis\OneDrive
SET RAR="C:\Program Files\WinRAR\Rar.exe"
:: VARIABLES
SET AppName=LiveReader2
SET RarPass=$k0un3157
:: CALC BACKUP FILENAME
For /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set MyDate=%%c-%%b-%%a)
For /f "tokens=1-2 delims=/:" %%a in ("%TIME%") do (set MyTime=%%a%%b)
SET RarFileName=%AppName%_%MyDate%_%MyTime%.rar
:: TAKE DATABASE BACKUP
::CALL DATABASE_BACKUP.cmd
:: MAKE ARCHIVE
%RAR% a -s -r -hp%RarPass% "..\%RarFileName%" .\*
:: COPY BACKUP TO CLOUD
COPY /Y "..\%RarFileName%" "%OneDrivePath%\SourceCode\%RarFileName%"
PAUSE