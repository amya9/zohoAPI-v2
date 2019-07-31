using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Data.Linq;
using MCG_Generic_Module.DataModel;
using System.Configuration;
using System.Xml;
using System.Threading;
using System.Text.RegularExpressions;

namespace MCG_Generic_Module

{
    public class ZohoLeads
    {
        private MCG_Generic_ModuleDataContext objDataContext = new MCG_Generic_ModuleDataContext(ConfigurationManager.ConnectionStrings["CGConnectionString"].ConnectionString);
        private string connection = ConfigurationManager.ConnectionStrings["CGConnectionString"].ConnectionString;
           
        ////////For Zoho Data details///////////
        private string sUser_ID = string.Empty;
        private string sLeadSource = string.Empty;
        private string sFName = string.Empty;
        private string sLName = string.Empty;
        private string sEmail = string.Empty;
        private string sContactNo = string.Empty;
        private string sParentContactNo = string.Empty;
        private string sAddress = string.Empty;
        private string sAssessmentCompleted = string.Empty;
        private string sProductCode = string.Empty;
        private string sQuestionURL = string.Empty;
        private string sContactUSQuery = string.Empty;
        private string sDescription = string.Empty;
        ////////For Zoho Data details///////////

        ////////Bizphone to Zoho Data details///////////
        private string sbizphone_LeadStatus = string.Empty;
        private string sbizphone_ContactNo = string.Empty;
        private string sbizphone_CallStatus = string.Empty;
        private string sbizphone_RecordingUrl = string.Empty;
        private string sbizphone_CallDuration = string.Empty;        
        private string sbizphone_Extn = string.Empty;
        private string sbizphone_Description = string.Empty;
        private string mleadsource = string.Empty;
        private string _landing_page = string.Empty;
        private string _utm_parameters = string.Empty;
        private string _education_level = string.Empty;
        ////////Bizphone to Zoho Data details///////////


        public static string zohocrmurl = "https://crm.zoho.com/crm/private/xml/";
        public static void Main(string[] args)
        {
            //string result = APIMethod("Leads", "insertRecords", "508020000000332001");//Change the id,method name, and module name here
            //Console.Write(result);
        }

        public string  LandingPage
        {
            get
            {
                return this._landing_page;
            }
            set
            {
                this._landing_page = value;
            }
        }

        public string UTMParameters
        {
            get
            {
                return this._utm_parameters;
            }
            set
            {
                this._utm_parameters = value;
            }
        }

        public string EducationLevel
        {
            get
            {
                return this._education_level;
            }
            set
            {
                this._education_level = value;
            }
        }

        public void InsertLeadToZoho_Threaded(string User_ID, string LeadSource, string FName, string LName, string Email, string ContactNo, string ParentContactNo, string Address, string AssessmentCompleted, string ProductCode, string QuestionURL, string ContactUSQuery, string Description)
        {
            sUser_ID = User_ID;
            sLeadSource = LeadSource;
            sFName = FName;
            sLName = LName;
            sEmail = Email;
            sContactNo = ContactNo;
            sParentContactNo = ParentContactNo;
            sAddress = Address;
            sAssessmentCompleted = AssessmentCompleted;
            sProductCode = ProductCode;
            sQuestionURL = QuestionURL;
            sContactUSQuery = ContactUSQuery;
            sDescription = Description;

            string sLocalOrNot = string.Empty;
            try
            {
				ThreadStart th = new ThreadStart(InsertLeadToZoho);
                Thread t = new Thread(th);
                t.Priority = ThreadPriority.Highest;
                t.Start();
               // sLocalOrNot = ConfigurationManager.AppSettings["zoholocal"].ToString();
            }
            catch(Exception ex) { }

           /* if (string.IsNullOrEmpty(sLocalOrNot))
            {
                
            }*/
        }

