if exist "program\editor\binary\" (
xcopy /y program\editor\binary\*.dll %1\*.*
if exist "%1\Plugins\" (echo "") else (md %1\Plugins\)
xcopy /y /s program\editor\binary\Plugins\*.* %1\Plugins\*.*
)