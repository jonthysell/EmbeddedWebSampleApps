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
    
    for ($i = 0; $i -lt $Iterations; $i++)
    {
        & "$RepoRoot\scripts\run-speedometer.ps1" -WebHost WV2 -LogFile "$RepoRoot\log\speedometer_WV2_$i.log"
    }

    for ($i = 0; $i -lt $Iterations; $i++)
    {
        & "$RepoRoot\scripts\run-speedometer.ps1" -WebHost CEF -LogFile "$RepoRoot\log\speedometer_CEF_$i.log"
    }
}
finally
{
    Set-Location -Path "$StartingLocation"
}
