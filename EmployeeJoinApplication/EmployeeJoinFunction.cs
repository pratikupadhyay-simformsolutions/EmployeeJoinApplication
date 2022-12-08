using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace EmployeeJoinApplication
{
    public class EmployeeJoinFunction
    {
        private static readonly string ConnectionString = "Data Source=DESKTOP-JB8A662;Initial Catalog=Employee;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        [FunctionName("EmployeeJoinFunction")]
        public void Run([TimerTrigger("* * * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var employees = new List<Employee>();
            string sql = @"Select * from Employee where CAST(JoiningDate as date) = Cast(GETDATE() as date)";
            try
            {
                var con = new SqlConnection(ConnectionString);
                using (var command = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            employees.Add(new Employee { Id = reader.GetInt32(0), Name = reader.GetString(1), JoiningDate = reader.GetDateTime(2), EmailId = reader.GetString(3) });
                    }
                    con.Close();
                }
                foreach(var employee in employees)
                {
                    log.LogInformation($"Employee Name: {employee.Name}, JoiningDate: {employee.JoiningDate}, Email: {employee.EmailId}. sent sucessfully.");
                }
                log.LogInformation($"C# Timer trigger function executed at sucessfully: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                log.LogInformation($"C# Timer trigger function executed but have error at: {DateTime.Now} : " +
                    $"error Message : {ex.Message} : error StackTrace : {ex.StackTrace}");
                throw ex;
            }
        }
    }
}