        private void InsertLeadToZoho()
        {
            string User_ID = string.Empty;
            string LeadSource = string.Empty;
            string FName = string.Empty;
            string LName = string.Empty;
            string Email = string.Empty;
            string ContactNo = string.Empty;
            string ParentContactNo = string.Empty;
            string Address = string.Empty;
            string AssessmentCompleted = string.Empty;
            string ProductCode = string.Empty;
            string QuestionURL = string.Empty;
            string ContactUSQuery = string.Empty;
            string Description = string.Empty;

            string PTGAttempts = string.Empty;

            User_ID = sUser_ID;
            LeadSource = sLeadSource;
            FName = sFName;
            LName = sLName;
            Email = sEmail;
            ContactNo = sContactNo;
            ParentContactNo = sParentContactNo;
            Address = sAddress;
            AssessmentCompleted = sAssessmentCompleted;
            ProductCode = sProductCode;
            QuestionURL = sQuestionURL;
            ContactUSQuery = sContactUSQuery;
            Description = sDescription;            

            FName = FName.Replace("&", " ");
            LName = LName.Replace("&", " ");            
            Address = Address.Replace("&", "and");
            AssessmentCompleted = AssessmentCompleted.Replace("&", " ");
            ProductCode = ProductCode.Replace("&", " ");
            QuestionURL = QuestionURL.Replace("&", " ");
            ContactUSQuery = ContactUSQuery.Replace("&", "and");
            Description = Description.Replace("&", "and");

            if (LName == string.Empty)
            {
                LName = ".";
            }

            string sZohoID = CheckForZohoID(Email);
            //ContactNo = "0" + FormatContactNo(ContactNo);
            
            if (LeadSource == "Proceed To Guidance") { PTGAttempts = "1"; }
          
            if (LeadSource == "Just Dial" || LeadSource == "Contact Us" || LeadSource == "Business Enquiries" || LeadSource == "FB Adds" || LeadSource == "Google Add-word" || LeadSource == "Youmint" || string.IsNullOrEmpty(sZohoID))
            {
                string modulename = "Leads";
                string methodname = "insertRecords";
                string recordId = "";//= "508020000000332001";
                string uri = zohocrmurl + modulename + "/" + methodname + "?";
                /* Append your parameters here */
                string postContent = "scope=crmapi";
                string landing_page = "";
                string utm = "";
                string edu_level = "";

                if (LandingPage != string.Empty) {
                    landing_page = "<FL val=\"Landing Page\">" + LandingPage + "</FL>";
                }
                if (UTMParameters != string.Empty)
                {
                    utm = "<FL val=\"UTM Parameters\">" + UTMParameters + "</FL>";
                }
                if (EducationLevel != string.Empty)
                {
                    edu_level = "<FL val=\"Education Level\">" + EducationLevel + "</FL>";
                }

                postContent = postContent + "&authtoken=*********************";//Give your authtoken
                if (methodname.Equals("insertRecords") || methodname.Equals("updateRecords"))
                {
                    postContent = postContent + "&xmlData=" + HttpUtility.UrlEncode("<Leads><row no=\"1\">" +
                        "<FL val=\"Task Owner\">lovechopra@meracareerguide.com</FL>" +
                        "<FL val=\"Lead Source\">" + LeadSource + "</FL>" +
                        "<FL val=\"Lead Status\">Open (O)</FL>" +
                        "<FL val=\"First Name\">" + FName + "</FL>" +
                        "<FL val=\"Last Name\">" + LName + "</FL>" +
                        "<FL val=\"Email\">" + Email + "</FL>" +
                        "<FL val=\"Phone1\">" + ContactNo + "</FL>" +
                        "<FL val=\"Contact No\">" + ContactNo + "</FL>" +
                        "<FL val=\"Parents Contact No\">" + ParentContactNo + "</FL>" +
                        "<FL val=\"Address\">" + Address + "</FL>" +
                        "<FL val=\"Assessment Completed\">" + AssessmentCompleted + "</FL>" +
                        "<FL val=\"Product Code\">" + ProductCode + "</FL>" +
                        "<FL val=\"Question Desc\">" + QuestionURL + "</FL>" +
                        "<FL val=\"Contact us Query\">" + ContactUSQuery + "</FL>" +
                        "<FL val=\"PTG Attempts\">" + PTGAttempts + "</FL>" +
                        "<FL val=\"Description\">" + Description + "</FL>" + landing_page + utm + edu_level +
                        "</row></Leads>");

                    postContent = postContent + "&wfTrigger=true";
                }
                if (methodname.Equals("updateRecords") || methodname.Equals("deleteRecords") || methodname.Equals("getRecordById"))
                {
                    postContent = postContent + "&id=" + recordId;
                }

                try
                {
                    string result = AccessCRM(uri, postContent);
                    //return result;
                    XmlDocument document = new XmlDocument();
                    document.Load(new System.IO.StringReader(result));
                    XmlNode node = document.DocumentElement.SelectSingleNode("//FL");
                    string ZohoLeadID = node.InnerXml;      //ZOHO Return Lead ID.

                    InsertLead(User_ID, Email, ContactNo, ZohoLeadID, LeadSource);

                  //insert Lead into Assessment Subdomain
                    this.InsertIntoSubDomain(Email, ZohoLeadID, LeadSource);
                  //insert Lead into Assessment Subdomain
                }
                catch (Exception ex)
                {
                    MCG_Email_SMS_Util.Email objEmail = new MCG_Email_SMS_Util.Email();
                    string sErrorMsg = string.Empty;
                    sErrorMsg = ex.Message.ToString() +
                        "LeadSource= " + LeadSource + "<br>" +
                        "FName= " + FName + "<br>" +
                        "LName= " + LName + "<br>" +
                        "Email= " + Email + "<br>" +
                        "ContactNo= " + ContactNo + "<br>" +
                        "Parent ContactNo= " + ParentContactNo + "<br>" +
                        "Address= " + Address + "<br>" +
                        "Assessment Completed= " + AssessmentCompleted + "<br>" +
                        "ProductCode= " + ProductCode + "<br>" +
                        "Question Desc= " + QuestionURL + "<br>" +
                        "Contact Us Query= " + ContactUSQuery + "<br>" +
                        "PTG Attempts= " + PTGAttempts + "<br>" +
                        "Description= " + Description + "<br>";
                    objEmail.ShootMail_NonThread(MCG_Email_SMS_Util.Email.EmailServer.Amazon_CRM_Utility,MCG_Email_SMS_Util.Email.EmailShootFor.crm_in,"sto","love@careerguide.com","Error while entering lead to Zoho for " + LeadSource + " LeadSource",sErrorMsg,"");
                    objEmail = null;
                }
            }
            else
            {                
                string sCurrentLeadStatus = string.Empty;
                string sCurrentModifiedTime = string.Empty;
                string sCurrentDiscription = string.Empty;
                string sCurrentProductCode = string.Empty;
                string sCurrentAssCompleted = string.Empty;
                string sCurrentQuestionDesc = string.Empty;
                string sCurrentLeadSource = string.Empty;
                string sCurrentPTGAttempts = string.Empty;

                //string sZohoID = CheckForZohoID(Email);

                string modulename = "Leads";
                string methodname = "getRecordById";
                string recordId = sZohoID;
                string uri = zohocrmurl + modulename + "/" + methodname + "?";
                string postContent = string.Empty;

                if (methodname.Equals("getRecordById"))
                {
                    postContent = "scope=crmapi";
                    postContent = postContent + "&authtoken=*************************";
                    postContent = postContent + "&id=" + recordId;
                    postContent = postContent + "&newFormat=2&selectColumns=Leads(Lead Source,Current Lead Source,Lead Status,Modified Time,Assessment Completed,Question Desc,Product Code,Description,PTG Attempts)";

                    string result = AccessCRM(uri, postContent);
                    int i = 0;                    
                    XmlDocument doc = new XmlDocument();
                    doc.Load(new System.IO.StringReader(result));                    
                    foreach (XmlNode node in doc.DocumentElement.SelectNodes("//FL"))
                    {
                        XmlNode node1 = doc.DocumentElement.SelectNodes("//FL")[i];                        
                        if (i == 2) { sCurrentLeadStatus = node1.InnerText; }
                        else if (i == 3) { sCurrentModifiedTime = node1.InnerText; }
                        else if (i == 4) { sCurrentDiscription = node1.InnerText; }
                        else if (i == 5) { sCurrentProductCode = node1.InnerText; }
                        else if (i == 6) { sCurrentAssCompleted = node1.InnerText; }
                        else if (i == 7) { sCurrentQuestionDesc = node1.InnerText; }
                        else if (i == 8) { sCurrentLeadSource = node1.InnerText; }
                        else if (i == 9)
                        {
                          sCurrentPTGAttempts = node1.InnerText;
                          if (LeadSource == "Proceed To Guidance")
                          {
                            if (!string.IsNullOrEmpty(sCurrentPTGAttempts) & sCurrentPTGAttempts.ToLower() != "null")
                            {
                              sCurrentPTGAttempts = Convert.ToString(Convert.ToInt32(sCurrentPTGAttempts) + 1);
                            }
                            else
                            {
                              sCurrentPTGAttempts = "1";
                            }
                          }
                        }
                        i++;
                    }
                }
                if (sCurrentModifiedTime.ToLower() != "null")
                {
                  try { DateTime dModifiedDate = Convert.ToDateTime(sCurrentModifiedTime); sCurrentModifiedTime = dModifiedDate.ToString("dd/MM/yyyy hh:mm tt"); }
                  catch (Exception x) { }
                }
                if (sCurrentLeadSource.ToLower() == "null")
                {
                    sCurrentLeadSource = LeadSource;
                }
                string sNewDescription = sCurrentModifiedTime + "\t Last Lead Source: " + sCurrentLeadSource + "\t Last Lead Status: " + sCurrentLeadStatus + "\t Last Product Code: " + sCurrentProductCode + "\t Assessment Done: " + sCurrentAssCompleted + "\t Question URL: " + sCurrentQuestionDesc;// +"\t Desc: " + Description;
                sNewDescription = sNewDescription + "\n\n" + sCurrentDiscription;
                
                postContent = string.Empty;
                methodname = "updateRecords";
                uri = zohocrmurl + modulename + "/" + methodname + "?";
                if (methodname.Equals("updateRecords"))
                {
                    postContent = "scope=crmapi";
                    postContent = postContent + "&authtoken=**********************";

                    string sParentUpdate = "";
                    if (ParentContactNo != "") { sParentUpdate = "<FL val=\"Parents Contact No\">" + ParentContactNo + "</FL>"; }
                    string sQuestionUpdate = "";
                    if (QuestionURL != "") { sQuestionUpdate = "<FL val=\"Question Desc\">" + QuestionURL + "</FL>"; }
                    string sContactUsUpdate = "";
                    if (ContactUSQuery != "") { sContactUsUpdate = "<FL val=\"Contact us Query\">" + ContactUSQuery + "</FL>"; }
                    string scontact_no = "";
                    if (ContactNo != "" ) { scontact_no = "<FL val=\"Contact No\">" + ContactNo + "</FL>"; }

                    postContent = postContent + "&xmlData=" + HttpUtility.UrlEncode("<Leads><row no=\"1\">" +

                        "<FL val=\"Current Lead Source\">" + LeadSource + "</FL>" +
                        "<FL val=\"Lead Status\">Open (O)</FL>" +                       
                        sParentUpdate + scontact_no +
                        "<FL val=\"Product Code\">" + ProductCode + "</FL>" +
                        sQuestionUpdate +
                        "<FL val=\"Assessment Completed\">" + AssessmentCompleted + "</FL>" +
                        sContactUsUpdate +
                        //"<FL val=\"Description\">" + Description + "</FL>" +
                        "<FL val=\"PTG Attempts\">" + sCurrentPTGAttempts + "</FL>" +
                        "<FL val=\"Description\">" + sNewDescription + "</FL>" +

                    "</row></Leads>");

                    postContent = postContent + "&wfTrigger=true";
                    postContent = postContent + "&id=" + recordId;

                    try
                    {
                        string result = AccessCRM(uri, postContent);
                    }
                    catch (Exception ex)
                    {
                        MCG_Email_SMS_Util.Email objEmail = new MCG_Email_SMS_Util.Email();
                        string sErrorMsg = string.Empty;
                        sErrorMsg = ex.Message.ToString() +
                            "LeadSource= " + LeadSource + "<br>" +
                            "FName= " + FName + "<br>" +
                            "LName= " + LName + "<br>" +
                            "Email= " + Email + "<br>" +
                            "ContactNo= " + ContactNo + "<br>" +
                            "Parent ContactNo= " + ParentContactNo + "<br>" +
                            "Address= " + Address + "<br>" +
                            "Assessment Completed= " + AssessmentCompleted + "<br>" +
                            "ProductCode= " + ProductCode + "<br>" +
                            "Question Desc= " + QuestionURL + "<br>" +
                            "Contact Us Query= " + ContactUSQuery + "<br>" +
                            "PTG Attempts= " + sCurrentPTGAttempts + "<br>" +
                            "Description= " + Description + "<br>";
                        objEmail.ShootMail_NonThread(MCG_Email_SMS_Util.Email.EmailServer.Amazon_CRM_Utility, MCG_Email_SMS_Util.Email.EmailShootFor.crm_in, "sto", "love@careerguide.com", "Error while Updating lead to Zoho for " + LeadSource + " LeadSource", sErrorMsg, "");
                        objEmail = null;
                    }
                }
            }
        }



