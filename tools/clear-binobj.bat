@echo off

cd %~dp0..

for /r /d %%a in (.) do (
	@echo off
	:: enter the directory
	pushd %%a
	
	for %%i in (bin, obj) do (
		if exist "%%i\" (		
			echo Deleting: %%~fi
			rd /s /q "%%i"
		)
	)

	@rem rd /s /q "bin" > nul 2>&1
	@rem rd /s /q "obj" > nul 2>&1
	:: leave the directory
	popd
)

@pause