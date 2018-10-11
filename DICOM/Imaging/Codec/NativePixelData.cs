using System;

namespace Dicom.Imaging.Codec
{
    /// <summary>
    /// borrowed from
    /// Native\Universal\Dicom.Imaging.Codec.PixelData.h
    /// Native\Universal\Dicom.Imaging.Codec.PixelData.cpp
    /// </summary>
    /// <param name="buffer"></param>
    public delegate void AddFrameDelegate(byte[] buffer);

    /// <summary>
    ///
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public delegate byte[] GetFrameDelegate(int index);

    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    public delegate void SetPhotometricInterpretationDelegate(string value);

    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    public delegate void SetPlanarConfigurationDelegate(int value);

    /// <summary>
    ///
    /// </summary>
    public class NativePixelData
    {
        /// <summary>
        ///
        /// </summary>
        public GetFrameDelegate GetFrameImpl { get; set; }

        /// <summary>
        ///
        /// </summary>
        public AddFrameDelegate AddFrameImpl { get; set; }

        /// <summary>
        ///
        /// </summary>
        public SetPlanarConfigurationDelegate SetPlanarConfigurationImpl { get; set; }

        /// <summary>
        ///
        /// </summary>
        public SetPhotometricInterpretationDelegate SetPhotometricInterpretationImpl { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int BitsAllocated { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int BitsStored { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int BytesAllocated { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int HighBit { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int NumberOfFrames { get; set; }

        /// <summary>
        ///
        /// </summary>
        private string _photometricInterpretation;

        /// <summary>
        ///
        /// </summary>
        public string PhotometricInterpretation
        {
            get
            {
                return this._photometricInterpretation;
            }
            set
            {
                this.SetPhotometricInterpretationImpl?.Invoke(value);
                this._photometricInterpretation = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public int PixelRepresentation { get; set; }

        /// <summary>
        ///
        /// </summary>
        private int _planarConfiguration;

        /// <summary>
        ///
        /// </summary>
        public int PlanarConfiguration
        {
            get
            {
                return this._planarConfiguration;
            }
            set
            {
                this.SetPlanarConfigurationImpl?.Invoke(value);
                this._planarConfiguration = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public int SamplesPerPixel { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool TransferSyntaxIsLossy { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int UncompressedFrameSize { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte[] GetFrame(int index)
        {
            return (this.GetFrameImpl == null)
                ? throw new NullReferenceException("GetFrame delegate not defined")
                : this.GetFrameImpl(index);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="buffer"></param>
        public void AddFrame(byte[] buffer)
        {
            if (this.AddFrameImpl == null)
            {
                throw new NullReferenceException("AddFrame delegate not defined");
            }
            else
            {
                this.AddFrameImpl(buffer);
            }
        }

        /// <summary>
        /// RRR..., GGG..., BBB... to RGB, RGB, RGB...
        /// </summary>
        /// <param name="oldPixels"></param>
        /// <returns></returns>
        public static byte[] InterleavedToPlanar24(byte[] oldPixels)
        {
            byte[] newPixels = new byte[oldPixels.Length];

            int pixelCount = newPixels.Length / 3;
            for (int n = 0; n < pixelCount; n++)
            {
                newPixels[n + (pixelCount * 0)] = oldPixels[(n * 3) + 0];
                newPixels[n + (pixelCount * 1)] = oldPixels[(n * 3) + 1];
                newPixels[n + (pixelCount * 2)] = oldPixels[(n * 3) + 2];
            }

            return newPixels;
        }

        /// <summary>
        /// RGB, RGB, RGB... to RRR..., GGG..., BBB...
        /// </summary>
        /// <param name="oldPixels"></param>
        /// <returns></returns>
        public static byte[] PlanarToInterleaved24(byte[] oldPixels)
        {
            byte[] newPixels = new byte[oldPixels.Length];

            int pixelCount = newPixels.Length / 3;
            for (int n = 0; n < pixelCount; n++)
            {
                newPixels[(n * 3) + 0] = oldPixels[n + (pixelCount * 0)];
                newPixels[(n * 3) + 1] = oldPixels[n + (pixelCount * 1)];
                newPixels[(n * 3) + 2] = oldPixels[n + (pixelCount * 2)];
            }

            return newPixels;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <remarks>unpack?</remarks>
        public static byte[] UnpackLow16(byte[] data)
        {
            byte[] bytes = new byte[data.Length / 2];

            for (int i = 0; i < bytes.Length && (i * 2) < data.Length; i++)
            {
                bytes[i] = data[i * 2];
            }

            return bytes;
        }
    };
}
