@ECHO OFF
:: PATHS
SET WINSCP=C:\Program Files (x86)\WinSCP\WinSCP.exe
:: EXECUTE
"%WINSCP%" /console /script=publish-setup.winscp
