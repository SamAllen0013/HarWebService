using SamAllen_Rigor_Challenge.Helpers;
using SamAllen_Rigor_Challenge.Models;
using SamAllen_Rigor_Challenge.Utilities;
using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Http;

namespace SamAllen_Rigor_Challenge.Controllers
{
    public class HarChallengeController : ApiController
    {
        [HttpPost, Route("api/CreateHarFile")]
        public object CreateHarFile()
        {
            HttpRequest httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count == 1)
            {
                try
                {
                    FileHelper fileHelper = new FileHelper();
                    object jsonObject = fileHelper.GetJsonFromFile(httpRequest.Files[0]);
                    NameValueCollection formVariables = httpRequest.Form;
                    string fileName = formVariables["FileName"];
                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        fileName = httpRequest.Files[0].FileName;
                    }
                    CacheHelper cacheHelper = new CacheHelper();
                    cacheHelper.AddCache(jsonObject, fileName);
                    return JsonBuilder.BuildJsonResponse("Success, the file was written with the following key: " + fileName);
                }
                catch (Exception e)
                {
                    return JsonBuilder.BuildJsonResponse(e.Message);
                }
            }
            else if (httpRequest.Files.Count > 1)
            {
                return JsonBuilder.BuildJsonResponse("Only supports adding one file");
            }
            else
            {
                return JsonBuilder.BuildJsonResponse("No files in request");
            }
        }

        [HttpPost, Route("api/GetHarFile")]
        public object GetHarFile([FromBody]HarChallengeModel harModel)
        {
            try
            {
                CacheHelper cacheHelper = new CacheHelper();
                object objectFromCache = cacheHelper.GetObjectFromCache(harModel.fileKeyName);
                if (objectFromCache == null)
                {
                    return JsonBuilder.BuildJsonResponse("Item does not exist");
                }
                else
                {
                    return JsonBuilder.BuildJsonResponse(objectFromCache);
                }
            }
            catch (Exception e)
            {
                return JsonBuilder.BuildJsonResponse(e.Message);
            }
        }

        [HttpPost, Route("api/UpdateHarFile")]
        public object UpdateHarFile()
        {
            HttpRequest httpRequest = HttpContext.Current.Request;            
            if (httpRequest.Files.Count == 1)
            {
                NameValueCollection formVariables = httpRequest.Form;
                string fileName = formVariables["FileName"];
                CacheHelper cacheHelper = new CacheHelper();
                if (cacheHelper.CheckIfItemExists(fileName) == true)
                {
                    try
                    {
                        FileHelper fileHelper = new FileHelper();
                        object jsonObject = fileHelper.GetJsonFromFile(httpRequest.Files[0]);
                        cacheHelper.UpdateCache(jsonObject, fileName);
                        return JsonBuilder.BuildJsonResponse("Success, the file in the following key was updated: " + fileName);
                    }
                    catch (Exception e)
                    {
                        return JsonBuilder.BuildJsonResponse(e.Message);
                    }
                }
                else
                {
                    return JsonBuilder.BuildJsonResponse("Item does not exist");
                }
            }
            else if (httpRequest.Files.Count > 1)
            {
                return JsonBuilder.BuildJsonResponse("Only supports updating one file");
            }
            else
            {
                return JsonBuilder.BuildJsonResponse("No files in request");
            }                       
        }

        [HttpPost, Route("api/DeleteHarFile")]
        public object DeleteHarFile([FromBody]HarChallengeModel harModel)
        {
            CacheHelper cacheHelper = new CacheHelper();
            if (cacheHelper.CheckIfItemExists(harModel.fileKeyName) == true)
            {
                try
                {
                    cacheHelper.RemoveObjectFromCache(harModel.fileKeyName);
                    return JsonBuilder.BuildJsonResponse("Success");
                }
                catch (Exception e)
                {
                    return JsonBuilder.BuildJsonResponse(e.Message);
                }
            }
            else
            {
                return JsonBuilder.BuildJsonResponse("Item does not exist");
            }
        }

        [HttpPost, Route("api/GetHarFileAnalysis")]
        public object GetHarFileAnalysis([FromBody]HarChallengeModel harModel)
        {
            try
            {
                CacheHelper cacheHelper = new CacheHelper();
                object objectFromCache = cacheHelper.GetObjectFromCache(harModel.fileKeyName);
                if (objectFromCache == null)
                {
                    return JsonBuilder.BuildJsonResponse("Item does not exist");
                }
                else
                {
                    FileHelper fileHelper = new FileHelper();
                    return fileHelper.GetHarFileAnalysis(objectFromCache);
                }
            }
            catch (Exception e)
            {
                return JsonBuilder.BuildJsonResponse(e.Message);
            }
        }
    }
}
