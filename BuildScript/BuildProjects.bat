@echo off
chcp 65001
cd %~dp0
dotnet clean ..\DynamicPatcher\Projects\Projects.sln
dotnet build ..\DynamicPatcher\Projects\Projects.sln
call CopyProjectsDll
call CopyToYRAndRun