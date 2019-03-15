using Dicom.Imaging.Codec;
using Dicom.Imaging.Render;
using System.Linq;

namespace Dicom.Imaging
{
    /// <summary>
    /// represents single frame in pixel data
    /// </summary>
    public class DicomFrame
    {
        /// <summary>
        /// 0-based frame number
        /// </summary>
        public int FrameIndex { get; }

        /// <summary>
        ///
        /// </summary>
        public Color32[] GrayscaleColorMap { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int Height { get; }

        /// <summary>
        ///
        /// </summary>
        public bool IsGrayScale { get; }

        /// <summary>
        ///
        /// </summary>
        public Color32 OverlayColor { get; set; } = new Color32(0xff, 0xff, 0x00, 0xff);

        /// <summary>
        /// DicomPixelData this frame created from
        /// </summary>
        public DicomPixelData PixelData { get; }

        /// <summary>
        /// scale factor
        /// </summary>
        public double Scale { get; set; } = 1;

        /// <summary>
        ///
        /// </summary>
        public bool ShowOverlays { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int Width { get; }

        /// <summary>
        ///
        /// </summary>
        public double WindowCenter { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double WindowWidth { get; set; }

        /// <summary>
        ///
        /// </summary>
        private readonly IPixelData _pixeldata;

        /// <summary>
        ///
        /// </summary>
        private class PipelineData
        {
            /// <summary>
            ///
            /// </summary>
            internal IPipeline Pipeline { get; set; }

            /// <summary>
            ///
            /// </summary>
            internal GrayscaleRenderOptions RenderOptions { get; set; }
        }

        /// <summary>
        ///
        /// </summary>
        private readonly PipelineData _pipeline;

        /// <summary>
        ///
        /// </summary>
        private readonly DicomOverlayData[] _overlays;

        /// <summary>
        ///
        /// </summary>
        /// <param name="image"></param>
        /// <param name="frameIndex"></param>
        public DicomFrame(DicomPixelData pixelData, int frameIndex)
        {
            this.FrameIndex = frameIndex;

            this._overlays = CreateGraphicsOverlays(pixelData.Dataset);
            this._pipeline = CreatePipelineData(pixelData.Dataset, pixelData);
            this._pixeldata = DecodeFrame(pixelData, frameIndex);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pixelData"></param>
        /// <param name="frameIndex"></param>
        /// <returns></returns>
        private static IPixelData DecodeFrame(DicomPixelData pixelData, int frameIndex)
        {
            var transfer_syntax = pixelData.Dataset.InternalTransferSyntax;
            if (transfer_syntax.IsEncapsulated)
            {
                var transcoder = new DicomTranscoder(
                    transfer_syntax,
                    DicomTransferSyntax.ExplicitVRLittleEndian
                );
                return transcoder.DecodePixelData(pixelData.Dataset, frameIndex);
            }
            else
            {
                return PixelDataFactory.Create(pixelData, frameIndex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IImage Render()
        {
            var scaled_pixels = _pixeldata.Rescale(Scale);

            var graphic = new ImageGraphic(scaled_pixels);

            if (ShowOverlays)
            {
                foreach (var overlay in _overlays)
                {
                    var og = new OverlayGraphic(
                        PixelDataFactory.Create(overlay),
                        overlay.OriginX - 1,
                        overlay.OriginY - 1,
                        OverlayColor.Value
                    );
                    graphic.AddOverlay(og);
                    og.Scale(Scale);
                }
            }

            if (_pipeline.RenderOptions != null)
            {
                _pipeline.RenderOptions.WindowCenter = WindowCenter;
                _pipeline.RenderOptions.WindowWidth = WindowWidth;
                _pipeline.RenderOptions.ColorMap = this.GrayscaleColorMap;
            }

            return graphic.RenderImage(_pipeline.Pipeline.LUT);
        }

        /// <summary>
        /// Create array of graphics overlays for this frame from dataset.
        /// </summary>
        /// <param name="dataset">Dataset potentially containing overlays.</param>
        /// <returns>Array of overlays of type <see cref="DicomOverlayType.Graphics"/>.</returns>
        private DicomOverlayData[] CreateGraphicsOverlays(DicomDataset dataset)
        {
            var frame_index = this.FrameIndex + 1;

            return DicomOverlayData.FromDataset(dataset)
                .Where(x => x.Type == DicomOverlayType.Graphics && x.Data != null && x.OriginFrame <= frame_index && frame_index < x.OriginFrame + x.NumberOfFrames)
                .ToArray();
        }

        /// <summary>
        /// Create image rendering pipeline according to the <see cref="DicomPixelData.PhotometricInterpretation">photometric interpretation</see>
        /// of the pixel data.
        /// </summary>
        private static PipelineData CreatePipelineData(DicomDataset dataset, DicomPixelData pixelData)
        {
            var pi = pixelData.PhotometricInterpretation;
            var samples = dataset.GetSingleValueOrDefault(DicomTag.SamplesPerPixel, (ushort)0);

            // temporary fix for JPEG compressed YBR images
            if ((dataset.InternalTransferSyntax == DicomTransferSyntax.JPEGProcess1
                 || dataset.InternalTransferSyntax == DicomTransferSyntax.JPEGProcess2_4) && samples == 3) pi = PhotometricInterpretation.Rgb;

            // temporary fix for JPEG 2000 Lossy images
            if (pi == PhotometricInterpretation.YbrIct || pi == PhotometricInterpretation.YbrRct)
            {
                pi = PhotometricInterpretation.Rgb;
            }

            if (pi == null)
            {
                // generally ACR-NEMA
                if (samples == 0 || samples == 1)
                {
                    pi = dataset.Contains(DicomTag.RedPaletteColorLookupTableData)
                        ? PhotometricInterpretation.PaletteColor
                        : PhotometricInterpretation.Monochrome2;
                }
                else
                {
                    // assume, probably incorrectly, that the image is RGB
                    pi = PhotometricInterpretation.Rgb;
                }
            }

            IPipeline pipeline;
            GrayscaleRenderOptions renderOptions = null;
            if (pi == PhotometricInterpretation.Monochrome1 || pi == PhotometricInterpretation.Monochrome2)
            {
                //Monochrome1 or Monochrome2 for grayscale image
                renderOptions = GrayscaleRenderOptions.FromDataset(dataset);
                pipeline = new GenericGrayscalePipeline(renderOptions);
            }
            else if (pi == PhotometricInterpretation.Rgb || pi == PhotometricInterpretation.YbrFull
                || pi == PhotometricInterpretation.YbrFull422 || pi == PhotometricInterpretation.YbrPartial422)
            {
                //RGB for color image
                pipeline = new RgbColorPipeline();
            }
            else if (pi == PhotometricInterpretation.PaletteColor)
            {
                //PALETTE COLOR for Palette image
                pipeline = new PaletteColorPipeline(pixelData);
            }
            else
            {
                throw new DicomImagingException("Unsupported pipeline photometric interpretation: {0}", pi);
            }

            return new PipelineData { Pipeline = pipeline, RenderOptions = renderOptions };
        }

        /// <summary>
        /// Create pixel data object based on <paramref name="dataset"/>.
        /// </summary>
        /// <param name="dataset">Dataset containing pixel data.</param>
        /// <returns>For non-encapsulated dataset, create pixel data object from original pixel data. For encapsulated dataset,
        /// create "empty" pixel data object to subsequentially be filled with uncompressed data for each frame.</returns>
        private static DicomPixelData CreateDicomPixelData(DicomDataset dataset)
        {
            var inputTransferSyntax = dataset.InternalTransferSyntax;
            if (!inputTransferSyntax.IsEncapsulated) return DicomPixelData.Create(dataset);

            // Clone the encapsulated dataset because modifying the pixel data modifies the dataset
            var clone = dataset.Clone();
            clone.InternalTransferSyntax = DicomTransferSyntax.ExplicitVRLittleEndian;

            var pixelData = DicomPixelData.Create(clone, true);

            // temporary fix for JPEG compressed YBR images, according to enforcement above
            if ((inputTransferSyntax == DicomTransferSyntax.JPEGProcess1
                 || inputTransferSyntax == DicomTransferSyntax.JPEGProcess2_4) && pixelData.SamplesPerPixel == 3)
            {
                // When converting to RGB in Dicom.Imaging.Codec.Jpeg.i, PlanarConfiguration is set to Interleaved
                pixelData.PhotometricInterpretation = PhotometricInterpretation.Rgb;
                pixelData.PlanarConfiguration = PlanarConfiguration.Interleaved;
            }

            // temporary fix for JPEG 2000 Lossy images
            if ((inputTransferSyntax == DicomTransferSyntax.JPEG2000Lossy
                 && pixelData.PhotometricInterpretation == PhotometricInterpretation.YbrIct)
                || (inputTransferSyntax == DicomTransferSyntax.JPEG2000Lossless
                    && pixelData.PhotometricInterpretation == PhotometricInterpretation.YbrRct))
            {
                // Converted to RGB in Dicom.Imaging.Codec.Jpeg2000.cpp
                pixelData.PhotometricInterpretation = PhotometricInterpretation.Rgb;
            }

            // temporary fix for JPEG2000 compressed YBR images
            if ((inputTransferSyntax == DicomTransferSyntax.JPEG2000Lossless
                 || inputTransferSyntax == DicomTransferSyntax.JPEG2000Lossy)
                && (pixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull
                    || pixelData.PhotometricInterpretation == PhotometricInterpretation.YbrFull422
                    || pixelData.PhotometricInterpretation == PhotometricInterpretation.YbrPartial422))
            {
                // For JPEG2000 YBR type images in Dicom.Imaging.Codec.Jpeg2000.cpp,
                // YBR_FULL is applied and PlanarConfiguration is set to Planar
                pixelData.PhotometricInterpretation = PhotometricInterpretation.YbrFull;
                pixelData.PlanarConfiguration = PlanarConfiguration.Planar;
            }

            return pixelData;
        }
    }
}
