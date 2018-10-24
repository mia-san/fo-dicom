﻿namespace Dicom.Imaging.Codec
{
    /// <summary>
    /// borrowed from Native\Universal\Dicom.Imaging.Codec.Jpeg.h
    /// </summary>
    public interface IJpegNativeCodec
    {
        /// <summary>
        ///
        /// </summary>
        JpegMode Mode { get; }

        /// <summary>
        ///
        /// </summary>
        int PointTransform { get; }

        /// <summary>
        ///
        /// </summary>
        int Predictor { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="oldPixelData"></param>
        /// <param name="newPixelData"></param>
        /// <param name="jpegParams"></param>
        /// <param name="frame"></param>
        void Decode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomJpegParams jpegParams, int frame);

        /// <summary>
        ///
        /// </summary>
        /// <param name="oldPixelData"></param>
        /// <param name="newPixelData"></param>
        /// <param name="jpegParams"></param>
        /// <param name="frame"></param>
        void Encode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomJpegParams jpegParams, int frame);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pixelData"></param>
        /// <returns></returns>
        int ScanHeaderForPrecision(DicomPixelData pixelData);
    }
}