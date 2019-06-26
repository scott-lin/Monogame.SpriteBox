using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monogame.SpriteBox
{
    /// <summary>
    /// Encapsulates many individual sprite images, packed into different areas of a single larger texture, along with information describing where in that texture each sprite is located.
    /// </summary>
    public class SpriteSheet
    {
        /// <summary>
        /// Name of this sprite sheet.
        /// </summary>
        public string Name;

        /// <summary>
        /// Mapping of sprite's name to its index in <see cref="SpriteRectangles"/>.
        /// </summary>
        public IDictionary<string, int> SpriteNamesToIndexMapping { get; } = new Dictionary<string, int>();

        /// <summary>
        /// Locations in the texture where each sprite has been placed.
        /// </summary>
        public IList<Rectangle> SpriteRectangles { get; } = new List<Rectangle>();

        /// <summary>
        /// Gets the texture used by this sprite sheet which contains many separate sprite images.
        /// </summary>
        public Texture2D Texture;

        /// <summary>
        /// Looks up the numeric index of the specified sprite. This is useful when implementing animation by cycling through a series of related sprites.
        /// </summary>
        public int GetIndex( string spriteName )
        {
            if ( !this.SpriteNamesToIndexMapping.TryGetValue( spriteName, out int index ) )
            {
                throw new KeyNotFoundException( $"Sprite sheet does not contain a sprite named '{spriteName}'." );
            }

            return index;
        }

        /// <summary>
        /// Gets the location of a sprite within <see cref="Texture"/>.
        /// </summary>
        /// <param name="spriteIndex">Index of the sprite to get location of.</param>
        /// <returns>Rectangle which represents location of sprite within the texture.</returns>
        public Rectangle GetSourceRectangle( int spriteIndex )
        {
            if ( spriteIndex < 0 || spriteIndex >= this.SpriteRectangles.Count )
            {
                throw new ArgumentOutOfRangeException( nameof( spriteIndex ), spriteIndex, $"Must be between zero and source rectangle count ({this.SpriteRectangles.Count}) inclusive." );
            }

            return this.SpriteRectangles[spriteIndex];
        }

        /// <summary>
        /// Gets the location of a sprite within <see cref="Texture"/>.
        /// </summary>
        /// <param name="spriteName">Name of the sprite to get location of.</param>
        /// <returns>Rectangle which represents location of sprite within the texture.</returns>
        public Rectangle GetSourceRectangle( string spriteName )
        {
            return this.SpriteRectangles[this.GetIndex( spriteName )];
        }
    }
}
