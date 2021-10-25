using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;
using Amazon.S3.Model;
using ApplicationSubmission.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace ApplicationSubmission.Controllers
{
    [Route("applicationsubmission/[controller]")]
    [ApiController]
    public class ApplicantController : ControllerBase
    {
        Amazon.S3.IAmazonS3 client { get; set; }
        string bucketName { get; set; }

        public ApplicantController()
        {
            client = new AmazonS3Client(Amazon.RegionEndpoint.APSouth1);
            bucketName = "pbloandocuments";
        }

        // GET: applicationsubmission/Applicant/5
        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            this.Response.ContentType = "application/json";
            this.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            string msg; int ApplicantID = 0;
            Applicant appdetail = new Applicant();

            try
            {
                if (int.TryParse(id, out ApplicantID))
                {
                    ApplicantID = Convert.ToInt32(id);
                }

                appdetail = appdetail.Get_ApplicantDetails(ApplicantID, id);

                if (appdetail.Applicant_ID == 0)
                {
                    msg = "No applicant details found.";
                    appdetail.LogMessage("Get Applicant Data ----" + " " + msg.ToString());
                    return new JsonResult(msg, new JsonSerializerSettings { Formatting = Formatting.Indented });
                }
                else
                {
                    var content = JsonConvert.SerializeObject(appdetail);
                    appdetail.LogMessage("Get Applicant Data ----" + " " + content.ToString());

                    return new JsonResult(appdetail, new JsonSerializerSettings { Formatting = Formatting.Indented });

                }
            }
            catch (Exception ex)
            {
                this.Response.StatusCode = 400;
                appdetail.LogMessage(ex.Message.ToString() + " " + ex.Message.ToString());
                return new JsonResult(ex.Message);
            }

        }

        // POST: applicationsubmission/Applicant
        [HttpPost]
        public JsonResult Post([FromBody] Applicant value)
        {
            this.Response.ContentType = "application/json";
            this.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            ResponseBody rb = new ResponseBody();
            Applicant appdetail = new Applicant();

            try
            {
                int result = appdetail.Add_ApplicantInfo(value);

                if (result > 0)
                {
                    rb.ID = result;
                    rb.msg = "Applicant details saved successfully.";

                    var content = JsonConvert.SerializeObject(value);
                    appdetail.LogMessage(rb.msg + " " + content.ToString());
                }
                else
                {
                    rb.ID = result;
                    rb.msg = "Applicant details not saved.";

                    var content = JsonConvert.SerializeObject(value);
                    appdetail.LogMessage(rb.msg + " " + content.ToString());
                }

                return new JsonResult(rb, new JsonSerializerSettings { Formatting = Formatting.Indented });
            }
            catch (Exception ex)
            {
                this.Response.StatusCode = 400;
                appdetail.LogMessage(ex.Message.ToString() + " " + ex.Message.ToString());
                return new JsonResult(ex.Message);
            }

        }

        // PUT: applicationsubmission/Applicant/5
        [HttpPut("{id}")]
        public JsonResult Put(int id, [FromBody] Applicant value)
        {
            this.Response.ContentType = "application/json";
            this.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            string msg = "";
            Applicant appdetail = new Applicant();
            try
            {
                bool result = appdetail.Update_ApplicantInfo(value, id);

                if (result == true)
                {
                    msg = "Applicant details updated successfully.";
                    var content = JsonConvert.SerializeObject(value);
                    appdetail.LogMessage(msg + " " + content.ToString());
                }
                else
                {
                    msg = "Applicant details not updated.";
                    var content = JsonConvert.SerializeObject(value);
                    appdetail.LogMessage(msg + " " + content.ToString());
                }


                return new JsonResult(msg, new JsonSerializerSettings { Formatting = Formatting.Indented });

            }
            catch (Exception ex)
            {
                this.Response.StatusCode = 400;
                appdetail.LogMessage(ex.Message.ToString() + " " + ex.Message.ToString());
                return new JsonResult(ex.Message);
            }

        }

        // DELETE: applicationsubmission/ApiWithActions/5
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            this.Response.ContentType = "application/json";
            this.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            string msg = "";
            Applicant appdetail = new Applicant();

            try
            {
                bool result = appdetail.Delete_ApplicantInfo(id);

                if (result == true)
                {
                    msg = "Applicant details deleted successfully.";
                    appdetail.LogMessage(msg + " " + id.ToString());
                }
                else
                {
                    msg = "Applicant details not deleted.";
                    appdetail.LogMessage(msg + " " + id.ToString());
                }


                return new JsonResult(msg, new JsonSerializerSettings { Formatting = Formatting.Indented });
            }
            catch (Exception ex)
            {
                this.Response.StatusCode = 400;
                appdetail.LogMessage(ex.Message.ToString() + " " + ex.Message.ToString());
                return new JsonResult(ex.Message);
            }

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

        private async Task<System.Net.HttpStatusCode> PutFileOnBucket(int id, IFormFile[] file)
        {
            PutObjectResponse response = null;
            foreach (IFormFile myfile in file)
            {
                string content = await ReadFormFileAsync(myfile);
                if (content == null)

                    return System.Net.HttpStatusCode.BadRequest;

                response = new PutObjectResponse();
                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = id.ToString() + "/" + myfile.FileName,
                    ContentBody = content
                };
                response = await client.PutObjectAsync(request);
            }
            //Send the correct response return for each file.
            return response.HttpStatusCode;
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

        public async Task GetDocument(int id, string filename)
        {
            try
            {
                var getResponse = await client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = id.ToString() + "/" + filename
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
    }
}
