using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using Microsoft.WindowsAzure.Storage.Blob;

namespace UploadBlobWebApp
{
    /// <summary>
    /// Summary description for FileUploadHandler
    /// </summary>
    public class FileUploadHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {


            String fileType = "test";


            string stoConnStr ="DefaultEndpointsProtocol=https;AccountName=rangeryuteststo;AccountKey=T4NjPlFcEGaz4c2Da9ZfN4o1CQCOAxLOGxfz5NRFnzLXPW4M971W4cSmXeQ4/DP8SwP8Xc7uLm5eaPrKHwhSTA==";

            var cloudStorageAccount = CloudStorageAccount.Parse(stoConnStr);
            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            IEnumerable<CloudBlobContainer> containers = blobClient.ListContainers();

            

            Boolean exist = false;
            foreach (var item in containers)
            {
                // do your stuff  
                if (item.Name == fileType)
                {
                    exist = true;
                }
            }

            foreach (string file in context.Request.Files)
            {
                HttpPostedFile hpf = context.Request.Files[file] as HttpPostedFile;
                string FileName = string.Empty;
                if (HttpContext.Current.Request.Browser.Browser.ToUpper() == "IE" || HttpContext.Current.Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] files = hpf.FileName.Split(new char[] { '\\' });
                    FileName = files[files.Length - 1];
                }
                else
                {
                    FileName = hpf.FileName;
                }

                Random random = new Random();
                int randomNumber = random.Next(0, 1000000);
                string[] fileVal = FileName.Split('.');


                FileName = Path.GetFileNameWithoutExtension(FileName) 
                    + "_" + randomNumber  + Path.GetExtension(FileName);
 

                string outputMessage = "";
                try
                {
                    var blobContainer = blobClient.GetContainerReference(fileType);

                    if (!blobContainer.Exists())
                    {
                        blobContainer.Create(BlobContainerPublicAccessType.Blob);
                    }
                        
    
                    /*blobContainer.CreateIfNotExist();*/

                    
                    var blob = blobContainer.GetBlockBlobReference(FileName);

                    using (var inputStream = hpf.InputStream)
                    {
                        var optResult = blob.BeginUploadFromStream(inputStream, new AsyncCallback(result =>
                        {
                            blob.EndUploadFromStream(result);
                            Trace.TraceInformation("Upload finished {0} {1}", result.IsCompleted, inputStream.Position);

                        }), blob.Uri);

                        optResult.AsyncWaitHandle.WaitOne();
                        //show result
                    }

                    var blobSvrRef = blobContainer.GetBlobReferenceFromServer(FileName);
                    outputMessage += "Size: " + blobSvrRef.Properties.Length + " Uri: "
                                     + blobSvrRef.StorageUri.PrimaryUri;


                }
                catch (StorageException e)
                {
                    string msg = "StorageException " + e.Message + ";" + e.StackTrace;
                    Trace.TraceWarning(msg);
                    outputMessage += msg;

                }
                catch (Exception e)
                {
                    string msg = e.Message + ";" + e.StackTrace;
                    Trace.TraceWarning(msg);
                    outputMessage += msg;
                }
                finally
                {
                    outputMessage += " request finished @" + DateTime.UtcNow;
                }


                context.Response.AddHeader("Pragma", "no-cache");
                context.Response.AddHeader("Cache-Control", "private, no-cache");
                context.Response.Write(outputMessage);

            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

 
}