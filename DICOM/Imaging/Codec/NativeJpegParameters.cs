namespace Dicom.Imaging.Codec
{
    /// <summary>
    /// borrowed from
    /// Native\Universal\Dicom.Imaging.Codec.JpegParameters.h
    /// Native\Universal\Dicom.Imaging.Codec.JpegParameters.cpp
    /// </summary>
    public class NativeJpegParameters
    {
        /// <summary>
        ///
        /// </summary>
        public bool ConvertColorspaceToRGB { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int PointTransform { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int Predictor { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int Quality { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int SampleFactor { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int SmoothingFactor { get; set; }

        /// <summary>
        ///
        /// </summary>
        public NativeJpegParameters()
        {
            this.ConvertColorspaceToRGB = false;
            this.PointTransform = 0;
            this.Predictor = 1;
            this.Quality = 90;
            this.SampleFactor = (int)DicomJpegSampleFactor.SF444;
            this.SmoothingFactor = 0;
        }
    };
}
