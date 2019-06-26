using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Monogame.SpriteBox
{
    /// <summary>
    /// Defines behavior for deserializing XNB data into a <see cref="SpriteSheet"/>.
    /// </summary>
    public class SpriteSheetReader : ContentTypeReader<SpriteSheet>
    {
        /// <summary>
        /// Deserializes XNB content into a sprite sheet instance.
        /// </summary>
        /// <param name="reader">Entity to use when reading raw data.</param>
        /// <param name="existingInstance">Existing sprite sheet instance.</param>
        /// <returns>Sprite sheet initialized with XNB data.</returns>
        protected override SpriteSheet Read( ContentReader reader, SpriteSheet existingInstance )
        {
            if ( reader == null )
            {
                throw new ArgumentNullException( nameof( reader ) );
            }

            SpriteSheet spriteSheet = existingInstance == null ? new SpriteSheet() : existingInstance;

            spriteSheet.Name = reader.ReadString();
            int count = reader.ReadInt32();

            for ( int i = 0; i < count; i++ )
            {
                string spriteName = reader.ReadString();
                int spriteIndex = reader.ReadInt32();
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();

                spriteSheet.SpriteNamesToIndexMapping.Add( spriteName, spriteIndex );
                spriteSheet.SpriteRectangles.Add( new Rectangle( x, y, width, height ) );
            }

            spriteSheet.Texture = reader.ReadExternalReference<Texture2D>();

            return spriteSheet;
        }
    }
}
