using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Amazon;
using System.Net.Http;

namespace ApplicationSubmission.Controllers
{
    [Route("applicationsubmission/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        Amazon.S3.IAmazonS3 client { get; set; }

        string bucketName { get; set; }

        public DocumentController()
        {
            string RegionName = Environment.GetEnvironmentVariable("LoanDocumentBucketRegion");
            client = new AmazonS3Client(RegionEndpoint.GetBySystemName(RegionName));
            //bucketName = "pbloandocuments";
            bucketName = Environment.GetEnvironmentVariable("LoanDocumentBucket");
        }

        // GET: applicationsubmission/Document/5
        [HttpGet("{loanid}")]
        public async Task Get(int loanid, string filename)
        {
            this.Response.ContentType = "application/json";
            this.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            try
            {
                var getResponse = await client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = loanid.ToString() + "/" + filename
                });

                this.Response.ContentType = getResponse.Headers.ContentType;
                getResponse.ResponseStream.CopyTo(this.Response.Body);
            }
            catch (AmazonS3Exception e)
            {
                this.Response.StatusCode = (int)e.StatusCode;
                var writer = new StreamWriter(this.Response.Body);
                writer.Write(e.Message);
            }
        }

        public async Task<string> ReadFormFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return await Task.FromResult((string)null);
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private async Task<System.Net.HttpStatusCode> PutFileOnBucket(int loanid, IFormFile[] myfile)
        {
            PutObjectResponse response = null;
            foreach (IFormFile file in myfile)
            {
                string content = await ReadFormFileAsync(file);
                if (content == null)
                    return System.Net.HttpStatusCode.BadRequest;

                response = new PutObjectResponse();
                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = loanid.ToString() + "/" + file.FileName,
                    ContentBody = content
                };
                response = await client.PutObjectAsync(request);
            }
            //Send the correct response return for each file.
            return response.HttpStatusCode;
        }

        // POST: applicationsubmission/Document
        [HttpPost("{loanid}")]
        public async Task<System.Net.HttpStatusCode> Post(int loanid, IFormFile[] myfile)
        {
            this.Response.ContentType = "application/json";
            this.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            return await PutFileOnBucket(loanid, myfile);
        }

        // PUT: applicationsubmission/Document/5
        [HttpPut("{loanid}")]
        public async Task<System.Net.HttpStatusCode> Put(int loanid, IFormFile[] myfile)
        {
            this.Response.ContentType = "application/json";
            this.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            return await PutFileOnBucket(loanid, myfile);
        }

        // DELETE: applicationsubmission/ApiWithActions/5
        [HttpDelete("{loanid}")]
        public async Task<System.Net.HttpStatusCode> Delete(int loanid, string filename)
        {
            this.Response.ContentType = "application/json";
            this.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            DeleteObjectResponse response = new DeleteObjectResponse();
            DeleteObjectRequest request = new DeleteObjectRequest

            {
                BucketName = bucketName,
                Key = loanid.ToString() + "/" + filename
            };

            // Issue request
            response = await client.DeleteObjectAsync(request);
            return response.HttpStatusCode;
        }

        // Options: applicationsubmission/ApiWithActions/5
        [HttpDelete("{id}")]
        public JsonResult Options(int id)
        {
            try
            {
                HttpResponseMessage res = new HttpResponseMessage();

                this.Response.ContentType = "application/json";
                this.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                return new JsonResult(res, new JsonSerializerSettings { Formatting = Formatting.Indented });
            }
            catch (Exception ex)
            {
                this.Response.StatusCode = 400;
                return new JsonResult(ex.Message);
            }

        }
    }
}
