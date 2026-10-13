$ErrorActionPreference = 'Stop'

Push-Location (Join-Path $PSScriptRoot '..')
try {
    $reportPath = 'docs\StyleCop.Analyzers.Status.json'
    $localizedReports = @(
        @{
            Culture = 'ru-RU'
            Locale = 'ru-ru'
            Path = 'docs\StyleCop.Analyzers.Status.ru-RU.json'
            EnglishTitleFallback = $false
            Reasons = @{
                'No automatic code fix is possible for general JSON syntax errors.' = 'Автоматическое исправление произвольных синтаксических ошибок JSON невозможно.'
                'The necessary actions for this code fix are not supported by the analysis infrastructure.' = 'Инфраструктура анализа не поддерживает действия, необходимые для этого исправления.'
                'Cannot generate documentation' = 'Невозможно создать документацию'
                "Don't fix what isn't broken." = 'Не следует исправлять то, что не является ошибкой.'
                'Provided by Visual Studio' = 'Предоставляется Visual Studio'
                'Cannot generate appropriate names.' = 'Невозможно подобрать подходящие имена.'
                'No message is available for Debug.Fail' = 'Сообщение для Debug.Fail отсутствует'
                'The "Encapsulate Field" fix is provided by Visual Studio.' = 'Исправление «Инкапсулировать поле» предоставляется Visual Studio.'
            }
        },
        @{
            Culture = 'cs-CZ'
            Locale = 'cs-cz'
            Path = 'docs\StyleCop.Analyzers.Status.cs-CZ.json'
            EnglishTitleFallback = $true
            Reasons = @{
                'No automatic code fix is possible for general JSON syntax errors.' = 'Obecné syntaktické chyby JSON nelze automaticky opravit.'
                'The necessary actions for this code fix are not supported by the analysis infrastructure.' = 'Analytická infrastruktura nepodporuje akce potřebné pro tuto opravu kódu.'
                'Cannot generate documentation' = 'Dokumentaci nelze vygenerovat.'
                "Don't fix what isn't broken." = 'Neopravujte to, co není rozbité.'
                'Provided by Visual Studio' = 'Poskytuje Visual Studio.'
                'Cannot generate appropriate names.' = 'Nelze vytvořit vhodné názvy.'
                'No message is available for Debug.Fail' = 'Pro Debug.Fail není k dispozici žádná zpráva.'
                'The "Encapsulate Field" fix is provided by Visual Studio.' = 'Opravu „Zapouzdřit pole“ poskytuje Visual Studio.'
            }
        }
    )
    if (Test-Path $reportPath) {
        foreach ($localized in $localizedReports) {
            $titles = @{}
            Get-ChildItem 'src\StyleCop.Analyzers' -Recurse -Filter "*.$($localized.Culture).resx" | ForEach-Object {
                [xml]$resources = Get-Content $_.FullName -Raw
                foreach ($entry in $resources.SelectNodes('/root/data')) {
                    $key = $entry.GetAttribute('name')
                    if ($key -match '^(S[AX]\d+S?)Title$') {
                        $titles[$Matches[1]] = $entry.value
                    }
                }
            }

            $report = Get-Content $reportPath -Raw | ConvertFrom-Json
            if (!$report.diagnostics -or !$report.git.Sha) {
                throw 'The rule status report is invalid or empty.'
            }

            foreach ($diagnostic in $report.diagnostics) {
                if ($titles.ContainsKey($diagnostic.Id)) {
                    $diagnostic.Title = $titles[$diagnostic.Id]
                } elseif (!$localized.EnglishTitleFallback) {
                    throw "Missing $($localized.Culture) title for $($diagnostic.Id)."
                }

                if ($diagnostic.NoCodeFixReason) {
                    if (!$localized.Reasons.ContainsKey($diagnostic.NoCodeFixReason)) {
                        throw "Missing $($localized.Culture) translation for code fix reason: $($diagnostic.NoCodeFixReason)"
                    }

                    $diagnostic.NoCodeFixReason = $localized.Reasons[$diagnostic.NoCodeFixReason]
                }
            }

            $report | ConvertTo-Json -Depth 20 | Set-Content $localized.Path -Encoding utf8
        }
    } else {
        foreach ($localized in $localizedReports) {
            $reportName = Split-Path $localized.Path -Leaf
            foreach ($path in @($localized.Path, "_site\status\$reportName", "_site\$($localized.Locale)\status\$reportName")) {
                if (Test-Path $path) {
                    Remove-Item $path
                }
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

    dotnet docfx docfx.cs-cz.json --warningsAsErrors
    if ($LASTEXITCODE -ne 0) {
        throw 'The Czech documentation build failed.'
    }
} finally {
    Pop-Location
}
