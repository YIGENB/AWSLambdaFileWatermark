using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AWSLambdaFileWatermark
{
    public class Common
    {
        /// <summary>
        /// 从S3下载并加上水印并返回下载地址
        /// </summary>
        /// <param name="keyName">S3文件名</param>
        /// <param name="waterMarkName">水印文本名</param>
        public static string ConverPDFByS3Download(string keyName, string waterMarkName)
        {
            var fileSuffix = Path.GetExtension(keyName);

            var download = new AWSS3().DownloadObjectAsync(keyName);
            var s3File = download.Result;
            download.Wait();

            string outPath = Path.Combine(Directory.GetCurrentDirectory(), @"temp");

            if (!Directory.Exists(outPath))
                Directory.CreateDirectory(outPath);

            string outFilePath = Path.Combine(outPath, string.Format("{0}{1}.{2}", waterMarkName, Guid.NewGuid().ToString(), fileSuffix));

            string domainPath = string.Format("{0}/temp/{1}{2}.{3}", Constants.domain, waterMarkName, Guid.NewGuid().ToString(), fileSuffix);

            if (fileSuffix == ".pdf")
                ConvertPDF.SetWatermark(s3File, outFilePath, waterMarkName);
            else
                StreamToFile(s3File, outFilePath);

            return domainPath;
        }

        /// <summary>
        /// 从S3下载并加上水印并返回流
        /// </summary>
        /// <param name="keyName">S3文件名</param>
        /// <param name="waterMarkName">水印文本名</param>
        public static Stream ConverPDFByStream(string keyName, string waterMarkName)
        {
            var fileSuffix = Path.GetExtension(keyName);

            var download = new AWSS3().DownloadObjectAsync(keyName);
            var s3File = download.Result;
            download.Wait();

            if (fileSuffix == ".pdf")
                return ConvertPDF.SetWatermark(s3File, waterMarkName);

            return null;
        }
        /// <summary> 
        /// 将 Stream 写入文件 
        /// </summary> 
        private static void StreamToFile(Stream stream, string fileName)
        {
            try
            {
                // 把 Stream 转换成 byte[] 
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                // 设置当前流的位置为流的开始 ,   aws对流还需要进一步处理
                stream.Seek(0, SeekOrigin.Begin);

                // 把 byte[] 写入文件 
                FileStream fs = new FileStream(fileName, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(bytes);
                bw.Close();
                fs.Close();
            }
            catch
            {

            }

        }
    }
}
