@echo off

cd /d %~dp0
if exist compiled rmdir /s/q compiled
mkdir compiled

@echo ----------------------------------------------------------------------
@echo ------------- compiling and optimizing shaders started! --------------
for /r %%a in (src/*.frag,src/*.vert) do (
	call %VULKAN_SDK%/bin/glslangValidator.exe --target-env spirv1.6 -V src/%%~nxa -o compiled/%%~nxa.spv
	call %VULKAN_SDK%/bin/spirv-opt.exe -O compiled/%%~nxa.spv -o compiled/%%~nxa-opt.spv
)
@echo:
@echo ------------- compiling and optimizing shaders finished! -------------
@echo ----------------------------------------------------------------------