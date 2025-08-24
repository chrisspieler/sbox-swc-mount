using Duccsoft.Mounting;
using Sandbox.Mounting;

namespace Duccsoft;

public class JspSpriteLoader( JspFile jspFile, int spriteIndex, MountAssetPath spritePath ) : ResourceLoader<HamumuMount>
{
	protected override object Load()
	{
		return jspFile.CreateSpriteTexture( spriteIndex, spritePath );
	}
}
