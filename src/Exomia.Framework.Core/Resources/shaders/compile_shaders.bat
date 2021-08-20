@echo off
cd /d %~dp0
@echo ----------------------------------------------------------------------
@echo ------------- compiling and optimizing shaders started! --------------
for /r %%a in (src/*) do (
	call %VULKAN_SDK%/bin/glslangValidator.exe -V src/%%~nxa -o %%~nxa.spv
	call %VULKAN_SDK%/bin/spirv-opt.exe -O %%~nxa.spv -o %%~nxa-opt.spv
)
@echo:
@echo ------------- compiling and optimizing shaders finsihed! -------------
@echo ----------------------------------------------------------------------