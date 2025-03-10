# Run the BrowserBench.org MotionMark test
param(
    [string] $WebHost = "WV2",
    [string] $LogFile = "motionmark.log",
    [boolean] $Build = $true
)

[string] $RepoRoot = Resolve-Path "$PSScriptRoot\.."
$StartingLocation = Get-Location
Set-Location -Path $RepoRoot

try
{
    $BuildParam = $Build ? '' : '--no-build'
    & dotnet run -c Release --no-launch-profile $BuildParam --project src\EmbeddedWebSampleApps.WebTester -- --web-host $WebHost --log-web-console --log-performance --window-size 1200x800 --starting-uri https://browserbench.org/MotionMark1.3.1/ --post-load-js scripts\motionmark.js --log-file $LogFile
    if (!$?) {
    	throw 'Run failed!'
    }
}
finally
{
    Set-Location -Path "$StartingLocation"
}
