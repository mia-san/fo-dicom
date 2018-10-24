namespace Dicom.Imaging.Codec
{
    /// <summary>
    ///
    /// </summary>
    public class Jpeg12Codec : JpegCodecBase
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="predictor"></param>
        /// <param name="pointTransform"></param>
        public Jpeg12Codec(JpegMode mode, int predictor, int pointTransform): base(mode, predictor, pointTransform, 12)
        {
        }
    }
}
