using System.Collections.Generic;
using System.Drawing;

namespace Monogame.SpriteBox.PipelineExtension.Processor.Packing
{
    /// <summary>
    /// Defines a texture containing many individual sprites and the locations of said sprites.
    /// </summary>
    class PackedTexture
    {
        /// <summary>
        /// Mapping of sprite's name to its index in <see cref="SpriteRectangles"/>.
        /// </summary>
        public IDictionary<string, int> SpriteNamesToIndexMapping;

        /// <summary>
        /// Locations in the <see cref="Texture"/> where each sprite has been placed.
        /// </summary>
        public Rectangle[] SpriteRectangles;

        /// <summary>
        /// Texture containing many individual sprites.
        /// </summary>
        public Bitmap Texture;
    }
}