        public void InsertBizphoneLeadToZoho_Threaded(string LeadStatus, string ContactNo, string BizphoneCallStatus, string RecordingURL, string CallDuration, string Extension, string Description,string leadsource)
        {
            sbizphone_LeadStatus = LeadStatus;
            sbizphone_ContactNo = ContactNo;
            sbizphone_CallStatus = BizphoneCallStatus;
            sbizphone_RecordingUrl = RecordingURL;
            sbizphone_CallDuration = CallDuration;            
            sbizphone_Extn = Extension;
            sbizphone_Description = Description;
            mleadsource = leadsource;

             string sLocalOrNot = string.Empty;
            try
            {
                sLocalOrNot = ConfigurationManager.AppSettings["zoholocal"].ToString();
            }
            catch(Exception ex) { }

            if (string.IsNullOrEmpty(sLocalOrNot))
            {
                ThreadStart th = new ThreadStart(InsertBizphoneLeadToZoho);
                Thread t = new Thread(th);
                t.Priority = ThreadPriority.Highest;
                t.Start();
            }
        }

        private void InsertBizphoneLeadToZoho()
        {
            string bizphone_LeadStatus = string.Empty;
            string bizphone_ContactNo = string.Empty;
            string bizphone_CallStatus = string.Empty;
            string bizphone_RecordingUrl = string.Empty;
            string bizphone_CallDuration = string.Empty;
            string bizphone_Department = string.Empty;
            string bizphone_Extn = string.Empty;
            string bizphone_Description = string.Empty;

            bizphone_LeadStatus = sbizphone_LeadStatus;
            bizphone_ContactNo = sbizphone_ContactNo;
            bizphone_CallStatus = sbizphone_CallStatus;
            bizphone_RecordingUrl = sbizphone_RecordingUrl;
            bizphone_CallDuration = sbizphone_CallDuration;            
            bizphone_Extn = sbizphone_Extn;
            bizphone_Description = sbizphone_Description;

            string modulename = "Leads";
            string methodname = "insertRecords";
            string recordId = "";//= "508020000000332001";
                        

            string uri = zohocrmurl + modulename + "/" + methodname + "?";
            /* Append your parameters here */
            string postContent = "scope=crmapi";
            bizphone_ContactNo = "0" + FormatContactNo(bizphone_ContactNo);
            postContent = postContent + "&authtoken=********************";//Give your authtoken
            if (methodname.Equals("insertRecords") || methodname.Equals("updateRecords"))
            {
                postContent = postContent + "&xmlData=" + HttpUtility.UrlEncode("<Leads><row no=\"1\">" +
                    "<FL val=\"Task Owner\">lovechopra@meracareerguide.com</FL>" +
                    "<FL val=\"Lead Source\">" + mleadsource + "</FL>" +
                    "<FL val=\"Lead Status\">" + bizphone_LeadStatus + "</FL>" +                    
                    "<FL val=\"Last Name\">.</FL>" +
                    "<FL val=\"Phone1\">" + bizphone_ContactNo + "</FL>" +
                    "<FL val=\"Contact No\">" + bizphone_ContactNo + "</FL>" +
                    "<FL val=\"Biz Call Status\">" + bizphone_CallStatus + "</FL>" +
                    "<FL val=\"Biz Recording URL\">" + bizphone_RecordingUrl + "</FL>" +
                    "<FL val=\"Biz Call Duration\">" + bizphone_CallDuration  + "</FL>" +
                    "<FL val=\"Biz Attended By\">" + bizphone_Extn + "</FL>" +                    
                    "<FL val=\"Description\">" + bizphone_Description + "</FL>" +
                    "</row></Leads>");
            }
            if (methodname.Equals("updateRecords") || methodname.Equals("deleteRecords") || methodname.Equals("getRecordById"))
            {
                postContent = postContent + "&id=" + recordId;
            }

            try
            {
                string result = AccessCRM(uri, postContent);
                //return result;
                //return result;
                XmlDocument document = new XmlDocument();
                document.Load(new System.IO.StringReader(result));
                XmlNode node = document.DocumentElement.SelectSingleNode("//FL");
                string ZohoLeadID = node.InnerXml;      //ZOHO Return Lead ID.
                Guid userID = new Guid();
                userID = Guid.Empty;
                InsertLead(userID.ToString(), "", bizphone_ContactNo, ZohoLeadID, "Inbound Calls");
            }
            catch (Exception ex)
            {
                MCG_Email_SMS_Util.Email objEmail = new MCG_Email_SMS_Util.Email();
                objEmail.ShootMail_NonThread(MCG_Email_SMS_Util.Email.EmailServer.Amazon_CRM_Utility, MCG_Email_SMS_Util.Email.EmailShootFor.crm_in, "sto", "love@careerguide.com", "Error while entering Bizphone lead to Zoho", ex.Message.ToString(), "");
                objEmail = null;
            }
           
        }
        public static string AccessCRM(string url, string postcontent)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(postcontent);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }


