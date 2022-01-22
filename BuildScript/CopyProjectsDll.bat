@echo off
chcp 65001
cd %~dp0
echo f|xcopy /y "..\DynamicPatcher\Projects\PatcherYRpp\bin\Debug\PatcherYRpp.dll" "..\DynamicPatcher\output\PatcherYRpp.dll"
echo f|xcopy /y "..\DynamicPatcher\Projects\PatcherYRpp.Utilities\bin\Debug\PatcherYRpp.Utilities.dll" "..\DynamicPatcher\output\PatcherYRpp.Utilities.dll"
echo f|xcopy /y "..\DynamicPatcher\Projects\Extension\bin\Debug\Extension.dll" "..\DynamicPatcher\output\Extension.dll"
echo f|xcopy /y "..\DynamicPatcher\Projects\Extension.FX\bin\Debug\Extension.FX.dll" "..\DynamicPatcher\output\Extension.FX.dll"
echo f|xcopy /y "..\DynamicPatcher\Kratos*.i*" "..\DynamicPatcher\output\Kratos*.i*"

