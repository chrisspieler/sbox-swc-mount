using System.IO;
using Duccsoft.Mounting;

namespace Duccsoft;

public partial class JspFile
{
	public record SpriteHeader( ushort Width, ushort Height, ushort OffsX, ushort OffsY, uint Size, uint Unknown );

	public JspFile( MountAssetPath assetPath )
	{
		AssetPath = assetPath;
		
		using var reader = new BinaryReader( File.OpenRead( AssetPath.SourcePath ) );

		SpriteCount = reader.ReadUInt16();

		// Read headers for each sprite.
		SpriteHeaders = new SpriteHeader[SpriteCount];
		for ( int i = 0; i < SpriteCount; i++ )
		{
			SpriteHeaders[i] = ReadSpriteHeader( reader );
		}

		// Read raw bytes for each sprite.
		SpriteData = new byte[SpriteCount][];
		for ( int i = 0; i < SpriteCount; i++ )
		{
			SpriteData[i] = reader.ReadBytes( (int)SpriteHeaders[i].Size );
		}
	}

	public static JspFile Load( MountAssetPath assetPath ) => new JspFile( assetPath );
	public MountAssetPath AssetPath { get; }
	public int SpriteCount { get; }
	public SpriteHeader[] SpriteHeaders { get; }
	public byte[][] SpriteData { get; }

	private SpriteHeader ReadSpriteHeader( BinaryReader reader )
	{
		return new SpriteHeader(
			Width: reader.ReadUInt16(),
			Height: reader.ReadUInt16(),
			OffsX: reader.ReadUInt16(),
			OffsY: reader.ReadUInt16(),
			Size: reader.ReadUInt32(),
			Unknown: reader.ReadUInt32()
		);
	}

	public Texture CreateSpriteTexture( int spriteIndex, MountAssetPath spritePath )
	{
		var header = SpriteHeaders[spriteIndex];
		var texData = new byte[header.Width * header.Height * 4];

		using var reader = new BinaryReader( new MemoryStream( SpriteData[spriteIndex] ) );
		using var writer = new BinaryWriter( new MemoryStream( texData ) );

		do
		{
			var b = reader.ReadByte();
			// This is a transparent run. Transparent run.
			if ( b > 128 )
			{
				for ( int i = 128; i < b; i++ )
				{
					writer.Write( (int)0 );
				}
			}
			else
			{
				for ( int i = 0; i < b; i++ )
				{
					var index = reader.ReadByte();
					writer.Write( GetPaletteColor( index ).RawInt );
				}
			}
		} while ( reader.BaseStream.Position < reader.BaseStream.Length );

		return Texture
			.Create( header.Width, header.Height, ImageFormat.RGBA8888 )
			.WithData( texData )
			.WithName( spritePath.ResourcePath )
			.Finish();
	}


}
