Assets\MSBuild.exe .\Sources\Khrussk.Interfaces.proj
Assets\MSBuild.exe .\Sources\Khrussk.Peers.proj
Assets\MSBuild.exe .\Sources\Khrussk.NetworkRealm.proj
Assets\MSBuild.exe .\Sources\Khrussk.NetworkRealm.Silverlight.proj 

Assets\NuGet.exe pack .\Packages\Specifications\Khrussk.Interfaces.nuspec -OutputDirectory packages
Assets\NuGet.exe pack .\Packages\Specifications\Khrussk.Peers.nuspec -OutputDirectory packages
Assets\NuGet.exe pack .\Packages\Specifications\Khrussk.NetworkRealm.nuspec -OutputDirectory packages
Assets\NuGet.exe pack .\Packages\Specifications\Khrussk.NetworkRealm.Silverlight.nuspec -OutputDirectory packages
