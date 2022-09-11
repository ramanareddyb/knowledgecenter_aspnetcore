using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Description;
using ExcelicareAPIGateWay.Filters;
using Newtonsoft.Json.Linq;


namespace ExcelicareAPIGateWay.Controllers
{
    /// <summary>
    /// apigateway for Excelicare api's
    /// created by: BRR
    /// created datetime: 25-June-2022 12:55PM
    /// </summary>
           
    //[EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]
    public class ApigatewayController : ApiController
    {
        string strApiUrl = ConfigurationManager.AppSettings["apiservice_url1"];    

        /// <summary>
        /// all user details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/user")]
        //[BasicAuthentication]
        //[ApiKeyAuthenticationFilter]
        [TokenAuthenticationFilter]
        //[ExceptionHandler]
        //[RequestValidation]
        //[Throttle("GetListOfUsers", "", 10)]
        //[EnableCors]
        public IHttpActionResult GetListofUsers()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(strApiUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/user").Result;
                if (response.IsSuccessStatusCode)
                {
                    //var data = response.Content.ReadAsAsync<IEnumerable<apigatewayController>>().Result;                    

                    JArray jsonArray = JArray.Parse(response.Content.ReadAsStringAsync().Result);

                    return Ok(jsonArray);
                }
                else
                {                    
                    return Ok(Request.CreateResponse(HttpStatusCode.BadRequest, "Some thing went wrong. please contact administrator."));
                }
                
            }
            catch (Exception ex) { throw ex; }
            finally { }           
        }

