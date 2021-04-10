using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.S3;
using UnityEngine;
using UnityEngine.UI;
using Amazon.Runtime.Credentials;
using Amazon.Runtime;
using Amazon;
using System.IO;
using Amazon.S3.Model;
using System;

public class BucketsCanvas : MonoBehaviour
{
    private string aws_access_key_id = "aws_access_key_id";
    private string aws_secret_access_key = "aws_secret_access_key";

    public GameObject BucketCountText;
    public GameObject CURDLogsText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void GetBucketsCountButtonClicked()
    {
        Debug.unityLogger.Log("Get Buckets Button Clicked");

        var credentials = new BasicAWSCredentials(aws_access_key_id, aws_secret_access_key);
        using var client = new AmazonS3Client(credentials, RegionEndpoint.USWest2);
        var buckets = await client.ListBucketsAsync();

        BucketCountText.GetComponent<Text>().text = $"Buckets count: {buckets.Buckets.Count}";
        Debug.unityLogger.Log($"Buckets count: {buckets.Buckets.Count}");
    }

    public async void UploadFileButtonClicked()
    {
        Debug.unityLogger.Log("Upload File Button Clicked");
        var credentials = new BasicAWSCredentials(aws_access_key_id, aws_secret_access_key);

        var bucketName = $"unity-test-{DateTime.UtcNow.Ticks}";
        var fileName = "file.txt";

        using var client = new AmazonS3Client(credentials, RegionEndpoint.USWest2);
        var putBucketRequest = new PutBucketRequest()
        {
            BucketName = bucketName
        };
        var putBucketResponse = await client.PutBucketAsync(putBucketRequest);
        Debug.unityLogger.Log($"PutBucket: {putBucketResponse.HttpStatusCode}");
        CURDLogsText.GetComponent<Text>().text = $"Put Bucket: {bucketName}";

        var filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        if (filePath.Contains("://"))
        {
            WWW www = new WWW(filePath);

            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fileName,
                ContentBody = www.text
            };

            var putObjectResponse = await client.PutObjectAsync(putObjectRequest);
            Debug.unityLogger.Log($"PutObject: {putObjectResponse.HttpStatusCode}");
            CURDLogsText.GetComponent<Text>().text += $"{Environment.NewLine}Put Object: {fileName}";
        }
        else
        {
            using var filestream = new FileStream(Path.Combine(Application.streamingAssetsPath, fileName), FileMode.Open);

            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fileName,
                InputStream = filestream
            };

            var putObjectResponse = await client.PutObjectAsync(putObjectRequest);
            Debug.unityLogger.Log($"PutObject: {putObjectResponse.HttpStatusCode}");
            CURDLogsText.GetComponent<Text>().text += $"{Environment.NewLine}Put Object: {fileName}";
        }

        var getObjectRequest = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = fileName
        };
        var getObjectResponse = await client.GetObjectAsync(getObjectRequest);
        Debug.unityLogger.Log($"Get Object: {getObjectResponse.HttpStatusCode}");
        CURDLogsText.GetComponent<Text>().text += $"{Environment.NewLine}Get Object: {fileName}";

        using var reader = new StreamReader(getObjectResponse.ResponseStream);
        var content = reader.ReadToEnd();
        Debug.unityLogger.Log($"Object: {content}");
        CURDLogsText.GetComponent<Text>().text += $"{Environment.NewLine}Get Object Content: {content}";


        var deleteObjectRequest = new DeleteObjectRequest()
        {
            BucketName = bucketName,
            Key = fileName
        };
        var deleteObjectResponse = await client.DeleteObjectAsync(deleteObjectRequest);
        Debug.unityLogger.Log($"Delete Object: {getObjectResponse.HttpStatusCode}");
        CURDLogsText.GetComponent<Text>().text += $"{Environment.NewLine}Delete Object: {fileName}";

        var deleteBucketRequest = new DeleteBucketRequest()
        {
            BucketName = bucketName,
        };
        var deleteBucketResponse = await client.DeleteBucketAsync(deleteBucketRequest);
        Debug.unityLogger.Log($"Delete Bucket: {deleteBucketResponse.HttpStatusCode}");
        CURDLogsText.GetComponent<Text>().text += $"{Environment.NewLine}Delete Bucket: {bucketName}";
    }
}
