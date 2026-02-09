@echo off

cd %~dp0..

for /r /d %%a in (.) do (
	@echo off
	:: enter the directory
	pushd %%a
	cd
	if exist "bin\" rd /s /q "bin"
    if exist "obj\" rd /s /q "obj"
	@rem rd /s /q "bin" > nul 2>&1
	@rem rd /s /q "obj" > nul 2>&1
	:: leave the directory
	popd
)

@pause