@echo off
setlocal enabledelayedexpansion

set AppSource=%1

rem === 現在のバッチファイルのディレクトリを取得 ===
set CurrentDir=%~dp0
set CurrentDir=%CurrentDir:~0,-1%

rem === コピー ===
xcopy %AppSource% %CurrentDir% /E /I /H /Y >nul

echo アップデート完了