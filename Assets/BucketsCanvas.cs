using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.S3;
using UnityEngine;
using UnityEngine.UI;
using Amazon.Runtime.Credentials;
using Amazon.Runtime;
using Amazon;

public class BucketsCanvas : MonoBehaviour
{
    public string aws_access_key_id = "aws_access_key_id";
    public string aws_secret_access_key = "aws_secret_access_key";

    public GameObject BucketCountText;

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
        Debug.unityLogger.Log("Button Clicked");

        var credentials = new BasicAWSCredentials(aws_access_key_id, aws_secret_access_key);
        var client = new AmazonS3Client(credentials, RegionEndpoint.USWest2);
        var buckets = await client.ListBucketsAsync();

        BucketCountText.GetComponent<Text>().text = $"Buckets count: {buckets.Buckets.Count}";
    }
}
