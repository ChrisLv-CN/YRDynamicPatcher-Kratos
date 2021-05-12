@echo off
chcp 65001
cd %~dp0
echo f|xcopy /e /s /y "..\DynamicPatcher\Projects\YRpp\bin\Debug\PatcherYRpp.dll" "..\DynamicPatcher\output\PatcherYRpp.dll"
echo f|xcopy /e /s /y "..\DynamicPatcher\Projects\Extension\bin\Debug\Extension.dll" "..\DynamicPatcher\output\Extension.dll"
