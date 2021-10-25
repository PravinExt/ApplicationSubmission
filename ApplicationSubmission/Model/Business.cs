using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationSubmission.Model
{
    public class Business
    {
        public int Business_ID { get; set; }
        public int Applicant_ID { get; set; }
        public string Business_Name { get; set; }
        public string Business_ContactNo { get; set; }
        public string Business_Address { get; set; }
        public string Business_Description { get; set; }
        //public string Business_Document { get; set; }

        public List<Business> Get_All_Business(int Applicant_ID)
        {
            List<Business> busilist = new List<Business>();

            DBHelper dbHelper = new DBHelper();
            try
            {
                dbHelper.Connect(dbHelper.GetConnStr());

                MySqlParameter[] busi_para = new MySqlParameter[1];
                busi_para[0] = new MySqlParameter("Applicant_ID", Applicant_ID);
                MySqlDataReader busiReader = dbHelper.ExecuteReader("Get_All_Business_Info", DBHelper.QueryType.StotedProcedure, busi_para);

                while (busiReader.Read())
                {
                    Business busiobj = new Business();
                    busiobj.Business_ID = int.Parse(busiReader["Business_ID"].ToString());
                    busiobj.Applicant_ID = int.Parse(busiReader["Applicant_ID"].ToString());
                    busiobj.Business_Name = busiReader["BusinessName"].ToString();
                    busiobj.Business_ContactNo = busiReader["BusinessContactNo"].ToString();
                    busiobj.Business_Address = busiReader["BusinessAddress"].ToString();
                    busiobj.Business_Description = busiReader["BusinessDescription"].ToString();
                    //if (busiReader["BusinessDocument"] != null)
                    //{
                    //    busiobj.Business_Document = busiReader["BusinessDocument"].ToString();
                    //}

                    busilist.Add(busiobj);
                }

                return busilist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbHelper.DisConnect();
                dbHelper = null;
            }
        }

        public Business Get_Business_by_id(int Business_ID)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                Business busiobj = new Business();
                dbHelper.Connect(dbHelper.GetConnStr());

                MySqlParameter[] busi_para = new MySqlParameter[1];
                busi_para[0] = new MySqlParameter("Business_ID", Business_ID);
                MySqlDataReader busiReader = dbHelper.ExecuteReader("Get_Business_Info_By_Id", DBHelper.QueryType.StotedProcedure, busi_para);

                while (busiReader.Read())
                {
                    busiobj.Business_ID = int.Parse(busiReader["Business_ID"].ToString());
                    busiobj.Applicant_ID = int.Parse(busiReader["Applicant_ID"].ToString());
                    busiobj.Business_Name = busiReader["BusinessName"].ToString();
                    busiobj.Business_ContactNo = busiReader["BusinessContactNo"].ToString();
                    busiobj.Business_Address = busiReader["BusinessAddress"].ToString();
                    busiobj.Business_Description = busiReader["BusinessDescription"].ToString();
                    //if (busiReader["BusinessDocument"] != null)
                    //{
                    //    busiobj.Business_Document = busiReader["BusinessDocument"].ToString();
                    //}
                }

                return busiobj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbHelper.DisConnect();
                dbHelper = null;
            }
        }

        public int Add_BusinessInfo(Business BusiPara)
        {
            DBHelper dbHelper = new DBHelper();
            bool Result = false;
            try
            {
                dbHelper.Connect(dbHelper.GetConnStr());

                MySqlParameter[] busi_para = new MySqlParameter[5];
                busi_para[0] = new MySqlParameter("Applicant_ID", BusiPara.Applicant_ID);
                busi_para[1] = new MySqlParameter("Business_name", BusiPara.Business_Name);
                busi_para[2] = new MySqlParameter("Business_ContactNo", BusiPara.Business_ContactNo);
                busi_para[3] = new MySqlParameter("Business_Address", BusiPara.Business_Address);
                busi_para[4] = new MySqlParameter("Business_Description", BusiPara.Business_Description);
                //busi_para[5] = new MySqlParameter("Business_Document", BusiPara.Business_Document);

                int r = Convert.ToInt32(dbHelper.ExecuteScalar("Add_BusinessInfo", DBHelper.QueryType.StotedProcedure, busi_para));

                if (r > 0)
                {
                    return r;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbHelper.DisConnect();
                dbHelper = null;
            }
        }

        public bool Update_BusinessInfo(Business BusiPara, int id)
        {
            DBHelper dbHelper = new DBHelper();
            bool Result = false;
            try
            {
                dbHelper.Connect(dbHelper.GetConnStr());

                MySqlParameter[] busi_para = new MySqlParameter[6];
                busi_para[0] = new MySqlParameter("Business_ID", id);
                busi_para[1] = new MySqlParameter("Applicant_ID", BusiPara.Applicant_ID);
                busi_para[2] = new MySqlParameter("Business_Name", BusiPara.Business_Name);
                busi_para[3] = new MySqlParameter("Business_ContactNo", BusiPara.Business_ContactNo);
                busi_para[4] = new MySqlParameter("Business_Address", BusiPara.Business_Address);
                busi_para[5] = new MySqlParameter("Business_Description", BusiPara.Business_Description);

                int r = dbHelper.Execute("Update_BusinessInfo", DBHelper.QueryType.StotedProcedure, busi_para);

                if (r == 1)
                {
                    Result = true;
                }

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbHelper.DisConnect();
                dbHelper = null;
            }
        }

        public bool Delete_ApplicantInfo(int id)
        {
            DBHelper dbHelper = new DBHelper();
            bool Result = false;
            try
            {
                dbHelper.Connect(dbHelper.GetConnStr());

                MySqlParameter[] busi_para = new MySqlParameter[1];
                busi_para[0] = new MySqlParameter("Business_ID", id);

                int r = dbHelper.Execute("Delete_BusinessInfo", DBHelper.QueryType.StotedProcedure, busi_para);

                if (r == 1)
                {
                    Result = true;
                }

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbHelper.DisConnect();
                dbHelper = null;
            }
        }

        public void LogMessage(string msg)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                dbHelper.Connect(dbHelper.GetConnStr());

                MySqlParameter[] app_para = new MySqlParameter[1];
                app_para[0] = new MySqlParameter("LogMsg", msg);

                int r = dbHelper.Execute("Add_LogMsg", DBHelper.QueryType.StotedProcedure, app_para);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbHelper.DisConnect();
                dbHelper = null;
            }
        }
    }
}
