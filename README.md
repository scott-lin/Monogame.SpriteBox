# Monogame.SpriteBox

![Azure DevOps builds (branch)](https://img.shields.io/azure-devops/build/scott-lin/503c90f9-8364-43c3-ad65-9801d8630354/1/master.svg)
![Nuget](https://img.shields.io/nuget/v/Monogame.SpriteBox.svg?label=Runtime%20nuget)
![Nuget](https://img.shields.io/nuget/v/Monogame.SpriteBox.PipelineExtension.svg?label=Pipeline%20Extension%20nuget)

A Monogame content pipeline extension for combining many individual sprites into a single texture.

## Setup

Add a reference to the SpriteBox pipeline extension using the Monogame Pipeline Tool.

    nuget install Monogame.SpriteBox.PipelineExtension

Add the SpriteBox runtime to your game project to enable loading of content produced by the pipeline extension.

    nuget install Monogame.SpriteBox

## Usage

### Pipeline Extension

1. Add Monogame.SpriteBox.PipelineExtension.dll to your MGCB using the Monogame Pipeline Tool.

2. Create an new file with `.box` extension. Populate it with a name and path information hinting at which sprites you'd like to pack. Note, the path hints support [Glob patterns](https://en.wikipedia.org/wiki/Glob_(programming)) for convenience.

    Schema

        {
            "Name" : string,
            "TexturePathPatterns" : [ string ]
        }
    
    Example

        {
            "Name": "Environment",
            "TexturePathPatterns":
                [
                    "Backgrounds/BG1.png",
                    "Dungeons/Floors/**/*",
                    "Dungeons/Walls/*.png",
                    "Overworld/**/*.png
                ]
        }

3. Add the `.box` file to your MGCB the same way other assets are. Optionally, set the processor parameters documented in the [code](https://github.com/scott-lin/Monogame.SpriteBox/blob/master/PipelineExtension/Processor/SpriteBoxProcessor.cs).

4. Build

### Runtime

1. Load a sprite sheet using `ContentManager`:

       SpriteSheet mySpriteSheet = Content.Load<SpriteSheet>( "myAssetName" );

2. Draw sprites using the `SpriteBatchExtensions` method:

       spriteBatch.Draw( mySpiteSheet.Sprites["someSpriteName"], position: new Vector2(50f, 120f) );