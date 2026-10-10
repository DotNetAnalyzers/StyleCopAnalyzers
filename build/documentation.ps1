$ErrorActionPreference = 'Stop'

Push-Location (Join-Path $PSScriptRoot '..')
try {
    $reportPath = 'docs\StyleCop.Analyzers.Status.json'
    $localizedReportPath = 'docs\StyleCop.Analyzers.Status.ru-RU.json'
    if (Test-Path $reportPath) {
        $titles = @{}
        Get-ChildItem 'StyleCop.Analyzers\StyleCop.Analyzers' -Recurse -Filter '*.ru-RU.resx' | ForEach-Object {
            [xml]$resources = Get-Content $_.FullName -Raw
            foreach ($entry in $resources.SelectNodes('/root/data')) {
                $key = $entry.GetAttribute('name')
                if ($key -match '^(S[AX]\d+S?)Title$') {
                    $titles[$Matches[1]] = $entry.value
                }
            }
        }

        $reasons = @{
            'No automatic code fix is possible for general JSON syntax errors.' = 'Автоматическое исправление произвольных синтаксических ошибок JSON невозможно.'
            'The necessary actions for this code fix are not supported by the analysis infrastructure.' = 'Инфраструктура анализа не поддерживает действия, необходимые для этого исправления.'
            'Cannot generate documentation' = 'Невозможно создать документацию'
            "Don't fix what isn't broken." = 'Не следует исправлять то, что не является ошибкой.'
            'Provided by Visual Studio' = 'Предоставляется Visual Studio'
            'Cannot generate appropriate names.' = 'Невозможно подобрать подходящие имена.'
            'No message is available for Debug.Fail' = 'Сообщение для Debug.Fail отсутствует'
            'The "Encapsulate Field" fix is provided by Visual Studio.' = 'Исправление «Инкапсулировать поле» предоставляется Visual Studio.'
        }
        $report = Get-Content $reportPath -Raw | ConvertFrom-Json
        if (!$report.diagnostics -or !$report.git.Sha) {
            throw 'The rule status report is invalid or empty.'
        }

        foreach ($diagnostic in $report.diagnostics) {
            if (!$titles.ContainsKey($diagnostic.Id)) {
                throw "Missing Russian title for $($diagnostic.Id)."
            }

            $diagnostic.Title = $titles[$diagnostic.Id]
            if ($diagnostic.NoCodeFixReason) {
                if (!$reasons.ContainsKey($diagnostic.NoCodeFixReason)) {
                    throw "Missing Russian translation for code fix reason: $($diagnostic.NoCodeFixReason)"
                }

                $diagnostic.NoCodeFixReason = $reasons[$diagnostic.NoCodeFixReason]
            }
        }

        $report | ConvertTo-Json -Depth 20 | Set-Content $localizedReportPath -Encoding utf8
    } else {
        foreach ($path in @($localizedReportPath, '_site\status\StyleCop.Analyzers.Status.ru-RU.json', '_site\ru-ru\status\StyleCop.Analyzers.Status.ru-RU.json')) {
            if (Test-Path $path) {
                Remove-Item $path
            }
        }

        Write-Warning 'No rule status report exists. Generate it as described in CONTRIBUTING.md to preview the status pages.'
    }

    dotnet docfx docfx.json --warningsAsErrors
    if ($LASTEXITCODE -ne 0) {
        throw 'The English documentation build failed.'
    }

    dotnet docfx docfx.ru-ru.json --warningsAsErrors
    if ($LASTEXITCODE -ne 0) {
        throw 'The Russian documentation build failed.'
    }
} finally {
    Pop-Location
}
