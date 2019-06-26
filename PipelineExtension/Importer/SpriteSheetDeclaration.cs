using System.Collections.Generic;

namespace Monogame.SpriteBox.PipelineExtension.Importer
{
    /// <summary>
    /// Defines a sprite sheet and references to its source textures on the local file system.
    /// </summary>
    public class SpriteSheetDeclaration
    {
        /// <summary>
        /// Gets or sets the name of the sprite sheet.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a list of path glob patterns, which resolve to local disk files containing textures to pack into the sprite sheet.
        /// </summary>
        public IList<string> TexturePathPatterns { get; } = new List<string>();
    }
}