        /// <summary>
        /// fetching user details for the given user id
        /// </summary>
        /// <param name="usertype">mandatory</param>
        /// <param name="userid">mandatory</param>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/user/userdetails/{usertype}/{userid}")]
       //[TokenAuthenticationFilter]
        //[ExceptionHandler]
        //[RequestValidation]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult GetUserDetails(int usertype, long userid)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(strApiUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/user/userdetails/" + usertype + "/" + userid ).Result;
                if (response.IsSuccessStatusCode)
                {
                    //var data = response.Content.ReadAsAsync<IEnumerable<apigatewayController>>().Result;                    

                    JArray jsonArray = JArray.Parse(response.Content.ReadAsStringAsync().Result);

                    return Ok(jsonArray);
                }
                else
                {
                    return Ok(Request.CreateResponse(HttpStatusCode.BadRequest, "Some thing went wrong. please contact administrator."));
                }

            }
            catch (Exception ex) { throw ex; }
            finally { }
        }

        /// <summary>
        /// save user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("apigateway/user/register")]
        public IHttpActionResult SaveUser(object general)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(strApiUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
               
                HttpResponseMessage response = client.GetAsync("api/user/register").Result;
                if (response.IsSuccessStatusCode)
                {
                    //var data = response.Content.ReadAsAsync<IEnumerable<apigatewayController>>().Result;                    

                    JArray jsonArray = JArray.Parse(response.Content.ReadAsStringAsync().Result);

                    return Ok(jsonArray);
                }
                else
                {
                    return Ok(Request.CreateResponse(HttpStatusCode.BadRequest, "Some thing went wrong. please contact administrator."));
                }
            }
            catch (Exception ex) { throw ex; }
            finally { }
        }

        /// <summary>
        /// delete user
        /// </summary>
        /// <param name="objDeleteInfo"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("apigateway/user")]
        public IHttpActionResult DeleteUser([FromBody] object objDeleteInfo)
        {
            return Ok("");
        }



       /// <summary>
       /// 
       /// </summary>
       /// <returns></returns>

        [HttpDelete]
        [Route("apigateway/Locations")]
        public IHttpActionResult Locations()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5190/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/v1.0/Locations").Result;
                if (response.IsSuccessStatusCode)
                {
                    //var data = response.Content.ReadAsAsync<IEnumerable<apigatewayController>>().Result;                    

                    JArray jsonArray = JArray.Parse(response.Content.ReadAsStringAsync().Result);

                    return Ok(jsonArray);
                }
                else
                {
                    return Ok(Request.CreateResponse(HttpStatusCode.BadRequest, "Some thing went wrong. please contact administrator."));
                }
            }
            catch (Exception ex) { throw ex; }
            finally { }
        }


        /// <summary>
        /// fetching given staff id user profile data
        /// </summary>
        /// <param name="staffid">mandatory</param>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/user/profiledata/{staffid}")]
        public IHttpActionResult GetUserProfile(long staffid)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(strApiUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/user/profiledata/" + staffid).Result;
                if (response.IsSuccessStatusCode)
                {
                    //var data = response.Content.ReadAsAsync<IEnumerable<apigatewayController>>().Result;                    

                    //JArray jsonArray = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                   
                    return Ok(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    return Ok(Request.CreateResponse(HttpStatusCode.BadRequest, "Some thing went wrong. please contact administrator."));
                }

            }
            catch (Exception ex) { throw ex; }
            finally { }
        }

        /// <summary>
        /// save user profile data
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("apigateway/userprofile")]
        public IHttpActionResult SaveUserProfile()
        {
            return Ok("");
        }

      
        
        //ACLManagement Api endpoints

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/aclmanagement")]
        public IHttpActionResult GetMCNManagementDetails()
        {
            return Ok("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user_id">mandatory</param>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/aclmanagement/{userid}")]
        public IHttpActionResult GetMCNManagementDetail(long user_id)
        {
            return Ok("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("apigateway/aclmanagement")]
        public IHttpActionResult Update()
        {
            return Ok("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objDeleteInfo">mandatory</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("apigateway/aclmanagement")]
        public IHttpActionResult Delete([FromBody] object objDeleteInfo)
        {
            return Ok("");
        }


        // Capability Managemt api's


        /// <summary>
        /// 
        /// </summary>
        /// <param name="user_id">mandatory</param>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/capabilitymanagement")]
        public IHttpActionResult ListofCapabilities(long user_id)
        {
            return Ok("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("apigateway/capabilitymanagement")]
        public IHttpActionResult SaveCapabilities()
        {
            return Ok("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objDeleteInfo">mandatory</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("apigateway/capabilitymanagement")]
        public IHttpActionResult DeleteCapabilities([FromBody] object objDeleteInfo)
        {
            return Ok("");
        }


        // Clinician api endpoint's

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user_id">mandatory</param>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/clinician")]
        public IHttpActionResult GetClinicians(long user_id)
        {
            return Ok("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("apigateway/clinician")]
        public IHttpActionResult SaveClinician()
        {
            return Ok("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objDeleteInfo">mandatory</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("apigateway/clinician")]
        public IHttpActionResult DeleteClinician([FromBody] object objDeleteInfo)
        {
            return Ok("");
        }

       
        //Location api endpoint's

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/location")]
        public IHttpActionResult GetLocations()
        {
            return Ok("");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationid">mandatory</param>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/location/{locationid}")]
        public IHttpActionResult GetLocation(Guid locationid)
        {

            return Ok("");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("apigateway/location")]
        public IHttpActionResult SaveLocation()
        {
            return Ok("");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationid">mandatory</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("apigateway/location/{locationid}")]
        public IHttpActionResult DeleteLocation(Guid locationid)
        {
            return Ok("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">mandatory</param>
        /// <returns></returns>
        [HttpPatch]  
        [Route("apigateway/location/{id}")]
        public IHttpActionResult EditLocation(Guid id)
        {
            return Ok("");
        }

        
        // ManageAccount api enpoints


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid">mandatory</param>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/manageaccount/{userid}")]
        public IHttpActionResult GetUserAccountDetails(long userid)
        {
            return Ok("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("apigateway/manageaccount")]
        public IHttpActionResult AddUserAccount()
        {
            return Ok("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objDeleteInfo">mandatory</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("apigateway/manageaccount")]
        public IHttpActionResult DeleteUserAccount([FromBody] object objDeleteInfo)
        {
            return Ok("");
        }

      
        //MCNManagement api endpoint's

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/mcnmanagement")]
        public IHttpActionResult GetMcnManagementDetails()
        {
            return Ok("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid">mandatory</param>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/mcnmanagement/{userid}")]
        public IHttpActionResult GetMcnManagementDetails(long userid)
        {
            return Ok("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("apigateway/mcnmanagement")]
        public IHttpActionResult UpdateMcnManagement()
        {
            return Ok("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objDeleteInfo">mandatory</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("apigateway/mcnmanagement")]
        public IHttpActionResult DeleteMcnManagement([FromBody] object objDeleteInfo)
        {
            return Ok("");
        }

       
        //PatientMerge api endpoint's

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mergeunmerge">mandatory</param>
        /// <returns></returns>
        [HttpPost]
        [Route("apigateway/patientmerge/{mergeorunmerge}")]
        public IHttpActionResult MergeorUnMerge(int mergeunmerge)
        {
            return Ok("");
        }

        
        
        //AdhocReports api endpoint's


        /// <summary>
        /// active patient details
        /// </summary>
        /// <param name="active">mandatory</param>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/adhocreports/patients")]
        public IHttpActionResult GetActivePatients()
        {      

            HttpClient client = new HttpClient();
            try
            {                
                client.BaseAddress = new Uri("http://localhost:57573/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/adhocreports/patient").Result;
                if (response.IsSuccessStatusCode)
                {

                    JArray jsonArray = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    return Ok(jsonArray);
                }
                else
                    return Ok(Request.CreateResponse(HttpStatusCode.BadRequest, "Some thing went wrong. please contact administrator."));
            }
            catch (Exception ex) { throw ex; }
            finally { client = null; }
           
           
        }

        /// <summary>
        /// all patient details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/adhocreports/patients/{startindex}/{endindex}")]
        public IHttpActionResult GetAllPatients(int startindex,int endindex)
        {            

            HttpClient client = new HttpClient();
            try
            {
                client.BaseAddress = new Uri("http://localhost:57573/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/adhocreports/patient/" + startindex + "/" + endindex).Result;
                if (response.IsSuccessStatusCode)
                {

                    JArray jsonArray = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    return Ok(jsonArray);
                }
                else
                    return Ok(Request.CreateResponse(HttpStatusCode.BadRequest, "Some thing went wrong. please contact administrator."));
            }
            catch (Exception ex) { throw ex; }
            finally { client = null; }

           
        }

        /// <summary>
        /// all mcn details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/adhocreports/mcnlist")]
        public IHttpActionResult GetMCNList()
        {
            HttpClient client = new HttpClient();
            try
            {
                client.BaseAddress = new Uri("http://localhost:57573/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/adhocreports/mcnlist").Result;
                if (response.IsSuccessStatusCode)
                {

                    JArray jsonArray = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    return Ok(jsonArray);
                }
                else
                    return Ok(Request.CreateResponse(HttpStatusCode.BadRequest, "Some thing went wrong. please contact administrator."));
            }
            catch (Exception ex) { throw ex; }
            finally { client = null; }
        }

        /// <summary>
        /// all patient address details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/adhocreports/patientaddress")]
        public IHttpActionResult GetAllPatientAddress()
        {
            HttpClient client = new HttpClient();
            try
            {
                client.BaseAddress = new Uri("http://localhost:57573/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/adhocreports/patientaddress").Result;
                if (response.IsSuccessStatusCode)
                {

                    JArray jsonArray = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    return Ok(jsonArray);
                }
                else
                    return Ok(Request.CreateResponse(HttpStatusCode.BadRequest, "Some thing went wrong. please contact administrator."));
            }
            catch (Exception ex) { throw ex; }
            finally { client = null; }
        }

        /// <summary>
        /// fetching specialform data based on given customform id
        /// </summary>
        /// <param name="formId">mandatory</param>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/adhocreports/specialform/{formid}")]
        public IHttpActionResult GetSpecialFormData(long formId)
        {
            return Ok("");
        }

        /// <summary>
        /// contactanalysis specialform data        
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apigateway/adhocreports/contactanalysis")]
        public IHttpActionResult GetContactAnalysisData()
        {
            var formId = "1000";
            /*SELECT Patient.PAT_ID, Patient.PAT_Forename1, Patient.PAT_Surname,Patient.PAT_DOB, Patient.PAT_Primary_Identifier, Patient.PAT_Secondary_Identifier, tblPCF_1000.CFC100013 AS 'Time Started', tblPCF_1000.CFC100028 AS 'Logged in User', tblPCF_1000.ID AS 'Contact ID',
                 tblPCF_1000.CFC100625 AS 'Cause 1', tblPCF_1000.CFC100630 AS 'Cause2', tblPCF_1000.CFC100631 AS 'Cause 3', tblPCF_1000.CFC100632 AS 'Cause 4', tblUDFLookupValue.Description AS 'Type', tblUDFLookupValue_1.Description AS 'Prompt', tblUDFLookupValue_2.Description as 'Service Type'
            FROM tblPCF_1000 LEFT OUTER JOIN
           tblUDFLookupValue AS tblUDFLookupValue_2 ON tblPCF_1000.CFC101357 = tblUDFLookupValue_2.ID LEFT OUTER JOIN
           tblUDFLookupValue AS tblUDFLookupValue_1 ON tblPCF_1000.CFC100015 = tblUDFLookupValue_1.ID LEFT OUTER JOIN
           tblUDFLookupValue ON tblPCF_1000.CFC100016 = tblUDFLookupValue.ID LEFT OUTER JOIN
           Patient ON tblPCF_1000.Patient_ID = Patient.PAT_ID
            where tblPCF_1000.isactive = 1 and Patient.isactive = 1*/


            /*SELECT App.[id]       ,App.[pat_id]    ,App.[startdatetime]    ,CLN.CLN_Fname    ,CLN.CLN_SName    ,l1.Description    ,P.PAT_Forename1    ,P.PAT_Surname    ,l2.lookupValue
            ,G.Pat_ID
            ,P2.Pat_forename1, P2.PAT_Surname
            FROM[AdhocReportsDB_SBHS_PROD].[dbo].[tblApptSchedule] as App
            LEFT OUTER JOIN[AdhocReportsDB_SBHS_PROD].[dbo].[Clinician] as CLN ON App.clinician_id = CLN.CLN_ID
            LEFT OUTER JOIN[AdhocReportsDB_SBHS_PROD].[dbo].tblUDFLookupValue as l1 ON App.clinic_id = l1.id
            LEFT OUTER JOIN[AdhocReportsDB_SBHS_PROD].[dbo].Patient as P ON App.pat_id = P.Pat_id
            LEFT OUTER JOIN[AdhocReportsDB_SBHS_PROD].[dbo].tbllookup as l2 ON App.attendance_code_slu = l2.id
            LEFT OUTER JOIN[AdhocReportsDB_SBHS_PROD].[dbo].tblApptGroupPatient as G ON App.id = G.Appt_ID
            LEFT OUTER JOIN[AdhocReportsDB_SBHS_PROD].[dbo].Patient as P2 ON G.pat_id = P2.Pat_id*/



            return Ok("");
        }

       /// <summary>
       /// fetching of appointmentscheuler information    
       /// </summary>
       /// <returns></returns>
        [HttpGet]
        [Route("apigateway/adhocreports/apptmntscheduler")]
        public IHttpActionResult ApptSchedulerInfo()
        {

            HttpClient client = new HttpClient();
            try
            {
                client.BaseAddress = new Uri("http://localhost:57573/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/adhocreports/apptmntschedulerInfo").Result;
                //HttpResponseMessage response = client.GetAsync("api/adhocreports/apptmntschedulerInfo/" + startindex + "/" + endindex).Result;
                if (response.IsSuccessStatusCode)
                {

                    JArray jsonArray = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    return Ok(jsonArray);
                }
                else
                    return Ok(Request.CreateResponse(HttpStatusCode.BadRequest, "Some thing went wrong. please contact administrator."));
            }
            catch (Exception ex) { throw ex; }
            finally { client = null; }
     
        }
    }
}
