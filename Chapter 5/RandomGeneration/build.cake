var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");
var solution = "RandomGeneration.sln";

Task("Clean")
		.Does(() =>
	{
		CleanDirectories("./**/bin/" + configuration);
		CleanDirectories("./**/obj/" + configuration);
	});

Task("Restore")
	.Does(() =>
	{
		NuGetRestore(solution);
	});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.Does(() =>
	{
		MSBuild(solution, settings =>
			settings.SetPlatformTarget(PlatformTarget.MSIL)
				.WithTarget("Build")
				.SetConfiguration(configuration));
	});

Task("Tests")
	.IsDependentOn("Build")
	.Does(() =>
	{
		MSTest("./**/*.Tests.dll", 
			new MSTestSettings
			{
				NoIsolation = true
			});
	});

Task("NuGetPack")
	.IsDependentOn("Tests")
	.Does(() =>
	{
		CreateDirectory("./NuGet Pack");
		CopyFile("./RandomGeneration/RandomGeneration.nuspec",
			"./NuGet Pack/RandomGeneration.nuspec");
		CopyDirectory("./RandomGeneration/bin/Release",
			"./NuGet Pack");
		NuGetPack("./NuGet Pack/RandomGeneration.nuspec", 
			new NuGetPackSettings
			{
				OutputDirectory = "./NuGet Pack"
			});
	});
	
Task("Default")
	.IsDependentOn("Build")
	.IsDependentOn("Tests")
	.IsDependentOn("NuGetPack");
	
RunTarget(target);