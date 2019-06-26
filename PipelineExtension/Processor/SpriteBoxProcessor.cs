using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using GlobExpressions;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;
using Monogame.SpriteBox.PipelineExtension.Importer;
using Monogame.SpriteBox.PipelineExtension.Processor.Packing;

namespace Monogame.SpriteBox.PipelineExtension.Processor
{
    /// <summary>
    /// Defines behavior for packing sprites into a sheet according to a <see cref="SpriteSheetDeclaration"/>.
    /// </summary>
    [ContentProcessor( DisplayName = "Sprite Box Processor" )]
    public class SpriteBoxProcessor : ContentProcessor<SpriteSheetDeclaration, SpriteSheetContent>
    {
        static readonly Color DefaultColorKeyColor = Color.Transparent;
        const bool DefaultGenerateMipmaps = false;
        const bool DefaultIsColorKeyEnabled = false;
        const bool DefaultIsPowerOfTwo = false;
        const bool DefaultIsSquare = false;
        const int DefaultPadding = 1;
        const bool DefaultPremultiplyTextureAlpha = true;
        static readonly TextureProcessorOutputFormat DefaultTextureFormat = TextureProcessorOutputFormat.Color;

        /// <summary>
        /// Gets or sets the color value to replace with transparent black.
        /// </summary>
        [DefaultValue( typeof( Color ), "0, 0, 0, 0" )]
        [DisplayName( "Color Key Color" )]
        [Description( "If the texture is color-keyed, pixels of this color are replaced with transparent black." )]
        public Color ColorKeyColor { get; set; } = DefaultColorKeyColor;

        /// <summary>
        /// Gets or sets a value indicating whether a full chain of mipmaps are generated from the source material. Existing mipmaps of the material are not replaced.
        /// </summary>
        [DefaultValue( DefaultGenerateMipmaps )]
        [DisplayName( "Generate Mipmaps?" )]
        [Description( "If enabled, a full chain of mipmaps are generated from the source material. Existing mipmaps of the material are not replaced." )]
        public bool GenerateMipmaps { get; set; } = DefaultGenerateMipmaps;

        /// <summary>
        /// Gets or sets a value indicating whether color keying of a texture is enabled.
        /// </summary>
        [DefaultValue( DefaultIsColorKeyEnabled )]
        [DisplayName( "Color Key Enabled?" )]
        [Description( "If enabled, the source texture is color-keyed. Pixels matching the value of \"Color Key Color\" are replaced with transparent black." )]
        public bool IsColorKeyEnabled { get; set; } = DefaultIsColorKeyEnabled;

        /// <summary>
        /// Gets or sets a value indicating whether the sprite sheet dimensions must be a power of two.
        /// </summary>
        [DefaultValue( DefaultIsPowerOfTwo )]
        [DisplayName( "Power of Two?" )]
        [Description( "When true, dimensions of the sprite sheet will be a power of two." )]
        public bool IsPowerOfTwo { get; set; } = DefaultIsPowerOfTwo;

        /// <summary>
        /// Gets or sets a value indicating whether the sprite sheet dimensions must be square.
        /// </summary>
        [DefaultValue( DefaultIsSquare )]
        [DisplayName( "Square?" )]
        [Description( "When true, the x and y dimensions of the sprite sheet will be equal." )]
        public bool IsSquare { get; set; } = DefaultIsSquare;

        /// <summary>
        /// Gets or sets the number of blank pixels to add between individual sprites in the sheet.
        /// </summary>
        [DefaultValue( DefaultPadding )]
        [DisplayName( "Padding" )]
        [Description( "Number of blank pixels to add between sprites in the sheet." )]
        public int Padding { get; set; } = DefaultPadding;

        /// <summary>
        /// Gets or sets a value indicating whether alpha premultiply of textures is enabled.
        /// </summary>
        [DefaultValue( DefaultPremultiplyTextureAlpha )]
        [DisplayName( "Premultiply Alpha?" )]
        [Description( "If enabled, the texture is converted to premultiplied alpha format." )]
        public bool PremultiplyTextureAlpha { get; set; } = DefaultPremultiplyTextureAlpha;

