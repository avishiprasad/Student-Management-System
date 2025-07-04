using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using WebApplication4.Models;
using Microsoft.Data.SqlClient;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Net.Http;
using WebApplication4;
using Microsoft.Extensions.Options;



namespace WebApplication4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly AppSettings _appSettings;
        public StudentController(IConfiguration configuration, IOptions<AppSettings> appSettings)
        {
            _connectionString = configuration.GetConnectionString("dbcs");
            _appSettings = appSettings.Value;
        }
        // POST: api/Student
        [HttpPost]
        public IActionResult AddStudent([FromBody] StudentRequest request)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. Insert or get State
                    int stateId;
                    using (SqlCommand cmd = new SqlCommand("SELECT StateId FROM pstates WHERE StateName = @StateName", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@StateName", request.State);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                            stateId = (int)result;
                        else
                        {
                            cmd.CommandText = "INSERT INTO pstates (StateName) OUTPUT INSERTED.StateId VALUES (@StateName)";
                            stateId = (int)cmd.ExecuteScalar();
                        }
                    }

                    // 2. Insert or get City
                    int cityId;
                    using (SqlCommand cmd = new SqlCommand("SELECT CityId FROM pcities WHERE CityName = @CityName AND StateId = @StateId", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@CityName", request.City);
                        cmd.Parameters.AddWithValue("@StateId", stateId);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                            cityId = (int)result;
                        else
                        {
                            cmd.CommandText = "INSERT INTO pcities (CityName, StateId) OUTPUT INSERTED.CityId VALUES (@CityName, @StateId)";
                            cityId = (int)cmd.ExecuteScalar();
                        }
                    }

                    // 3. Insert or get College
                    int collegeId;
                    using (SqlCommand cmd = new SqlCommand("SELECT CollegeId FROM pcolleges WHERE CollegeYear = @Year", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Year", request.CollegeYear);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                            collegeId = (int)result;
                        else
                        {
                            cmd.CommandText = "INSERT INTO pcolleges (CollegeYear) OUTPUT INSERTED.CollegeId VALUES (@Year)";
                            collegeId = (int)cmd.ExecuteScalar();
                        }
                    }

                    // 4. Insert Student
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO pstudents (Name, CityId, CollegeId) VALUES (@Name, @CityId, @CollegeId)", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Name", request.Name);
                        cmd.Parameters.AddWithValue("@CityId", cityId);
                        cmd.Parameters.AddWithValue("@CollegeId", collegeId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return Ok("Student added successfully.");
                }
                catch
                {
                    transaction.Rollback();
                    return StatusCode(500, "An error occurred while inserting the student.");
                }
            }
        }

        // GET: api/Student
        [HttpGet]
        public IActionResult GetStudents()
        {
            List<StudentResponse> students = new List<StudentResponse>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT s.Name, c.CityName, st.StateName, col.CollegeYear
                    FROM pstudents s
                    JOIN pcities c ON s.CityId = c.CityId
                    JOIN pstates st ON c.StateId = st.StateId
                    JOIN pcolleges col ON s.CollegeId = col.CollegeId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(new StudentResponse
                        {
                            Name = reader["Name"].ToString(),
                            City = reader["CityName"].ToString(),
                            State = reader["StateName"].ToString(),
                            CollegeYear = Convert.ToInt32(reader["CollegeYear"])
                        });
                    }
                }
            }

            return Ok(students);
        }

        /* [HttpGet("by-id")]
        public IActionResult GetStudentByQuery([FromQuery] int id)
        {
            StudentResponse student = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
        SELECT s.Name, c.CityName, st.StateName, col.CollegeYear
        FROM pstudents s
        JOIN pcities c ON s.CityId = c.CityId
        JOIN pstates st ON c.StateId = st.StateId
        JOIN pcolleges col ON s.CollegeId = col.CollegeId
        WHERE s.StudentId = @StudentId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentId", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            student = new StudentResponse
                            {
                                Name = reader["Name"].ToString(),
                                City = reader["CityName"].ToString(),
                                State = reader["StateName"].ToString(),
                                CollegeYear = Convert.ToInt32(reader["CollegeYear"])
                            };
                        }
                    }
                }
            }

            if (student == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            return Ok(student);
        } */

        [HttpGet("by-name")]
        public IActionResult GetStudentByName([FromQuery] string name)
        {
            StudentResponse student = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
            SELECT s.Name, c.CityName, st.StateName, col.CollegeYear
            FROM pstudents s
            JOIN pcities c ON s.CityId = c.CityId
            JOIN pstates st ON c.StateId = st.StateId
            JOIN pcolleges col ON s.CollegeId = col.CollegeId
            WHERE s.Name = @Name";  // Match by name now

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            student = new StudentResponse
                            {
                                Name = reader["Name"].ToString(),
                                City = reader["CityName"].ToString(),
                                State = reader["StateName"].ToString(),
                                CollegeYear = Convert.ToInt32(reader["CollegeYear"])
                            };
                        }
                    }
                }
            }

            if (student == null)
            {
                return NotFound($"Student with name '{name}' not found.");
            }

            return Ok(student);
        }

        [HttpDelete("by-name")]
        public IActionResult DeleteStudentByName([FromQuery] string name)
        {
            int rowsAffected = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM pstudents WHERE Name = @Name";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            if (rowsAffected == 0)
            {
                return NotFound($"No student found with name '{name}' to delete.");
            }

            return Ok($"Student '{name}' deleted successfully.");
        }
        [HttpGet("export")]
        public IActionResult ExportStudentsToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new OfficeOpenXml.ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Students");

            // Header row
            worksheet.Cells[1, 1].Value = "Name";
            worksheet.Cells[1, 2].Value = "City";
            worksheet.Cells[1, 3].Value = "State";
            worksheet.Cells[1, 4].Value = "College Year";

            var students = new List<StudentResponse>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
            SELECT s.Name, c.CityName, st.StateName, col.CollegeYear
            FROM pstudents s
            JOIN pcities c ON s.CityId = c.CityId
            JOIN pstates st ON c.StateId = st.StateId
            JOIN pcolleges col ON s.CollegeId = col.CollegeId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(new StudentResponse
                        {
                            Name = reader["Name"].ToString(),
                            City = reader["CityName"].ToString(),
                            State = reader["StateName"].ToString(),
                            CollegeYear = Convert.ToInt32(reader["CollegeYear"])
                        });
                    }
                }
            }

            // Fill data
            for (int i = 0; i < students.Count; i++)
            {
                var s = students[i];
                worksheet.Cells[i + 2, 1].Value = s.Name;
                worksheet.Cells[i + 2, 2].Value = s.City;
                worksheet.Cells[i + 2, 3].Value = s.State;
                worksheet.Cells[i + 2, 4].Value = s.CollegeYear;
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            string excelName = $"Students_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        
        

    }
}
   
