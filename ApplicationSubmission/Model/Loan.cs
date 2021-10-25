using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationSubmission.Model
{
    public class Loan
    {
        public int LoanApplication_ID { get; set; }
        public int Business_ID { get; set; }
        public int Applicant_ID { get; set; }
        public decimal LoanApplication_Amount { get; set; }
        public DateTime LoanApplication_Date { get; set; }
        public string LoanApplication_Description { get; set; }
        public int LoanApplication_Status { get; set; }
        public string LoanApplication_BankerComment { get; set; }
        public IFormFile[] LoanDocument { get; set; }

        public List<Loan> Get_All_Loan(int Business_ID)
        {
            List<Loan> loanlist = new List<Loan>();
            DBHelper dbHelper = new DBHelper();
            try
            {
                dbHelper.Connect(dbHelper.GetConnStr());

                MySqlParameter[] loan_para = new MySqlParameter[1];
                loan_para[0] = new MySqlParameter("Business_ID", Business_ID);
                MySqlDataReader loanReader = dbHelper.ExecuteReader("Get_All_Loan_Info", DBHelper.QueryType.StotedProcedure, loan_para);

                while (loanReader.Read())
                {
                    Loan loaniobj = new Loan();

                    loaniobj.LoanApplication_ID = int.Parse(loanReader["LoanApplication_ID"].ToString());
                    loaniobj.Business_ID = int.Parse(loanReader["Business_ID"].ToString());
                    loaniobj.Applicant_ID = int.Parse(loanReader["Applicant_ID"].ToString());
                    loaniobj.LoanApplication_Amount = Convert.ToDecimal(loanReader["LoanAmount"].ToString());
                    loaniobj.LoanApplication_Description = loanReader["LoanDescription"].ToString();
                    loaniobj.LoanApplication_Status = int.Parse(loanReader["LoanStatus"].ToString());
                    loaniobj.LoanApplication_Date = Convert.ToDateTime(loanReader["LoanApplication_Date"].ToString());
                    loaniobj.LoanApplication_BankerComment = loanReader["LoanBanker_Comment"].ToString();

                    loanlist.Add(loaniobj);
                }

                return loanlist;
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

        public Loan Get_Loan_by_id(int Loan_ID)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                Loan loaniobj = new Loan();
                dbHelper.Connect(dbHelper.GetConnStr());

                MySqlParameter[] loan_para = new MySqlParameter[1];
                loan_para[0] = new MySqlParameter("Loan_ID", Loan_ID);
                MySqlDataReader loanReader = dbHelper.ExecuteReader("Get_Loan_Info_By_Id", DBHelper.QueryType.StotedProcedure, loan_para);

                while (loanReader.Read())
                {
                    loaniobj.LoanApplication_ID = int.Parse(loanReader["LoanApplication_ID"].ToString());
                    loaniobj.Business_ID = int.Parse(loanReader["Business_ID"].ToString());
                    loaniobj.Applicant_ID = int.Parse(loanReader["Applicant_ID"].ToString());
                    loaniobj.LoanApplication_Amount = Convert.ToDecimal(loanReader["LoanAmount"].ToString());
                    loaniobj.LoanApplication_Description = loanReader["LoanDescription"].ToString();
                    loaniobj.LoanApplication_Status = int.Parse(loanReader["LoanStatus"].ToString());
                    loaniobj.LoanApplication_Date = Convert.ToDateTime(loanReader["LoanApplication_Date"].ToString());
                    loaniobj.LoanApplication_BankerComment = loanReader["LoanBanker_Comment"].ToString();
                }

                return loaniobj;
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

        public bool CheckIsClosed(int LoanApplication_ID)
        {
            DBHelper dbHelper = new DBHelper();
            bool Result = false;
            try
            {
                dbHelper.Connect(dbHelper.GetConnStr());

                MySqlParameter[] loan_para = new MySqlParameter[1];
                loan_para[0] = new MySqlParameter("Loan_ID", LoanApplication_ID);

                DataSet dsloan = dbHelper.ExecuteDS("Get_Loan_Info_By_Id", DBHelper.QueryType.StotedProcedure, loan_para);

                if (dsloan.Tables[0].Rows.Count > 0)
                {
                    if (int.Parse(dsloan.Tables[0].Rows[0]["LoanStatus"].ToString()) == 8)// Loan Status 8 i.e. Closed by External Service
                    {
                        Result = true;
                    }
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

        public int Add_LoanInfo(Loan LoanPara)
        {
            DBHelper dbHelper = new DBHelper();
            bool Result = false;
            try
            {
                dbHelper.Connect(dbHelper.GetConnStr());

                MySqlParameter[] loan_para = new MySqlParameter[6];
                loan_para[0] = new MySqlParameter("Applicant_ID", LoanPara.Applicant_ID);
                loan_para[1] = new MySqlParameter("Business_ID", LoanPara.Business_ID);
                loan_para[2] = new MySqlParameter("LoanApplication_Amount", LoanPara.LoanApplication_Amount);
                loan_para[3] = new MySqlParameter("LoanApplication_Description", LoanPara.LoanApplication_Description);
                loan_para[4] = new MySqlParameter("LoanApplication_Status", LoanPara.LoanApplication_Status);
                loan_para[5] = new MySqlParameter("LoanApplication_BankerComment", LoanPara.LoanApplication_BankerComment);

                int r = Convert.ToInt32(dbHelper.ExecuteScalar("Add_LoanInfo", DBHelper.QueryType.StotedProcedure, loan_para));

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

        public bool Update_LoanInfo(Loan LoanPara, int id)
        {
            DBHelper dbHelper = new DBHelper();
            bool Result = false;
            try
            {
                dbHelper.Connect(dbHelper.GetConnStr());

                MySqlParameter[] loan_para = new MySqlParameter[7];
                loan_para[0] = new MySqlParameter("LoanApplication_ID", id);
                loan_para[1] = new MySqlParameter("Applicant_ID", LoanPara.Applicant_ID);
                loan_para[2] = new MySqlParameter("Business_ID", LoanPara.Business_ID);
                loan_para[3] = new MySqlParameter("LoanApplication_Amount", LoanPara.LoanApplication_Amount);
                loan_para[4] = new MySqlParameter("LoanApplication_Description", LoanPara.LoanApplication_Description);
                loan_para[5] = new MySqlParameter("LoanApplication_Status", LoanPara.LoanApplication_Status);
                loan_para[6] = new MySqlParameter("LoanApplication_BankerComment", LoanPara.LoanApplication_BankerComment);

                int r = dbHelper.Execute("Update_LoanInfo", DBHelper.QueryType.StotedProcedure, loan_para);

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

        public bool Delete_LoanInfo(int id)
        {
            DBHelper dbHelper = new DBHelper();
            bool Result = false;
            try
            {
                dbHelper.Connect(dbHelper.GetConnStr());

                MySqlParameter[] loan_para = new MySqlParameter[1];
                loan_para[0] = new MySqlParameter("LoanApplication_ID", id);

                int r = dbHelper.Execute("Delete_LoanInfo", DBHelper.QueryType.StotedProcedure, loan_para);

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