        /// <summary>
        /// Specifies the texture format of output materials. Materials can either be left unchanged from the source asset, converted to a corresponding Color, or compressed using the appropriate DxtCompressed format.
        /// </summary>
        [DefaultValue( typeof( TextureProcessorOutputFormat ), "Color" )]
        [DisplayName( "Texture Format" )]
        [Description( "Specifies the SurfaceFormat type of processed textures. Textures can either remain unchanged from the source asset, converted to the Color format, or DXT compressed." )]
        public TextureProcessorOutputFormat TextureFormat { get; set; } = DefaultTextureFormat;

        ExternalReference<TextureContent> BuildTexture( string name, string sourcePath, ContentProcessorContext context )
        {
            var parameters = new OpaqueDataDictionary();

            parameters.Add( "ColorKeyColor", this.ColorKeyColor );
            parameters.Add( "ColorKeyEnabled", this.IsColorKeyEnabled );
            parameters.Add( "GenerateMipmaps", this.GenerateMipmaps );
            parameters.Add( "PremultiplyAlpha", this.PremultiplyTextureAlpha );
            parameters.Add( "ResizeToPowerOfTwo", false );
            parameters.Add( "TextureFormat", this.TextureFormat );

            var sourceTexture = new ExternalReference<TextureContent>( sourcePath );
            sourceTexture.Name = name;

            return context.BuildAsset<TextureContent, TextureContent>(
                sourceAsset: sourceTexture,
                processorName: "TextureProcessor",
                processorParameters: parameters,
                importerName: "TextureImporter",
                assetName: name );
        }

        /// <summary>
        /// Loads source sprites referenced in declaration input and packs sprites into a sprite sheet.
        /// </summary>
        /// <param name="declaration">Sprite sheet delcaration input.</param>
        /// <param name="context">Context of the process routine.</param>
        /// <returns>Sprite sheet content.</returns>
        public override SpriteSheetContent Process( SpriteSheetDeclaration declaration, ContentProcessorContext context )
        {
            if ( declaration == null )
            {
                throw new ArgumentNullException( nameof( declaration ) );
            }

            if ( context == null )
            {
                throw new ArgumentNullException( nameof( context ) );
            }

            // Collect all texture file paths.
            //
            var texturePaths = new HashSet<string>();

            foreach ( string pathPattern in declaration.TexturePathPatterns )
            {
                foreach ( string filePath in Glob.Files( Directory.GetCurrentDirectory(), pathPattern ) )
                {
                    texturePaths.Add( filePath );
                }
            }

            // Limit the packed texture based on the target graphics profile.
            //
            int maximumDimension = context.TargetProfile == GraphicsProfile.HiDef ? 4096 : 2048;

            PackedTexture packedTexture =
                SpritePacker.Pack(
                    configuration: new PackConfiguration( maximumDimension, maximumDimension, this.IsPowerOfTwo, this.IsSquare, this.Padding ),
                    spritePaths: texturePaths.ToArray() );

            // Ensure our output directory exists.
            //            
            if ( !Directory.Exists( context.OutputDirectory ) )
            {
                Directory.CreateDirectory( context.OutputDirectory );
            }

            // Write the packed texture to the output location.
            //
            string outputPath = Path.Combine( context.OutputDirectory, $"{declaration.Name}.png" );

            using ( var stream = new FileStream( outputPath, FileMode.Create ) )
            {
                packedTexture.Texture.Save( stream, ImageFormat.Png );
            }

            context.AddOutputFile( outputPath );

            // Finalize the content to serialize from this processor.
            //
            return new SpriteSheetContent
            {
                Name = declaration.Name,
                SpriteNamesToIndexMapping = packedTexture.SpriteNamesToIndexMapping,
                SpriteRectangles = packedTexture.SpriteRectangles,
                Texture = this.BuildTexture( $"{declaration.Name}Texture", outputPath, context ),
            };
        }
    }
}