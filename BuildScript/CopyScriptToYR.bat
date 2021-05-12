@echo off
chcp 65001
cd %~dp0
echo d|xcopy /D /e /s /y "..\DynamicPatcher\DecoratorHooks" "D:\Games\Yuri's Revenge\DynamicPatcher\DecoratorHooks"
echo d|xcopy /D /e /s /y "..\DynamicPatcher\ExtensionHooks" "D:\Games\Yuri's Revenge\DynamicPatcher\ExtensionHooks"
echo d|xcopy /D /e /s /y "..\DynamicPatcher\Miscellaneous" "D:\Games\Yuri's Revenge\DynamicPatcher\Miscellaneous"
echo d|xcopy /D /e /s /y "..\DynamicPatcher\MyExtension" "D:\Games\Yuri's Revenge\DynamicPatcher\MyExtension"
echo d|xcopy /D /e /s /y "..\DynamicPatcher\Scripts" "D:\Games\Yuri's Revenge\DynamicPatcher\Scripts"
