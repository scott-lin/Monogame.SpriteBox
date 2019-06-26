using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;

namespace Monogame.SpriteBox.PipelineExtension.Importer
{
    /// <summary>
    /// Defines behavior for importing sprite sheet declaration.
    /// </summary>
    [ContentImporter( fileExtension: ".box", DefaultProcessor = "SpriteBoxProcessor", DisplayName = "Sprite Box Importer" )]
    public class SpriteBoxImporter : ContentImporter<SpriteSheetDeclaration>
    {
        /// <summary>
        /// Imports a sprite sheet declaration from a file's content.
        /// </summary>
        /// <param name="filePath">Path to the declaration file.</param>
        /// <param name="context">Context of the import routine.</param>
        /// <returns>Sprite sheet declaration populated with file data.</returns>
        public override SpriteSheetDeclaration Import( string filePath, ContentImporterContext context )
        {
            if ( string.IsNullOrWhiteSpace( filePath ) )
            {
                throw new ArgumentException( "Cannot be null or white space.", nameof( filePath ) );
            }

            string content;

            try
            {
                content = File.ReadAllText( filePath );
            }
            catch ( Exception exception )
            {
                context.Logger.LogMessage( $"Failed to read file '{filePath}' with exception: {exception}" );

                throw;
            }

            try
            {
                return JsonConvert.DeserializeObject<SpriteSheetDeclaration>( content );
            }
            catch ( Exception exception )
            {
                context.Logger.LogMessage( $"Failed to deserialize content of file '{filePath}' with exception: {exception}" );

                throw;
            }
        }
    }
}
