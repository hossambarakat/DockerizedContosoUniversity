#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#addin "Cake.Docker"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var dockerhubUserName = Argument("dockerhubusername","");
var dockerhubPassword = Argument("dockerhubpassword","");

var solutionFile = "./ContosoUniversity.sln"; 

Task("Clean")
    .Does(() =>
{
    CleanDirectory("./release-package");
});

Task("Restore-Nuget-Packages")
    .IsDependentOn("Clean")
    .Does(()=>{
        NuGetRestore(solutionFile);
    });


Task("Build-Solution")
    .IsDependentOn("Restore-Nuget-Packages")
    .Does(() =>
    {
         MSBuild(solutionFile, settings => settings.SetConfiguration(configuration)
                                                    .WithProperty("OutputPath","../release-package")
                                                    );
    });

Task("Copy-Docker-Files")
    .IsDependentOn("Build-Solution")
    .Does(() =>
{
    CopyFiles("./DockerFile", @"./release-package/_PublishedWebsites");
});

Task("Build-WebApp-Docker-Image")
    .IsDependentOn("Copy-Docker-Files")
    .Does(() =>
{
    DockerBuild(new DockerBuildSettings(){
        Tag = new []{"dockerdemo:latest","hossambarakat/dockerdemo:latest"}
    },"./release-package/_PublishedWebsites");
});

Task("Publish-Docker-Image")
    .IsDependentOn("Build-WebApp-Docker-Image")
    .Does(() =>
{
    DockerLogin(new DockerLoginSettings(){
        Username = dockerhubUserName,
        Password = dockerhubPassword
    },"https://index.docker.io/v1/");
    DockerPush("hossambarakat/dockerdemo");
});

Task("Default").IsDependentOn("Publish-Docker-Image");
//Publish-Docker-Image
RunTarget(target);