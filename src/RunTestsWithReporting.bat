SET playlist=All.playlist

SET runner-path=packages\NUnit.ConsoleRunner.3.9.0\tools\
SET runner-tool=nunit3-console.exe
SET project=./../../../SFA.DAS.Forecasting.sln
SET specflow-path=packages\SpecFlow.2.4.0\tools\
SET specflow-tool=specflow.exe
SET test-proj=./../../../SFA.DAS.Forecasting.Web.AcceptanceTests/SFA.DAS.Forecasting.Web.AcceptanceTests.csproj

CSCRIPT playlist_converter.vbs %playlist%

START /B /d %runner-path% /WAIT %runner-tool% --labels=All "--result=TestResult.xml;format=nunit3" --testlist=./../../../testlist %project%
MOVE /y %runner-path%TestResult.xml %specflow-path%TestResult.xml
START %specflow-path%%specflow-tool% nunitexecutionreport --ProjectFile "C:\newSrc\das-forecasting-tool\src\SFA.DAS.Forecasting.Web.AcceptanceTests\SFA.DAS.Forecasting.Web.AcceptanceTests.csproj" --xmlTestResult TestResult.xml --testOutput TestOutput.txt --OutputFile TestResult.html
MOVE /y %specflow-path%TestResult.html .\TestResult.html
MOVE /y %specflow-path%TestResult.xml .\TestResult.xml
@echo off
pause