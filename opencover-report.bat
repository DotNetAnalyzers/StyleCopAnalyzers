@set packages_folder=.\packages
@set opencover_console=%packages_folder%\OpenCover.4.6.247-rc\tools\OpenCover.Console.exe
@set xunit_runner_console=%packages_folder%\xunit.runner.console.2.1.0\tools\xunit.console.x86.exe
@set report_generator=%packages_folder%\ReportGenerator.2.3.5.0\tools\ReportGenerator.exe
@set report_folder=.\OpenCover.Reports
@set target_dll=.\StyleCop.Analyzers\StyleCop.Analyzers.Test\bin\Debug\StyleCop.Analyzers.Test.dll
@if not exist %target_dll% goto error_target_dll
@if not exist %opencover_console% goto error_opencover_console
@if not exist %xunit_runner_console% goto error_xunit_runner_console
@if not exist %report_generator% goto error_report_generator

@if exist %report_folder%\. (del /Q %report_folder%\*) else (md %report_folder%)
@%opencover_console% ^
-register:user ^
-threshold:1 ^
-returntargetcode ^
-hideskipped:All ^
-filter:"+[StyleCop*]*" ^
-excludebyattribute:*.ExcludeFromCodeCoverage* ^
-output:"%report_folder%\OpenCover.StyleCopAnalyzers.xml" ^
-target:"%xunit_runner_console%" ^
-targetargs:"%target_dll% -noshadow"

@%report_generator% -targetdir:%report_folder% -reports:%report_folder%\OpenCover.*.xml

@goto end

:error_target_dll
@echo target dll not found (build target?)
@echo %target_dll%
@goto end

:error_opencover_console
@echo OpenCover Console not found (nuget restore?)
@echo %opencover_console%
@goto end

:error_xunit_runner_console
@echo OpenCover Console not found (nuget restore?)
@echo %xunit_runner_console%
@goto end

:error_report_generator
@echo Report Generator not found (nuget restore?)
@echo %report_generator%
@goto end

:end
