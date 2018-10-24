using System;
using System.Runtime.InteropServices;
using Dicom.IO.Buffer;


namespace Dicom.Imaging.Codec
{
    /// <summary>
    ///
    /// </summary>
    public class Jpeg8Codec : JpegCodecBase
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="predictor"></param>
        /// <param name="pointTransform"></param>
        public Jpeg8Codec(JpegMode mode, int predictor, int pointTransform) : base(mode, predictor, pointTransform, 8)
        {
        }
    }
}
