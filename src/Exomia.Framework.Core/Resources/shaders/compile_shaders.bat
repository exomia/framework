@echo off

cd /d %~dp0

if exist bin\%1\compiled rmdir /s/q bin\%1\compiled

mkdir bin\%1\compiled

@echo -----------------------------------------------------------------------------
@echo ------------- compiling and optimizing shaders for %1 started! --------------
for /r %%a in (src/*.frag,src/*.vert) do (
	call %VULKAN_SDK%/bin/glslangValidator.exe --target-env spirv1.6 -V src/%%~nxa -o bin/%1/compiled/%%~nxa.spv
	call %VULKAN_SDK%/bin/spirv-opt.exe -O bin/%1/compiled/%%~nxa.spv -o bin/%1/compiled/%%~nxa-opt.spv
)
@echo:
@echo ------------- compiling and optimizing shaders for %1 finished! -------------
@echo -----------------------------------------------------------------------------