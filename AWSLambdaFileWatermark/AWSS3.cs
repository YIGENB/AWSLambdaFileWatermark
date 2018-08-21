using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AWSLambdaFileWatermark
{
    public class AWSS3
    {
        private static string bucketName = Constants.bucketName;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.CNNorthWest1; //Your bucket region
        public static AWSCredentials awsCredentials = new BasicAWSCredentials(Constants.accessKey, Constants.secretKey);


        private static IAmazonS3 client;

        public AWSS3()
        {
            Console.WriteLine("Aws S3 servisine dosya atan .Net Core Console Uygulaması Başladı");

            client = new AmazonS3Client(awsCredentials, bucketRegion);
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="resimPath">相对路径</param>
        /// <returns></returns>
        public async Task WritingAnObjectAsync(string keyName, string resimPath)
        {
            if (string.IsNullOrEmpty(resimPath))
                return;

            try
            {
                string path =Path.Combine(Directory.GetCurrentDirectory(), resimPath);
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    CannedACL = S3CannedACL.PublicRead,
                    FilePath = path
                };
                PutObjectResponse response = await client.PutObjectAsync(putRequest);

                Console.WriteLine("İşlem Başarı ile gerçekleştirildi");

            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                        "Error encountered ***. Message:'{0}' when writing an object"
                        , e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'{0}' when writing an object"
                    , e.Message);
            }
        }

        /// <summary>
        /// 文件流上传
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public async Task WritingAnObjectAsync(string keyName, Stream inputStream)
        {
            try
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    CannedACL = S3CannedACL.PublicRead,
                    InputStream = inputStream
                };
                PutObjectResponse response = await client.PutObjectAsync(putRequest);

                Console.WriteLine("is ok");

            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                        "Error encountered ***. Message:'{0}' when writing an object"
                        , e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'{0}' when writing an object"
                    , e.Message);
            }
        }


        /// <summary>
        /// 从S3下载文件
        /// </summary>
        /// <param name="keyName">文件名</param>
        /// <param name="fileName">保存文件名</param>
        /// <returns></returns>
        public async Task DownloadObjectAsync(string keyName,string fileName)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            IDictionary<string, object> additionalProperties
                = new Dictionary<string, object>();

            try
            {
                await client.DownloadToFilePathAsync(bucketName, keyName, path, additionalProperties);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                        "Error encountered ***. Message:'{0}' when writing an object"
                        , e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'{0}' when writing an object"
                    , e.Message);
            }
        }

        /// <summary>
        /// 从S3下载文件
        /// </summary>
        /// <param name="keyName">文件名</param>
        /// <returns></returns>
        public async Task<Stream> DownloadObjectAsync(string keyName)
        {
            IDictionary<string, object> additionalProperties
                = new Dictionary<string, object>();
            try
            {
                var rest=await client.GetObjectStreamAsync(bucketName, keyName, additionalProperties);
                return rest;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                        "Error encountered ***. Message:'{0}' when writing an object"
                        , e.Message);

                return null;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task DeleteAnObjectAsync(string key)
        {
            try
            {
                // 1. Delete object-specify only key name for the object.
                var deleteRequest1 = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                };

                DeleteObjectResponse response1 = await client.DeleteObjectAsync(deleteRequest1);

                Console.WriteLine("Silme İşlem Başarı ile gerçekleştirildi");

            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                        "Error encountered ***. Message:'{0}' when writing an object"
                        , e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'{0}' when writing an object"
                    , e.Message);
            }
        }
    }
}
