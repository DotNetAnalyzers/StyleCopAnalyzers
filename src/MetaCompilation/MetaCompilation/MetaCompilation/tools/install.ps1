param($installPath, $toolsPath, $package, $project)

$analyzersPath = join-path $toolsPath "analyzers"

# Install the language agnostic analyzers.
foreach ($analyzerFilePath in Get-ChildItem $analyzersPath -Filter *.dll)
{
    if($project.Object.AnalyzerReferences)
    {
        $project.Object.AnalyzerReferences.Add($analyzerFilePath.FullName)
    }
}

# Install language specific analyzers.
# $project.Type gives the language name like (C# or VB.NET)
$languageAnalyzersPath = join-path $analyzersPath $project.Type

foreach ($analyzerFilePath in Get-ChildItem $languageAnalyzersPath -Filter *.dll)
{
    if($project.Object.AnalyzerReferences)
    {
        $project.Object.AnalyzerReferences.Add($analyzerFilePath.FullName)
    }
}
