using System.Drawing;

namespace Monogame.SpriteBox.PipelineExtension.Processor.Packing
{
    /// <summary>
    /// Defines a sprite to pack.
    /// </summary>
    class Sprite
    {
        /// <summary>
        /// Bitmap content of this sprite.
        /// </summary>
        public Bitmap Bitmap;

        /// <summary>
        /// Path to the sprite on the local file system.
        /// </summary>
        public string FilePath;

        /// <summary>
        /// Height of this sprite in pixels including any padding.
        /// </summary>
        public int Height;

        /// <summary>
        /// Arrangement index of this sprite.
        /// </summary>
        public int Index;

        /// <summary>
        /// Name of the sprite.
        /// </summary>
        public string Name;

        /// <summary>
        /// Width of this sprite in pixels including any padding.
        /// </summary>
        public int Width;

        /// <summary>
        /// X-coordinate of this sprite in the packed texture.
        /// </summary>
        public int X;

        /// <summary>
        /// Y-coordinate of this sprite in the packed texture.
        /// </summary>
        public int Y;
    }
}
