using System.IO;
using Sandbox.Mounting;

namespace Duccsoft;

public class SupremeWithCheeseMount : HamumuMount
{
	public override string Ident => "hamumu_swc_steam";
	public override string Title => "Dr. Lunatic Supreme With Steam";
	public override long AppId => 2547330;
	public string InstallerAssetsDirectory => Path.Combine( AppDirectory, "installers/supreme8_install.exe/" );
	protected override IEnumerable<AddMountResourceCommand> GetAllResources()
	{
		var jspPaths = Explorer.FindFilesRecursive( InstallerAssetsDirectory, "*.jsp" );
		foreach ( var jspPath in jspPaths )
		{
			var jspFile = JspFile.Load( jspPath );
			for ( int i = 0; i < jspFile.SpriteCount; i++ )
			{
				var spriteAssetPath = jspPath.WithBaseDirectory( InstallerAssetsDirectory );
				var newDir = Path.GetDirectoryName( spriteAssetPath.RelativePath );
				var newFileName = Path.GetFileNameWithoutExtension( spriteAssetPath.RelativePath )
				                  + "_"
				                  + i
				                  + Path.GetExtension( spriteAssetPath.RelativePath );
				spriteAssetPath = spriteAssetPath.WithRelativePath( Path.Combine( newDir!, newFileName ) );
				var spriteLoader = new JspSpriteLoader( jspFile, i, spriteAssetPath );
				yield return new AddMountResourceCommand( ResourceType.Texture, spriteAssetPath, spriteLoader );
			}
		}
	}
}
