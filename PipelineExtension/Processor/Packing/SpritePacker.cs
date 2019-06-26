using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Monogame.SpriteBox.PipelineExtension.Processor.Packing
{
    /// <summary>
    /// Defines behavior for arranging many small sprites into a single larger sheet.
    /// </summary>
    static class SpritePacker
    {
        static IDictionary<string, int> BuildNameToIndexMap( Sprite[] sprites )
        {
            var map = new Dictionary<string, int>( sprites.Length );

            foreach ( Sprite sprite in sprites )
            {
                if ( map.ContainsKey( sprite.Name ) )
                {
                    string conflictPath = sprites.First( other => sprite.Name == other.Name ).FilePath;

                    throw new Exception( $"Sprite name conflict encountered. File names determine sprite names, and sprites at '{sprite.FilePath}' and '{conflictPath}' have the same name." );
                }

                map.Add( sprite.Name, sprite.Index );
            }

            return map;
        }

        static Rectangle[] BuildRectangles( Sprite[] sprites, int padding )
        {
            return sprites.Select(
                sprite => new Rectangle(
                    sprite.X + padding,
                    sprite.Y + padding,
                    sprite.Width - ( padding * 2 ),
                    sprite.Height - ( padding * 2 ) ) )
                .ToArray();
        }

        static Bitmap BuildTexture( Sprite[] sprites, int width, int height, int padding )
        {
            var texture = new Bitmap( width, height, PixelFormat.Format32bppArgb );

            // For all sprites, copy their pixel data over to the final texture based on the positions previously determined.
            //
            foreach ( Sprite sprite in sprites )
            {
                for ( int x = 0; x < sprite.Bitmap.Width; x++ )
                {
                    for ( int y = 0; y < sprite.Bitmap.Height; y++ )
                    {
                        int tempY = sprite.Y + y + padding;
                        if ( tempY < 0 || tempY > height )
                        {
                            throw new Exception( $"Sprite index is {sprite.Index} of {sprites.Length}. Y = {tempY}, Height = {height}" );
                        }

                        texture.SetPixel( sprite.X + x + padding, sprite.Y + y + padding, sprite.Bitmap.GetPixel( x, y ) );
                    }
                }
            }

            return texture;
        }

        /// <summary>
        /// Calculates a power of two that is greater than or equal to a value.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Power of two value that is greater than or equal to the input.</returns>
        static int CalculateNextPowerOfTwo( int value )
        {
            value--;

            for ( int i = 1; i < sizeof( int ) * 8; i <<= 1 )
            {
                value = value | value >> i;
            }

            return value + 1;
        }

        /// <summary>
        /// Calculates a width for the packed texture.
        /// </summary>
        /// <param name="sprites">Sprites to pack.</param>
        /// <param name="isPowerOfTwo">Indicates whether the width must be a power of two.</param>
        /// <returns>Width of the packed texture.</returns>
        static int CalculateWidth( Sprite[] sprites, bool isPowerOfTwo )
        {
            int[] spriteWidths = sprites.Select( sprite => sprite.Width ).OrderBy( w => w ).ToArray();
            int maxWidth = spriteWidths[spriteWidths.Length - 1];
            int medianWidth = spriteWidths[spriteWidths.Length / 2];

            int width = medianWidth * (int)Math.Round( Math.Sqrt( sprites.Length ) );

            // Ensure the width calculated is not less than the widest sprite. If it is, we use the widest sprite width instead.
            //
            width = Math.Max( width, maxWidth );

            if ( isPowerOfTwo )
            {
                width = SpritePacker.CalculateNextPowerOfTwo( width );
            }

            return width;
        }

        static void FindSpritePosition( Sprite[] sprites, int index, int textureWidth, out int x, out int y )
        {
            x = 0;
            y = 0;

            while ( true )
            {
                if ( !SpritePacker.IsIntersectingPreviousSprite( sprites, index, x, y, out int intersectionIndex ) )
                {
                    return;
                }

                // Skip to the right of the previously positioned sprite that we collided with. Howver, move down when we
                // can no longer fit on the current line.
                //
                x = sprites[intersectionIndex].X + sprites[intersectionIndex].Width;

                if ( x + sprites[index].Width > textureWidth )
                {
                    x = 0;
                    y++;
                }
            }
        }

        static bool IsIntersectingPreviousSprite( Sprite[] sprites, int index, int x, int y, out int intersectionIndex )
        {
            intersectionIndex = -1;

            int w = sprites[index].Width;
            int h = sprites[index].Height;

            for ( int i = 0; i < index; i++ )
            {
                if ( sprites[i].X >= x + w
                    || sprites[i].X + sprites[i].Width <= x
                    || sprites[i].Y >= y + h
                    || sprites[i].Y + sprites[i].Height <= y )
                {
                    continue;
                }

                intersectionIndex = i;

                return true;
            }

            return false;
        }

        static Sprite[] LoadSprites( string[] spritePaths, int padding )
        {
            var sprites = new List<Sprite>( spritePaths.Length );

            for ( int i = 0; i < spritePaths.Length; i++ )
            {
                var bitmap = Image.FromFile( spritePaths[i] ) as Bitmap;

                if ( bitmap == null )
                {
                    throw new Exception( $"Failed to load sprite at '{spritePaths[i]}' into memory as a bitmap image." );
                }

                sprites.Add( new Sprite
                {
                    Bitmap = bitmap,
                    FilePath = spritePaths[i],
                    Height = bitmap.Height + ( padding * 2 ),
                    Index = i,
                    Name = Path.GetFileNameWithoutExtension( spritePaths[i] ),
                    Width = bitmap.Width + ( padding * 2 ),
                } );
            }

            return sprites.ToArray();
        }

        /// <summary>
        /// Packs many individual sprites into a single sprite and records the individual sprite names and locations inside the large sprite.
        /// </summary>
        /// <param name="configuration">Settings controlling how to pack sprites.</param>
        /// <param name="spritePaths">File paths of sprites to pack.</param>
        /// <returns>Object containing the packed texture along with the names and locations of the individual sprites inside of it.</returns>
        public static PackedTexture Pack( PackConfiguration configuration, string[] spritePaths )
        {
            if ( configuration == null )
            {
                throw new ArgumentNullException( nameof( configuration ) );
            }

            if ( spritePaths == null )
            {
                throw new ArgumentNullException( nameof( spritePaths ) );
            }

            if ( spritePaths.Length <= 0 )
            {
                throw new ArgumentException( "Must contain at least one element.", nameof( spritePaths ) );
            }

            Sprite[] sprites = SpritePacker.LoadSprites( spritePaths.Distinct().ToArray(), configuration.Padding );
            int textureWidth = SpritePacker.CalculateWidth( sprites, configuration.IsPowerOfTwo );
            int textureHeight = 0;

            // Find the position of each sprite in the final texture.
            //
            Sprite[] orderedSprites = SpritePacker.OrderSprites( sprites );

            for ( int i = 0; i < orderedSprites.Length; i++ )
            {
                SpritePacker.FindSpritePosition( orderedSprites, i, textureWidth, out int x, out int y );

                orderedSprites[i].X = x;
                orderedSprites[i].Y = y;

                // Update texture height when the just positioned sprite pushes the boundary out.
                //
                textureHeight = Math.Max( textureHeight, orderedSprites[i].Y + orderedSprites[i].Height );
            }

            // Ensure height dimension is a power of two when the user asked for as much.
            //
            if ( configuration.IsPowerOfTwo )
            {
                textureHeight = SpritePacker.CalculateNextPowerOfTwo( textureHeight );
            }

            // Ensure both dimensions are square when user asked for as much.
            //
            if ( configuration.IsSquare )
            {
                int maximum = Math.Max( textureWidth, textureHeight );

                textureWidth = maximum;
                textureHeight = maximum;
            }

            // Ensure the final dimensions are within the user's limits.
            //
            if ( textureWidth > configuration.MaximumWidth || textureHeight > configuration.MaximumHeight )
            {
                throw new Exception( $"Packed texture dimensions ({textureWidth} x {textureHeight}) exceed the maximum requested ({configuration.MaximumWidth} x {configuration.MaximumHeight})." );
            }

            return new PackedTexture
            {
                SpriteNamesToIndexMapping = SpritePacker.BuildNameToIndexMap( sprites ),
                SpriteRectangles = SpritePacker.BuildRectangles( sprites, configuration.Padding ),
                Texture = SpritePacker.BuildTexture( sprites, textureWidth, textureHeight, configuration.Padding ),
            };
        }

        static Sprite[] OrderSprites( Sprite[] sprites )
        {
            List<Sprite> orderedSprites = sprites.ToList();

            // Weight the height of sprites, so the tallest sprites are packed first.
            //
            orderedSprites.Sort( ( a, b ) =>
            {
                int aSize = a.Height * 1024 + a.Width;
                int bSize = b.Height * 1024 + b.Width;

                return bSize.CompareTo( aSize );
            } );

            return orderedSprites.ToArray();
        }
    }
}
