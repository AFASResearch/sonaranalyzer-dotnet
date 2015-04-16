﻿param($installPath, $toolsPath, $package, $project)

$analyzersPath = join-path $toolsPath "analyzers"
$analyzerFilePath = join-path $analyzersPath "SonarQube.CSharp.CodeAnalysis.dll"
$project.Object.AnalyzerReferences.Add($analyzerFilePath)