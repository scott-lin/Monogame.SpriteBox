using System;

namespace Monogame.SpriteBox.PipelineExtension.Processor.Packing
{
    /// <summary>
    /// Defines states controlling how sprites are packed.
    /// </summary>
    class PackConfiguration
    {
        /// <summary>
        /// Gets a value indicating whether the final sprite's x and y dimensions must be a power of two respectively.
        /// </summary>
        public bool IsPowerOfTwo { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the final sprite's x and y dimensions must be equal.
        /// </summary>
        public bool IsSquare { get; private set; }

        /// <summary>
        /// Gets the maximum height in pixels the final sprite may be.
        /// </summary>
        public int MaximumHeight { get; private set; }

        /// <summary>
        /// Gets the maximum width in pixels the final sprite may be.
        /// </summary>
        public int MaximumWidth { get; private set; }

        /// <summary>
        /// Gets the number of blank pixels to add between each packed sprite.
        /// </summary>
        public int Padding { get; private set; }

        /// <summary>
        /// Initializes an instance of <see cref="PackConfiguration" />.
        /// </summary>
        /// <param name="maximumWidth">Maximum width in pixels the final sprite may be.</param>
        /// <param name="maximumHeight">Maximum height in pixels the final sprite may be.</param>
        /// <param name="isPowerOfTwo">Indicates whether the final sprite's x and y dimensions must be a power of two respectively.</param>
        /// <param name="isSquare">Indicates whether the final sprite's x and y dimensions must be equal.</param>
        /// <param name="padding">Number of blank pixels to add between each packed sprite.</param>
        public PackConfiguration( int maximumWidth, int maximumHeight, bool isPowerOfTwo, bool isSquare, int padding )
        {
            if ( maximumWidth <= 0 )
            {
                throw new ArgumentOutOfRangeException( nameof( maximumWidth ), maximumWidth, "Must be greater than zero." );
            }

            if ( maximumHeight <= 0 )
            {
                throw new ArgumentOutOfRangeException( nameof( maximumHeight ), maximumHeight, "Must be greater than zero." );
            }

            if ( padding < 0 )
            {
                throw new ArgumentOutOfRangeException( nameof( padding ), padding, "Must be equal to or greater than zero." );
            }

            this.IsPowerOfTwo = isPowerOfTwo;
            this.IsSquare = isSquare;
            this.MaximumHeight = maximumHeight;
            this.MaximumWidth = maximumWidth;
            this.Padding = padding;
        }
    }
}
