# Run the BrowserBench.org Tests
param(
    [int] $Iterations = 10
)

[string] $RepoRoot = Resolve-Path "$PSScriptRoot\.."
$StartingLocation = Get-Location
Set-Location -Path $RepoRoot

Write-Host "Run BrowserBench.org Tests"
try
{
    if (-not (Test-Path "$RepoRoot\log")) {
        New-Item -ItemType Directory -Force -Path "$RepoRoot\log"
    }

    Write-Host "Build WebTester"
    & dotnet build -c Release src\EmbeddedWebSampleApps.WebTester
    
    for ($i = 0; $i -lt $Iterations; $i++)
    {
        Write-Host "Run BrowserBench.org Speedometer test for WV2 $(i+1)"
        & "$RepoRoot\scripts\run-speedometer.ps1" -WebHost WV2 -LogFile "$RepoRoot\log\speedometer_WV2_$i.log" -Build $False
    }

    for ($i = 0; $i -lt $Iterations; $i++)
    {
        Write-Host "Run BrowserBench.org Speedometer test for CEF $(i+1)"
        & "$RepoRoot\scripts\run-speedometer.ps1" -WebHost CEF -LogFile "$RepoRoot\log\speedometer_CEF_$i.log" -Build $False
    }

    for ($i = 0; $i -lt $Iterations; $i++)
    {
        Write-Host "Run BrowserBench.org JetStream test for WV2 $(i+1)"
        & "$RepoRoot\scripts\run-jetstream.ps1" -WebHost WV2 -LogFile "$RepoRoot\log\jetstream_WV2_$i.log" -Build $False
    }

    for ($i = 0; $i -lt $Iterations; $i++)
    {
        Write-Host "Run BrowserBench.org JetStream test for CEF $(i+1)"
        & "$RepoRoot\scripts\run-jetstream.ps1" -WebHost CEF -LogFile "$RepoRoot\log\jetstream_CEF_$i.log" -Build $False
    }
}
finally
{
    Set-Location -Path "$StartingLocation"
}
