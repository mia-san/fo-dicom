namespace Dicom.Imaging.Codec
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
        void Decode(NativePixelData oldPixelData, NativePixelData newPixelData, NativeJpegParameters jpegParams, int frame);

        /// <summary>
        ///
        /// </summary>
        /// <param name="oldPixelData"></param>
        /// <param name="newPixelData"></param>
        /// <param name="jpegParams"></param>
        /// <param name="frame"></param>
        void Encode(NativePixelData oldPixelData, NativePixelData newPixelData, NativeJpegParameters jpegParams, int frame);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pixelData"></param>
        /// <returns></returns>
        int ScanHeaderForPrecision(NativePixelData pixelData);
    }
}
