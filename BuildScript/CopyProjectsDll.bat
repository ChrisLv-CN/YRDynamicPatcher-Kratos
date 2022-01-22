@echo off
chcp 65001
cd %~dp0
echo f|xcopy /e /s /y "..\DynamicPatcher\Projects\PatcherYRpp\bin\Debug\PatcherYRpp.dll" "..\DynamicPatcher\output\PatcherYRpp.dll"
echo f|xcopy /e /s /y "..\DynamicPatcher\Projects\PatcherYRpp.Utilities\bin\Debug\PatcherYRpp.Utilities.dll" "..\DynamicPatcher\output\PatcherYRpp.Utilities.dll"
echo f|xcopy /e /s /y "..\DynamicPatcher\Projects\Extension\bin\Debug\Extension.dll" "..\DynamicPatcher\output\Extension.dll"
echo f|xcopy /e /s /y "..\DynamicPatcher\Projects\Extension.FX\bin\Debug\Extension.FX.dll" "..\DynamicPatcher\output\Extension.FX.dll"
echo f|xcopy /e /s /y "..\DynamicPatcher\Kratos食用说明书.ini" "..\DynamicPatcher\output\Kratos食用说明书.ini"
