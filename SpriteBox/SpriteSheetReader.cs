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

            //// Use custom read texture implementation to avoid automatic XNB deserialization. Reflection is used to find the deserializer,
            //// which creates an unwanted performance overhead and heap garbage to clean. This is undesirable on lower spec devices.
            ////
            //spriteSheet.Texture = this.ReadTexture( reader, spriteSheet );

            return spriteSheet;
        }

        Texture2D ReadTexture( ContentReader reader, SpriteSheet spriteSheet )
        {
            var format = (SurfaceFormat)reader.ReadInt32();
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            int levelCount = reader.ReadInt32();

            var graphicsDeviceManager = (GraphicsDeviceManager)reader.ContentManager.ServiceProvider.GetService( typeof( IGraphicsDeviceManager ) );
            var texture = new Texture2D( graphicsDeviceManager.GraphicsDevice, width, height, mipmap: levelCount > 1, format: format );

            for ( int level = 0; level < levelCount; level++ )
            {
                int levelPixelDataLength = reader.ReadInt32();
                byte[] levelPixelData = reader.ReadBytes( levelPixelDataLength );

                int levelWidth = Math.Max( width >> level, 1 );
                int levelHeight = Math.Max( height >> level, 1 );

                texture.SetData(
                    level,
                    rect: null,
                    data: levelPixelData,
                    startIndex: 0,
                    elementCount: levelPixelData.Length );
            }

            return texture;
        }
    }
}
