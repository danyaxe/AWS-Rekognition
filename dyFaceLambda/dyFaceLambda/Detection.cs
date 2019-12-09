using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace dyFaceLambda
{
    public class Detection
    {

        /// <summary>
        /// This function compares the input image with the existing ones in the dyFace Collection
        /// Function Name: "dyFaceRekognition"
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<bool> RekognizeFace(string input, ILambdaContext context)
        {
            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient();

            Image image = new Image()
            {
                S3Object = new S3Object()
                {
                    Bucket = "dyface-test-bucket",
                    Name = input
                }
            };

            SearchFacesByImageRequest searchFaceRequest = new SearchFacesByImageRequest()
            {
                CollectionId = "dyFaceCollection",
                Image = image,
                FaceMatchThreshold = 70F,
                MaxFaces = 1
            };

            SearchFacesByImageResponse searchFaceResponse = await rekognitionClient.SearchFacesByImageAsync(searchFaceRequest);

            foreach (FaceMatch face in searchFaceResponse.FaceMatches)
            {
                
                if (face.Similarity>=95)
                {
                    return true;
                }
            }
            //missing error handling - image does't exist etc
            return false;
        }

        /// <summary>
        /// This function add images to the dyFace Collection
        /// Function Name: "dyFaceRekognitionAddToCollection"
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<List<string>> AddToCollection(string input, ILambdaContext context)
        {
            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient();

            String collectionId = "dyFaceCollection";
            List<string> collectionIds = await GetCollections(rekognitionClient);
            List<string> reasons = new List<string>();

            //error handling
            if (!collectionIds.Contains(collectionId))
            {
                int createCollectionResponse = await CreateCollection(rekognitionClient, collectionId);
                if (createCollectionResponse < 200 || createCollectionResponse > 299)
                {
                    reasons.Add("Failure Creating Collection = " + createCollectionResponse);
                    return reasons;
                }
            }

            IndexFacesRequest indexFacesRequest = new IndexFacesRequest()
            {
                Image = {
                    S3Object = new S3Object()
                    {
                        Bucket = "dyface-test-bucket",
                        Name = input
                    }
                },
                CollectionId = collectionId,
                ExternalImageId = input,
                DetectionAttributes = new List<String>() { "ALL" }
            };

            IndexFacesResponse indexFacesResponse = await rekognitionClient.IndexFacesAsync(indexFacesRequest);
            
            foreach (UnindexedFace unindexedFace in indexFacesResponse.UnindexedFaces)
            {
                reasons.Add(unindexedFace.Reasons.ToString());
            }

            return reasons;
        }

        private async Task<int> CreateCollection(AmazonRekognitionClient rekognitionClient, String collectionId)
        {
            CreateCollectionRequest createCollectionRequest = new CreateCollectionRequest()
            {
                CollectionId = collectionId
            };

            CreateCollectionResponse createCollectionResponse = await rekognitionClient.CreateCollectionAsync(createCollectionRequest);

            return createCollectionResponse.StatusCode;
        }

        private async Task<List<string>> GetCollections(AmazonRekognitionClient rekognitionClient)
        {
            ListCollectionsResponse listCollectionsResponse = null;
            ListCollectionsRequest listCollectionsRequest = new ListCollectionsRequest()
            {
                //MaxResults = limit, //declare a limit
                //NextToken = paginationToken
            };

            listCollectionsResponse = await rekognitionClient.ListCollectionsAsync(listCollectionsRequest);

            return listCollectionsResponse.CollectionIds;
        }

    }
}
