@echo off
cd /d %~dp0
for /r %%a in (src/*) do (
	call  %VULKAN_SDK%/bin/glslangValidator.exe -V src/%%~nxa -o %%~nxa.spv
)
@echo:
echo compiling shaders finsihed!