using Microsoft.Xna.Framework;

namespace Monogame.SpriteBox
{
    /// <summary>
    /// Defines a sprite content asset.
    /// </summary>
    public class Sprite
    {
        /// <summary>
        /// Height of this sprite.
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// Name of this sprite.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Origin of this sprite.
        /// </summary>
        public readonly Vector2 Origin;

        /// <summary>
        /// Area bounding this sprite in its texture.
        /// </summary>
        public readonly Rectangle SourceRectangle;

        /// <summary>
        /// SpriteSheet which contains this sprite.
        /// </summary>
        public readonly SpriteSheet SpriteSheet;

        /// <summary>
        /// Width of this sprite.
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// Initializes an instance of <see cref="Sprite" />.
        /// </summary>
        /// <param name="name">Name of the sprite.</param>
        /// <param name="spriteSheet">SpriteSheet which contains the sprite.</param>
        public Sprite( string name, SpriteSheet spriteSheet )
        {
            this.Name = name;
            this.SpriteSheet = spriteSheet;

            this.SourceRectangle = spriteSheet.GetSourceRectangle( name );
            this.Width = this.SourceRectangle.Width;
            this.Height = this.SourceRectangle.Height;
            this.Origin = new Vector2( (float)this.Width / 2.0f, (float)this.Height / 2.0f );
        }
    }
}
