Task Default -depends SourceIndex

Task Build {
  Exec { msbuild /m:4 /property:Configuration=Release Logos.Git.sln }
}

Task SourceIndex -depends Build {
  $headSha = & "C:\Program Files (x86)\Git\bin\git.exe" rev-parse HEAD
  Exec { tools\SourceIndex\github-sourceindexer.ps1 -symbolsFolder src\Logos.Git\bin\Release -userId LogosBible -repository Logos.Git -branch $headSha -sourcesRoot ${pwd} -dbgToolsPath "C:\Program Files (x86)\Windows Kits\8.0\Debuggers\x86" -gitHubUrl "https://raw.github.com" -serverIsRaw -verbose -ignore libgit2sharp, "c:\program files", "C:\Users", "f:\binaries", "f:\dd" }
}

Task NuGetPack -depends SourceIndex {
  Exec { nuget pack src\Logos.Git\Logos.Git.csproj -Prop Configuration=Release -Symbols }
}
