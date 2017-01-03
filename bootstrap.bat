@ECHO OFF

SET "HERE=%~dp0"

CALL POWERSHELL -ExecutionPolicy ByPass -NoProfile -File "%HERE%Scripts\GatherGameFiles.ps1" -ProjectFolder "%HERE%"
if not errorlevel 0 (
	ECHO.
	ECHO.
	ECHO ###################################################
	ECHO There has been an error gathering game files!
	ECHO ###################################################
)