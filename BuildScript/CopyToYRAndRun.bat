@echo off
chcp 65001
cd %~dp0
echo f|xcopy /D /e /s /y "..\DynamicPatcher\output\PatcherYRpp.dll" "D:\Games\Yuri's Revenge\DynamicPatcher\Libraries\PatcherYRpp.dll"
echo f|xcopy /D /e /s /y "..\DynamicPatcher\output\Extension.dll" "D:\Games\Yuri's Revenge\DynamicPatcher\Libraries\Extension.dll"
call CopyScriptToYR
call "D:\Games\Yuri's Revenge\TestMod.bat"

