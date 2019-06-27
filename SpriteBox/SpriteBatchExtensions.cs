using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monogame.SpriteBox
{
    /// <summary>
    /// Defines extension behavior for <see cref="SpriteBatch"/>.
    /// </summary>
    public static class SpriteBatchExtensions
    {
        /// <summary>
        /// Draws a sprite.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch to use.</param>
        /// <param name="sprite">Sprite to draw.</param>
        /// <param name="position">Position to draw sprite at.</param>
        /// <param name="color">Color to draw sprite with.</param>
        /// <param name="rotation">Rotation to draw sprite with.</param>
        /// <param name="scale">Scale to draw sprite with.</param>
        /// <param name="effect">Effect to draw sprite with.</param>
        /// <param name="layerDepth">Layer depth to draw sprite with.</param>
        public static void Draw( this SpriteBatch spriteBatch, Sprite sprite, Vector2 position, Color? color = null, float rotation = 0f, Vector2? scale = null, SpriteEffects effect = SpriteEffects.None, float layerDepth = 0f )
        {
            spriteBatch.Draw(
                sprite.SpriteSheet.Texture,
                position,
                sprite.SourceRectangle,
                color ?? Color.White,
                rotation,
                sprite.Origin,
                scale ?? Vector2.One,
                effect,
                layerDepth );
        }
    }
}
