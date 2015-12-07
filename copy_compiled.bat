@echo off
setlocal

rem Globals
set baseDir=Compiled
set platform=%1
set arch=AnyCPU

rem Argument check
set argC=0
for %%x in (%*) do Set /A argC+=1

if %argC% NEQ 1 (
  if %argC% NEQ 2 goto argfail
  set arch=%2%
)

rem Directories
if not exist %baseDir% mkdir %baseDir%
set outputDir=%baseDir%\%platform%-%arch%
if not exist %outputDir% mkdir %outputDir%

rem Platform specific files

if "%platform%"=="WindowsPhone81" (
  set monosrc=MonoGame\MonoGame.Framework\bin\WindowsPhone81\AnyCPU\Release
  copy %monosrc%\SharpDX.dll %outputDir%\
  copy %monosrc%\SharpDX.xml %outputDir%\

  mkdir %outputDir%\MonoGame.Framework\Themes
  copy %monosrc%\MonoGame.Framework.xr.xml %outputDir%\MonoGame.Framework\
  copy %monosrc%\Themes\generic.xbf %outputDir%\MonoGame.Framework\Themes\
)

rem Common files
copy Jypeli\bin\%platform%\%arch%\Release\*.dll %outputDir%\
copy Jypeli\bin\%platform%\%arch%\Release\*.xml %outputDir%\
copy Jypeli\bin\%platform%\%arch%\Release\*.config %outputDir%\
copy SimplePhysics\bin\%platform%\%arch%\Release\* %outputDir%\
copy Physics2d\bin\%platform%\%arch%\Release\* %outputDir%\
goto end

:argfail
echo Syntax: %0% (platform) [build architecture]
goto error

:error
endlocal
exit /B 1

:end
endlocal
