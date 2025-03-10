# Run the BrowserBench.org Tests
param(
    [int] $TargetIterations = 10
)


function Run-Test {
    param(
        [string] $TestName,
        [string] $WebHost,
        [int] $Iterations
    )

    Write-Host "Warmup BrowserBench.org $TestName test for $WebHost"
    & "$RepoRoot\scripts\run-$TestName.ps1" -WebHost $WebHost -LogFile "$RepoRoot\log\$TestName.$WebHost.warmup.log" -Build $False | Out-Null
    for ($i = 0; $i -lt $Iterations; $i++)
    {
        Write-Host "Run BrowserBench.org $TestName test for $WebHost #$($i+1)"
        & "$RepoRoot\scripts\run-$TestName.ps1" -WebHost $WebHost -LogFile "$RepoRoot\log\$TestName.$WebHost.$i.log" -Build $False | Out-Null
        $cpu = @()
        $ram = @()
        $score = '';
        Get-Content "$RepoRoot\log\$TestName.$WebHost.$i.log" | ForEach-Object {
            if ($_ -match "Score: (\d+\.\d+)") {
                $score = [double] $matches[1]
            } elseif  ($_ -match "CPU: (\d+\.\d+)%, RAM: (\d+\.\d+) MB") {
                $cpu += [double] $matches[1]
                $ram += [double] $matches[2]
            }
        }
        $maxCpu = ($cpu | Measure-Object -Maximum).Maximum
        $maxRam = ($ram | Measure-Object -Maximum).Maximum
        Write-Host "$TestName $WebHost [$i]: CPU: $maxCpu%, RAM: $maxRam MB, Score: $score"
        "$TestName $WebHost [$i]`t$maxCpu`t$maxRam`t$score" | Add-Content "$RepoRoot\log\total.log"
    }
}

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
    & dotnet build -c Release src\EmbeddedWebSampleApps.WebTester | Out-Null

    "Test`tMax CPU`tMax RAM`tScore" | Set-Content "$RepoRoot\log\total.log"

    Run-Test -TestName "speedometer" -WebHost "WV2" -Iterations $TargetIterations
    Run-Test -TestName "speedometer" -WebHost "CEF" -Iterations $TargetIterations

    Run-Test -TestName "jetstream" -WebHost "WV2" -Iterations $TargetIterations
    Run-Test -TestName "jetstream" -WebHost "CEF" -Iterations $TargetIterations

    Run-Test -TestName "motionmark" -WebHost "WV2" -Iterations $TargetIterations
    Run-Test -TestName "motionmark" -WebHost "CEF" -Iterations $TargetIterations
}
finally
{
    Set-Location -Path "$StartingLocation"
}
