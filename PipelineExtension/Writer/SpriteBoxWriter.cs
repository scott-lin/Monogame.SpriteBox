using System;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Monogame.SpriteBox.PipelineExtension.Processor;

namespace Monogame.SpriteBox.PipelineExtension.Writer
{
    /// <summary>
    /// Defines behavior for serializing a sprite sheet.
    /// </summary>
    [ContentTypeWriter]
    public class SpriteBoxWriter : ContentTypeWriter<SpriteSheetContent>
    {
        /// <summary>
        /// Serializes sprite sheet data.
        /// </summary>
        /// <param name="writer">Entity to use when writing data.</param>
        /// <param name="spriteSheetContent">Sprite sheet to serialize.</param>
        protected override void Write( ContentWriter writer, SpriteSheetContent spriteSheetContent )
        {
            if ( writer == null )
            {
                throw new ArgumentNullException( nameof( writer ) );
            }

            if ( spriteSheetContent == null )
            {
                throw new ArgumentNullException( nameof( spriteSheetContent ) );
            }

            writer.Write( spriteSheetContent.Name );
            writer.Write( spriteSheetContent.SpriteRectangles.Length );

            string[] spriteNames = spriteSheetContent.SpriteNamesToIndexMapping.Keys.ToArray();
            int[] spriteIndices = spriteSheetContent.SpriteNamesToIndexMapping.Values.ToArray();

            for ( int i = 0; i < spriteSheetContent.SpriteRectangles.Length; i++ )
            {
                writer.Write( spriteNames[i] );
                writer.Write( spriteIndices[i] );
                writer.Write( spriteSheetContent.SpriteRectangles[i].X );
                writer.Write( spriteSheetContent.SpriteRectangles[i].Y );
                writer.Write( spriteSheetContent.SpriteRectangles[i].Width );
                writer.Write( spriteSheetContent.SpriteRectangles[i].Height );
            }

            writer.WriteExternalReference( spriteSheetContent.Texture );
        }

        /// <summary>
        /// Gets the runtime type of the content serialized by this class.
        /// </summary>
        /// <param name="targetPlatform">Target platform.</param>
        /// <returns>Runtime type as string.</returns>
        public override string GetRuntimeType( TargetPlatform targetPlatform )
        {
            return "Monogame.SpriteBox.SpriteSheet";
        }

        /// <summary>
        /// Gets the runtime reader type, as a string, that is capable of deserializing data written by this class.
        /// </summary>
        /// <param name="targetPlatform">Target platform.</param>
        /// <returns>Runtime reader type as string.</returns>
        public override string GetRuntimeReader( TargetPlatform targetPlatform )
        {
            return "Monogame.SpriteBox.SpriteSheetReader, Monogame.SpriteBox";
        }
    }
}
