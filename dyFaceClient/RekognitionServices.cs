using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.S3;
using Amazon.S3.Model;


namespace dyFaceClient
{
    public class RekognitionServices
    {
        private const string bucketName = "dyface-test-bucket";
        private readonly string accessKey;
        private readonly string secretKey;

        public RekognitionServices(string accessKey, string secretKey)
        {
            this.accessKey = accessKey;
            this.secretKey = secretKey;
        }

        public async Task<string> RegisterUser(string filePath, string fileName)
        {
            //RekognitionRegisterUserResponse response = new RekognitionRegisterUserResponse();
            //response.ResponseCode = 
            AmazonLambdaClient amazonLambdaClient =
                new AmazonLambdaClient(accessKey, secretKey, Amazon.RegionEndpoint.EUCentral1);
            AmazonS3Client s3Client = 
                new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.EUCentral1);

            await UploadToS3(filePath, fileName);

            InvokeRequest lambdaRequest = new InvokeRequest
            {
                InvocationType = InvocationType.RequestResponse,
                FunctionName = "dyFaceRekognitionAddToCollection",
                Payload = "\"" + fileName + "\""
            };

            var result = await amazonLambdaClient.InvokeAsync(lambdaRequest);
            var lambdaResponse = Encoding.ASCII.GetString(result.Payload.ToArray());

            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = fileName
            };

            await s3Client.DeleteObjectAsync(deleteObjectRequest);


            if (!lambdaResponse.Any())
                return "Your image was uploaded successfully";
            else
                return "Your image couldn't be uploaded due to some errors: \r\n"
                    + lambdaResponse;
        }

        public async Task<bool> IsValidUser(string filePath, string fileName)
        {
            RekognitionIsValidUserResponse response = new RekognitionIsValidUserResponse
            {
                ResponseCode = await UploadToS3(filePath, fileName)
            };

            AmazonLambdaClient amazonLambdaClient = 
                new AmazonLambdaClient(accessKey, secretKey, Amazon.RegionEndpoint.EUCentral1);

            InvokeRequest lambdaRequest = new InvokeRequest
            {
                InvocationType = InvocationType.RequestResponse,
                FunctionName = "dyFaceRekognition",
                Payload = "\"" + fileName + "\""
            };
            //remove file

            var result = await amazonLambdaClient.InvokeAsync(lambdaRequest);

            var lambdaResponse = Encoding.ASCII.GetString(result.Payload.ToArray());

            //include error handling 
            if(bool.TryParse(lambdaResponse, out bool returnValue))
            {
                return returnValue;
            }
            //return error message saying that there was an issue
            return false;
        }

        private async Task<int> UploadToS3(string filePath, string fileName)
        {
            var s3Client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.EUCentral1);

            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fileName,
                FilePath = filePath,
                ContentType = "text/plain"
            };

            //include error handling
            //try
            //{
            PutObjectResponse response = await s3Client.PutObjectAsync(putRequest);
            /*
            RekognitionUploadToS3Response responseS3 = new RekognitionUploadToS3Response()
            {
                ResponseCode = (int)response.HttpStatusCode,
                ResponseDescription = ""
            };
            */

            //return responseS3.ResponseCode;
            return (int)response.HttpStatusCode;
            //}
            //catch(Exception ee) {

            //}
        }

    }
}
