namespace Dicom.Imaging.Codec
{
    /// <summary>
    ///
    /// </summary>
    public class Jpeg16Codec : JpegCodecBase
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="predictor"></param>
        /// <param name="pointTransform"></param>
        public Jpeg16Codec(JpegMode mode, int predictor, int pointTransform): base(mode, predictor, pointTransform, 16)
        {
        }
    }
}
