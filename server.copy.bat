if exist "program\editor\binary\Plugins\ResourcesBrowser\bin\AIEditor.dll" (
xcopy /y program\editor\binary\Plugins\ResourcesBrowser\bin\AIEditor.dll %1\*.*)

if exist "program\editor\binary\Plugins\DelegateMethodEditor\bin\DelegateMethodEditor.dll" (
xcopy /y program\editor\binary\Plugins\DelegateMethodEditor\bin\DelegateMethodEditor.dll %1\*.*)

if exist "program\bin\DllWindow\ClientCommon.dll" (
xcopy /y program\bin\DllWindow\ClientCommon.dll %1\*.*)

if exist "program\editor\binary\EditorCommon.dll" (
xcopy /y program\editor\binary\EditorCommon.dll %1\*.*)

if exist "program\editor\binary\CodeGenerateSystem.dll" (
xcopy /y program\editor\binary\CodeGenerateSystem.dll %1\*.*)

if exist "program\editor\binary\ICSharpCode.AvalonEdit.dll" (
xcopy /y program\editor\binary\ICSharpCode.AvalonEdit.dll %1\*.*)

if exist "program\editor\binary\ResourceLibrary.dll" (
xcopy /y program\editor\binary\ResourceLibrary.dll %1\*.*)

if exist "program\editor\binary\EditorControlLib.dll" (
xcopy /y program\editor\binary\EditorControlLib.dll %1\*.*)

if exist "program\editor\binary\WPG.dll" (
xcopy /y program\editor\binary\WPG.dll %1\*.*)