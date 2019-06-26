using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Monogame.SpriteBox.PipelineExtension.Processor
{
    /// <summary>
    /// Defines the content of a sprite sheet.
    /// </summary>
    public class SpriteSheetContent
    {
        /// <summary>
        /// Name of the sprite sheet.
        /// </summary>
        public string Name;

        /// <summary>
        /// Mapping of sprite names to their index within <see cref="SpriteRectangles"/>.
        /// </summary>
        public IDictionary<string, int> SpriteNamesToIndexMapping;

        /// <summary>
        /// Locations of each sprite within <see cref="Texture"/>.
        /// </summary>
        public Rectangle[] SpriteRectangles;

        /// <summary>
        /// Reference to the sprite sheet texture.
        /// </summary>
        public ExternalReference<TextureContent> Texture;
    }
}