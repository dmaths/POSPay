using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace POSPayGP
{
    class POS
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the check date in this format:  YYYYMMDD ");
            string sDate = Console.ReadLine();
            
            DateTime dDtest;
            if (!DateTime.TryParseExact(sDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dDtest))
            {
                Console.WriteLine("INVALID DATE FORMAT.  Next Time Please Enter Check Date in this format:  YYYYMMDD ");
                    Console.WriteLine("Press enter to close...");
                    Console.ReadLine();
            }

            else
            {
                SqlConnection cDBLOB = new SqlConnection();

                var vConString = ConfigurationManager.ConnectionStrings["DBLOWCon"];
                string sConn = vConString.ConnectionString;
                cDBLOB.ConnectionString = sConn;

                    try
                    {
                        cDBLOB.Open();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }


                SqlDataReader qLOBChecks = null;
                string sMainSQL = System.Configuration.ConfigurationManager.AppSettings["SQLMain"];
                SqlCommand cLOBQuery = new SqlCommand(sMainSQL, cDBLOB);
                cLOBQuery.Parameters.AddWithValue("@Date", sDate);
                qLOBChecks = cLOBQuery.ExecuteReader();

                string sOutLoc = System.Configuration.ConfigurationManager.AppSettings["OutLoc"];
                
                StreamWriter sOutput = new StreamWriter(@sOutLoc + sDate + ".txt");
                {
                    while (qLOBChecks.Read())
                    {
                        sOutput.WriteLine(qLOBChecks[0].ToString());
                    }
                    qLOBChecks.Close();
                    sOutput.Flush();
                    sOutput.Close();
                }

                int iCount = 0;
                string sCountSQL = System.Configuration.ConfigurationManager.AppSettings["SQLCount"];
                SqlCommand cCount = new SqlCommand(sCountSQL, cDBLOB);
                cCount.Parameters.AddWithValue("@Date", sDate);
                iCount = (int)cCount.ExecuteScalar();

                decimal dSum = 0;
                string sSumSQL = System.Configuration.ConfigurationManager.AppSettings["SQLSum"];
                SqlCommand cSum = new SqlCommand(sSumSQL, cDBLOB);
                cSum.Parameters.AddWithValue("@Date", sDate);
                dSum = (decimal)cSum.ExecuteScalar();

                cDBLOB.Close();

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("File has been created in the following directory:");
                Console.WriteLine();
                Console.WriteLine(@sOutLoc + sDate + ".txt");
                Console.WriteLine();
                Console.WriteLine("Check Count = " + iCount);
                Console.WriteLine("Check Sum = $" + dSum);
                Console.WriteLine("Press enter to close program.");
                Console.ReadLine();
               
            }
        }
    }
}



          

