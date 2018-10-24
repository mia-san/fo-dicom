// Copyright (c) 2012-2018 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

using Dicom.Imaging.Codec.Jpeg;

namespace Dicom.Imaging.Codec
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DicomJpegNativeCodec : DicomJpegCodec
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bits"></param>
        /// <param name="jparams"></param>
        /// <returns></returns>
        protected abstract IJpegNativeCodec GetCodec(int bits, DicomJpegParams jparams);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldPixelData"></param>
        /// <param name="newPixelData"></param>
        /// <param name="parameters"></param>
        public override void Encode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomCodecParams parameters)
        {
            if (0 < oldPixelData.NumberOfFrames)
            {
                // IJG eats the extra padding bits. Is there a better way to test for this?
                if (oldPixelData.BitsAllocated == 16 && oldPixelData.BitsStored <= 8)
                {
                    // check for embedded overlays?
                    newPixelData.Dataset.AddOrUpdate(DicomTag.BitsAllocated, (ushort)8);
                }

                var jparams = parameters as DicomJpegParams ?? GetDefaultParameters() as DicomJpegParams;

                var codec = GetCodec(oldPixelData.BitsStored, jparams);

                for (var frame = 0; frame < oldPixelData.NumberOfFrames; frame++)
                {
                    codec.Encode(oldPixelData, newPixelData, jparams, frame);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldPixelData"></param>
        /// <param name="newPixelData"></param>
        /// <param name="parameters"></param>
        public override void Decode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomCodecParams parameters)
        {
            if (0 < oldPixelData.NumberOfFrames)
            {
                // IJG eats the extra padding bits. Is there a better way to test for this?
                if (newPixelData.BitsAllocated == 16 && newPixelData.BitsStored <= 8)
                {
                    // check for embedded overlays here or below?
                    newPixelData.Dataset.AddOrUpdate(DicomTag.BitsAllocated, (ushort)8);
                }

                var jparams = parameters as DicomJpegParams ?? GetDefaultParameters() as DicomJpegParams;

                int precision = this.QueryPrecision(oldPixelData);

                if (newPixelData.BitsStored <= 8 && 8 < precision)
                    newPixelData.Dataset.AddOrUpdate(DicomTag.BitsAllocated, (ushort)16); // embedded overlay?

                var codec = GetCodec(precision, jparams);

                for (var frame = 0; frame < oldPixelData.NumberOfFrames; frame++)
                {
                    codec.Decode(oldPixelData, newPixelData, jparams, frame);
                }
            }
        }

        /// <summary>
        /// query precision by one way or another.
        /// </summary>
        /// <param name="pixelData"></param>
        /// <returns></returns>
        private int QueryPrecision(DicomPixelData pixelData)
        {
            try
            {
                return JpegHelper.ScanJpegForBitDepth(pixelData);
            }
            catch
            {
            }

            try
            {
                // if the internal scanner chokes on an image, try again using ijg
                return new Jpeg12Codec(JpegMode.Baseline, 0, 0).ScanHeaderForPrecision(pixelData);
            }
            catch
            {
            }

            // the old scanner choked on several valid images...
            // assume the correct encoder was used and let libijg handle the rest
            return pixelData.BitsStored;
        }
    }
}
