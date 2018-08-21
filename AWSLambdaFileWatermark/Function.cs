using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using AwsLambdaOwin;
using Microsoft.Owin;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AWSLambdaFileWatermark
{
    public class Function: APIGatewayOwinProxyFunction
    {
        public override Func<IDictionary<string, object>, Task> AppFunc { get; }

        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
        /// to respond to S3 notifications.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        //public string FunctionHandler(string input, ILambdaContext context)
        //{
        //    //var keyName_new = "testwriting.pdf";

        //    var res = Common.ConverPDFByStream(input, "»ªÏÄº½¿ÕË®Ó¡²âÊÔ");

        //    //new AWSS3().WritingAnObjectAsync(keyName_new, res).Wait();

        //    return JsonConvert.SerializeObject(new { input, context });
        //}

        public Function()
        {
            // Register any MIME content types you want treated as binary
            //RegisterResponseContentEncodingForContentType(
            //    "application/octet-stream",
            //    ResponseContentEncoding.Base64);

            AppFunc = async env =>
            {
                var context = new LambdaOwinContext(env);
                context.Response.ContentType = "application/octet-stream";
                var res = Common.ConverPDFByStream("test.pdf", "»ªÏÄº½¿ÕË®Ó¡²âÊÔ");

                byte[] bytes = new byte[res.Length];

                await context.Response.WriteAsync(JsonConvert.SerializeObject(context));
                //await context.Response.WriteAsync(new string('a', 100000));
            };
        }
    }
}
