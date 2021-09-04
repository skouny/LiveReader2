@ECHO OFF
"%windir%\System32\taskkill.exe" /F /IM "LiveReader2.exe" /T
"%windir%\System32\taskkill.exe" /F /IM "CefSharp.BrowserSubprocess.exe" /T
PAUSE