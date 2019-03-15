// Copyright (c) 2012-2019 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

using Dicom.Imaging.Codec;
using System;

namespace Dicom.Imaging
{
    /// <summary>
    /// DICOM Image class for image rendering.
    /// </summary>
    public class DicomImage
    {
        #region FIELDS

        private readonly object _lock = new object();

        #endregion FIELDS

        #region CONSTRUCTORS

        /// <summary>Creates DICOM image object from dataset</summary>
        /// <param name="dataset">Source dataset</param>
        /// <param name="frame">Zero indexed frame number. If <paramref name="frame"/> is set to a negative number, the
        /// <see cref="DicomImage"/> object will remain in a partly initialized state, allowing for <see cref="WindowCenter"/>,
        /// <see cref="WindowWidth"/> and <see cref="GrayscaleColorMap"/> to be configured prior to rendering the image frames.</param>
        public DicomImage(DicomDataset dataset, int frame = 0)
        {
            ShowOverlays = true;

            Dataset = DicomTranscoder.ExtractOverlays(dataset);
            PixelData = DicomPixelData.Create(dataset);
            CurrentFrame = frame;

            var photometric_interpretation = QueryInternalPhotometricInterpretation(Dataset, PixelData);
            IsGrayscale = photometric_interpretation == PhotometricInterpretation.Monochrome1 || photometric_interpretation == PhotometricInterpretation.Monochrome2;
        }

        /// <summary>Creates DICOM image object from file</summary>
        /// <param name="fileName">Source file</param>
        /// <param name="frame">Zero indexed frame number</param>
        public DicomImage(string fileName, int frame = 0)
            : this(DicomFile.Open(fileName).Dataset, frame)
        {
        }

        #endregion CONSTRUCTORS

        #region PROPERTIES

        /// <summary>
        /// Gets the dataset constituting the DICOM image.
        /// </summary>
        [Obsolete("Dataset should not be publicly accessible from DicomImage object.")]
        public DicomDataset Dataset { get; private set; }

        /// <summary>
        /// Gets the pixel data header object associated with the image.
        /// </summary>
        [Obsolete("PixelData should not be publicly accessible from the DicomImage object.")]
        public DicomPixelData PixelData { get; private set; }

        [Obsolete("Use IsGrayscale to determine whether DicomImage object is grayscale or color.")]
        public PhotometricInterpretation PhotometricInterpretation => PixelData.PhotometricInterpretation;

        /// <summary>Width of image in pixels</summary>
        public int Width => PixelData.Width;

        /// <summary>Height of image in pixels</summary>
        public int Height => PixelData.Height;

        /// <summary>Scaling factor of the rendered image</summary>
        public double Scale { get; set; } = 1.0;

        // Note that the NumberOfFrames getter accesses the dataset's attribute. This is because the corresponding
        // getter in the pixel data might not be complete in the case of encapsulated datasets, where the frames are
        // continuously added upon request.

        /// <summary>Number of frames contained in image data.</summary>
        public int NumberOfFrames => Dataset.GetSingleValueOrDefault(DicomTag.NumberOfFrames, 1);

        /// <summary>
        ///
        /// </summary>
        private double _windowWidth;

        /// <summary>Gets or sets window width of rendered gray scale image.</summary>
        public double WindowWidth
        {
            get
            {
                return IsGrayscale ? 255 : _windowWidth;
            }
            set
            {
                if (!IsGrayscale)
                {
                    _windowWidth = value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        private double _windowCenter;

        /// <summary>Gets or sets window center of rendered gray scale image.</summary>
        public double WindowCenter
        {
            get
            {
                return IsGrayscale ? 127 : _windowCenter;
            }
            set
            {
                if (!IsGrayscale)
                {
                    _windowCenter = value;
                }
            }
        }

        /// <summary>Gets or sets the color map to be applied when rendering grayscale images.</summary>
        private Color32[] _grayscaleColorMap;

        /// <summary>
        ///
        /// </summary>
        public Color32[] GrayscaleColorMap
        {
            get
            {
                return this._grayscaleColorMap;
            }
            set
            {
                if (this.IsGrayscale)
                {
                    throw new DicomImagingException(
                        "Grayscale color map not applicable for photometric interpretation: {0}",
                        PixelData.PhotometricInterpretation);
                }
                else
                {
                    this._grayscaleColorMap = value;
                }
            }
        }

        /// <summary>Gets or sets whether the image is gray scale.</summary>
        public bool IsGrayscale { get; }

        /// <summary>Show or hide DICOM overlays</summary>
        public bool ShowOverlays { get; set; }

        /// <summary>Gets or sets the color used for displaying DICOM overlays. Default is magenta.</summary>
        public int OverlayColor { get; set; } = unchecked((int)0xffff00ff);

        /// <summary>
        /// Gets the index of the current frame.
        /// </summary>
        public int CurrentFrame { get; private set; }

        #endregion PROPERTIES

        #region METHODS

        /// <summary>
        ///
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public DicomFrame GetDicomFrame(int frame = 0)
        {
            return new DicomFrame(this.PixelData, frame)
            {
                GrayscaleColorMap = this.GrayscaleColorMap,
                OverlayColor = new Color32(this.OverlayColor),
                Scale = this.Scale,
                ShowOverlays = this.ShowOverlays,
                WindowCenter = this.WindowCenter,
                WindowWidth = this.WindowWidth,
            };
        }

        /// <summary>Renders DICOM image to <see cref="IImage"/>.</summary>
        /// <param name="frame">Zero indexed frame number.</param>
        /// <returns>Rendered image</returns>
        public virtual IImage RenderImage(int frame = 0)
        {
            this.CurrentFrame = frame;
            return this.GetDicomFrame(frame).Render();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="pixelData"></param>
        /// <returns></returns>
        private static PhotometricInterpretation QueryInternalPhotometricInterpretation(DicomDataset dataset, DicomPixelData pixelData)
        {
            var pi = pixelData.PhotometricInterpretation;
            var samples = dataset.GetSingleValueOrDefault(DicomTag.SamplesPerPixel, (ushort)0);

            if ((dataset.InternalTransferSyntax == DicomTransferSyntax.JPEGProcess1 || dataset.InternalTransferSyntax == DicomTransferSyntax.JPEGProcess2_4) && samples == 3)
            {
                // temporary fix for JPEG compressed YBR images
                pi = PhotometricInterpretation.Rgb;
            }
            else if (pi == PhotometricInterpretation.YbrIct || pi == PhotometricInterpretation.YbrRct)
            {
                // temporary fix for JPEG 2000 Lossy images
                pi = PhotometricInterpretation.Rgb;
            }
            else if (pi == null)
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

            return pi;
        }

        #endregion METHODS
    }
}