        public void InsertLead(string userID, string Email_ID, string Contact_No, string Zoho_Lead_ID, string Lead_Source)
        {
            sh_Zoho_Lead ZOHO = new sh_Zoho_Lead();

            ZOHO.User_ID = new Guid(userID);
            ZOHO.Email_ID = Email_ID;
            ZOHO.Contact_No = Contact_No;
            ZOHO.Zoho_Lead_ID = Zoho_Lead_ID;
            ZOHO.Lead_Source = Lead_Source;
            ZOHO.RcdCreatedDate = DateTime.Now;
            ZOHO.RcdLastUpdatedDate = DateTime.Now;

            InsertZohoLead(ZOHO);
        }

        private void InsertZohoLead(sh_Zoho_Lead LocalTable)
        {
            try
            {
                objDataContext.sh_Zoho_Leads.InsertOnSubmit(LocalTable);
                objDataContext.SubmitChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public ISingleResult<sp_GetZohoLeadIDByEmailResult> CheckForZohoID(string sUserEmail)
        //{
        //    try
        //    {
        //        return objDataContext.sp_GetZohoLeadIDByEmail(sUserEmail);                
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public string CheckForZohoID(string sUserEmail)
        {
            string sZohoID = string.Empty;
            SqlConnection con = new SqlConnection(connection);
            try
            {                                
                SqlCommand cmd = new SqlCommand("sp_GetZohoLeadIDByEmail", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserEmail", sUserEmail);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    sZohoID = dr["Zoho_Lead_ID"].ToString();
                }
                dr.Close();
                con.Close();
                return sZohoID;
            }
            catch (Exception ex)
            {
                con.Close();
                return sZohoID;                
            }
        }


        public enum ForAssessment { None, Commerce, Engineering, Humanities, Ideal, Skills, Stream,Vocational };
        public string GetAssessmentProductCodeByEduLevelID(int iEduLevelID, ForAssessment eForAssessment)
        {
          string sOut = string.Empty;
          if (iEduLevelID != 0 & eForAssessment.ToString() != "None")
          {
            if (iEduLevelID == 1) { sOut = "9th - 499"; }
            else if (iEduLevelID == 2) { sOut = "10th - 499"; }
            else if (iEduLevelID == 3) { sOut = "11th Arts - 499"; }
            else if (iEduLevelID == 4) { sOut = "11th Eng. Aspirant - 499"; }
            else if (iEduLevelID == 5) { sOut = "11th Commerce - 499"; }
            else if (iEduLevelID == 6) { sOut = "11th Biology - 499"; }
            else if (iEduLevelID == 7) { sOut = "11th Science - 499"; }
            else if (iEduLevelID == 8) { sOut = "12th Arts - 499"; }
            else if (iEduLevelID == 9) { sOut = "12th Eng. Aspirant - 499"; }
            else if (iEduLevelID == 10) { sOut = "12th Science - 499"; }
            else if (iEduLevelID == 11) { sOut = "12th Commerce - 499"; }
            else if (iEduLevelID == 12) { sOut = "12th Biology - 499"; }
            else if (iEduLevelID == 13) { sOut = "B.E/ B.Tech - 499"; }
            else if (iEduLevelID == 14) { sOut = "B.Com - 499"; }
            else if (iEduLevelID == 15) { sOut = "B.B.A - 499"; }
            else if (iEduLevelID == 16) { sOut = "B.C.A - 499"; }
            else if (iEduLevelID == 17) { sOut = "B.A - 499"; }
            else if (iEduLevelID == 18) { sOut = "Other Graduate - 499"; }
            else if (iEduLevelID == 19) { sOut = "8th - 499"; }
            else if (iEduLevelID == 20) { sOut = "M.A - 499"; }
            else if (iEduLevelID == 21) { sOut = "M.C.A - 499"; }
            else if (iEduLevelID == 22) { sOut = "M.B.A - 499"; }
            else if (iEduLevelID == 23) { sOut = "Upto 10th - 499"; }
            else if (iEduLevelID == 24) { sOut = "11th class - 499"; }
            else if (iEduLevelID == 25) { sOut = "12th class - 499"; }
            else if (iEduLevelID == 26) { sOut = "Graduate - 499"; }
            else if (iEduLevelID == 27) { sOut = "Post Graduate - 499"; }
          }
          sOut = eForAssessment.ToString() + " - " + sOut;
          return sOut;
        }


        private void InsertIntoSubDomain(string sUserEmailID, string sZohoID, string sLeadSource)
        {
          string sDomainName = string.Empty;
          try { sDomainName = ConfigurationManager.AppSettings["AssessmentAPISubDomain"]; }
          catch (Exception ex) { }
          if (!string.IsNullOrEmpty(sDomainName))
          {
            string sURL = sDomainName + "/webservices/GetUserAssessmentInfo.asmx/InsertZohoLeadIDForUser?emailid=" + sUserEmailID + "&leadid=" + sZohoID + "&source=" + sLeadSource;
            StreamReader ObjReader;
            WebRequest wrgetUrl = WebRequest.Create(sURL);
            try
            {
              Stream ObjStream;
              ObjStream = wrgetUrl.GetResponse().GetResponseStream();
              ObjReader = new StreamReader(ObjStream);
              ObjReader.ReadToEnd();
              ObjReader.Close();
              ObjStream.Close();
            }
            catch (Exception ex) { }           
          }
        }

        private string FormatContactNo(string sStudentNumber)
        {
          //Remove +91 or 0 from starting of the number 
          string pattern = @"^([0]|\+91)?";
          Regex rgx = new Regex(pattern);
          try
          {
            if (sStudentNumber != null) sStudentNumber = rgx.Replace(sStudentNumber, "");
            if (sStudentNumber.Length > 10) {
              pattern = @"^(91)?";
              rgx = new Regex(pattern);
              if (sStudentNumber != null) sStudentNumber = rgx.Replace(sStudentNumber, "");
            }
          }
          catch { }
          return sStudentNumber;
        }

    }
}
