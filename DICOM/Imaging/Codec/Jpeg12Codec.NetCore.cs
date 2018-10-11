namespace Dicom.Imaging.Codec
{
    /// <summary>
    ///
    /// </summary>
    public class Jpeg12Codec : IJpegNativeCodec
    {
        /// <summary>
        ///
        /// </summary>
        public JpegMode Mode { get; }

        /// <summary>
        ///
        /// </summary>
        public int PointTransform { get; }

        /// <summary>
        ///
        /// </summary>
        public int Predictor { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="predictor"></param>
        /// <param name="pointTransform"></param>
        public Jpeg12Codec(JpegMode mode, int predictor, int pointTransform)
        {
            this.Mode = mode;
            this.PointTransform = pointTransform;
            this.Predictor = predictor;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="oldPixelData"></param>
        /// <param name="newPixelData"></param>
        /// <param name="jpegParams"></param>
        /// <param name="frame"></param>
        public void Decode(NativePixelData oldPixelData, NativePixelData newPixelData, NativeJpegParameters jpegParams, int frame)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="oldPixelData"></param>
        /// <param name="newPixelData"></param>
        /// <param name="jpegParams"></param>
        /// <param name="frame"></param>
        public void Encode(NativePixelData oldPixelData, NativePixelData newPixelData, NativeJpegParameters jpegParams, int frame)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pixelData"></param>
        /// <returns></returns>
        public int ScanHeaderForPrecision(NativePixelData pixelData)
        {
            throw new System.NotImplementedException();
        }
    }
}
