@echo off

cd %~dp0..

for /r %%a in (.) do (
	@echo off
	:: enter the directory
	pushd %%a
	cd
	rd /s /q "bin" > nul 2>&1
	rd /s /q "obj" > nul 2>&1
	:: leave the directory
	popd
)

@pause