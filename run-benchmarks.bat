@echo off
rem ---------------------------------------------------------------------------
rem  Runs the FastEnum benchmarks pinned to the performance cores and prints
rem  the result tables only.
rem
rem  Everything dotnet build and BenchmarkDotNet print - package restore, the
rem  generated exe per job, per-iteration progress - goes to the log file. The
rem  console gets only the markdown tables BenchmarkDotNet exports as
rem  *-report-github.md.
rem
rem  Arguments are passed to BenchmarkDotNet as they are; with none it runs
rem  every benchmark (--filter *).
rem
rem    run-benchmarks.bat                              - all benchmarks
rem    run-benchmarks.bat --filter *NonConstantInput*  - one class
rem
rem  On a hybrid CPU the run is pinned to the performance cores - see the
rem  affinity block below and eng\get-pcore-affinity.ps1.
rem
rem  ASCII only on purpose: cmd.exe reads batch files in the OEM codepage, so
rem  non-ASCII comments here would be mangled into unparsable commands.
rem ---------------------------------------------------------------------------

setlocal

cd /d "%~dp0"

set "PROJECT=src\insights\FastEnum.Benchmarks\FastEnum.Benchmarks.csproj"
set "HOSTEXE=src\artifacts\bin\FastEnum.Benchmarks\release_net11.0\FastEnum.Benchmarks.exe"
set "ARTIFACTS=%CD%\BenchmarkDotNet.Artifacts"
set "LOG=%CD%\benchmarks.log"

set "BENCHARGS=%*"
if not defined BENCHARGS set "BENCHARGS=--filter *"

rem Remembered now, restored after the reports are printed - see :restorecp.
rem Split on the colon only and trim afterwards: chcp's message is localized and
rem its wording contains spaces, so a space delimiter would pick up a word.
for /f "tokens=2 delims=:" %%C in ('chcp') do for /f "tokens=*" %%D in ("%%C") do set "OLDCP=%%D"

rem Report file names are derived from class names, so a stale report left by a
rem renamed benchmark would be printed below as if it came from this run.
if exist "%ARTIFACTS%" rd /s /q "%ARTIFACTS%"
if exist "%LOG%" del /q "%LOG%"

rem A hybrid CPU (Intel P/E cores, Snapdragon X, Zen 5c) lets the scheduler move
rem a benchmark from a P core to an E core in the middle of an iteration, and that
rem is a several-fold difference, not noise within StdDev - BenchmarkDotNet only
rem reports it as a spread, and two runs of the same code disagree by 10-15%.
rem eng\get-pcore-affinity.ps1 asks Windows for the efficiency class of every
rem physical core and prints the mask of the fastest ones; on a CPU whose cores
rem are all equal it prints nothing and the run stays unrestricted. Its own
rem diagnostics go to the log.
rem
rem Set FASTENUM_BENCH_AFFINITY to a hex mask to pin the run by hand, or to
rem "off" to run on every core.
set "AFFINITY="
if defined FASTENUM_BENCH_AFFINITY (
    if /i not "%FASTENUM_BENCH_AFFINITY%"=="off" set "AFFINITY=%FASTENUM_BENCH_AFFINITY%"
) else (
    for /f "usebackq tokens=1" %%M in (`powershell -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0eng\get-pcore-affinity.ps1" 2^>^>"%LOG%"`) do set "AFFINITY=%%M"
)

rem Whatever powershell.exe writes to stdout ends up here, and that is not always
rem the mask: a missing script file, for one, makes it print its logo instead. A
rem non-mask would be handed to start below, which would refuse to launch anything.
if defined AFFINITY call :checkmask

if defined AFFINITY (
    echo Affinity: 0x%AFFINITY% - the run is pinned to these CPUs.
) else (
    echo Affinity: not restricted.
)

echo [1/2] Building Release: %PROJECT% ...
dotnet build -c Release -f net11.0 "%PROJECT%" --nologo -v quiet >> "%LOG%" 2>&1
if errorlevel 1 goto :failed

rem The mask goes onto the host process rather than into BenchmarkDotNet's own
rem --affinity, for two reasons. Windows hands a process affinity down to every
rem child it starts, so the one mask covers the per-job exe BenchmarkDotNet
rem generates, builds and runs - and it is in place BEFORE the runtime of those
rem children starts, so the GC sizes its heaps for the cores they will actually
rem get. --affinity is applied to an already running process and is parsed as a
rem signed 32-bit number, so it cannot even express cores above CPU 30.
rem
rem Priority goes on for the same reason and by the same mechanism. Above normal
rem rather than high on purpose - high starves the shell that has to collect the
rem output, and the point is to lose fewer timeslices to background work.
rem
rem /b keeps the child in this console, so the redirection below still reaches it,
rem and /wait is what makes errorlevel below mean the benchmark run.
if defined AFFINITY (
    echo [2/2] Running benchmarks on the performance cores, above normal priority ...
    start "FastEnum benchmarks" /affinity %AFFINITY% /abovenormal /b /wait "%HOSTEXE%" %BENCHARGS% >> "%LOG%" 2>&1
) else (
    echo [2/2] Running benchmarks, above normal priority ...
    start "FastEnum benchmarks" /abovenormal /b /wait "%HOSTEXE%" %BENCHARGS% >> "%LOG%" 2>&1
)
if errorlevel 1 goto :failed

if not exist "%ARTIFACTS%\results\*-report-github.md" goto :noresults

rem Not "type": the reports are UTF-8 and contain the micro sign, which the OEM
rem codepage turns into mojibake. They are also HTML-escaped by BenchmarkDotNet's
rem github exporter. PowerShell reads them as UTF-8 and unescapes; chcp makes the
rem console show it.
chcp 65001 > nul
powershell -NoProfile -Command "[Console]::OutputEncoding = [Text.Encoding]::UTF8; Get-ChildItem '%ARTIFACTS%\results' -Filter '*-report-github.md' | ForEach-Object { Write-Output ''; Write-Output ('=' * 79); Write-Output (' ' + $_.BaseName); Write-Output ('=' * 79); Write-Output ([System.Net.WebUtility]::HtmlDecode((Get-Content -Raw -Encoding UTF8 $_.FullName))) }"
call :restorecp

echo Full log: %LOG%
endlocal
exit /b 0

rem The console codepage is process-wide and survives this script, so put back
rem whatever the user had before.
:restorecp
if defined OLDCP chcp %OLDCP% > nul
goto :eof

rem No space before the pipe: it would be echoed as part of the mask.
:checkmask
if /i "%AFFINITY:~0,2%"=="0x" set "AFFINITY=%AFFINITY:~2%"
echo %AFFINITY%| findstr /r /i /c:"^[0-9a-f][0-9a-f]*$" > nul
if errorlevel 1 (
    echo Affinity: "%AFFINITY%" is not a hex mask, ignoring it.
    set "AFFINITY="
)
goto :eof

:noresults
echo.
echo No report files were produced. Full log: %LOG%
endlocal
exit /b 1

:failed
echo.
echo BUILD OR RUN FAILED. Log tail:
echo -------------------------------------------------------------------------------
powershell -NoProfile -Command "Get-Content -Tail 40 '%LOG%'"
echo -------------------------------------------------------------------------------
echo Full log: %LOG%
endlocal
exit /b 1
