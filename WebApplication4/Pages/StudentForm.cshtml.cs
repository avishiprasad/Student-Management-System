using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Net;
using WebApplication4;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;


namespace WebApplication4.Pages
{
    public class StudentFormModel : PageModel
    {
        
        private readonly IHttpClientFactory _clientFactory;
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _configuration;
        public string SsrsReportUrl { get; set; }
        public string StudentApiBaseUrl { get; set; }

        public StudentFormModel(IHttpClientFactory clientFactory, IOptions<AppSettings> appSettings, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _appSettings = appSettings.Value;
            _configuration = configuration;
        }

        // For binding new student input
        [BindProperty]
        public StudentRequest NewStudent { get; set; }

        // For binding search input
        [BindProperty]
        public int SearchId { get; set; }

        // Results and messages to display
        public List<StudentResponse> Students { get; set; } = new();
        public StudentResponse SearchedStudent { get; set; }
        public string ReportViewerUrl { get; set; }
        public string Message { get; set; }
        public string SSRSReportViewerUrl { get; set; }  // 👈 Add this

        public async Task OnGetAsync()
        {
            // Load all students when the page loads
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"{_appSettings.StudentApiBaseUrl}");
            SsrsReportUrl = _configuration["AppSettings:SsrsReportUrl"];
            StudentApiBaseUrl = _appSettings.StudentApiBaseUrl;
            SSRSReportViewerUrl = _configuration["AppSettings:SSRSReportViewerUrl"];

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Students = JsonSerializer.Deserialize<List<StudentResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                Message = "Failed to load students.";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Post a new student
            if (!ModelState.IsValid)
            {
                Message = "Please fill all required fields.";
                return Page();
            }

            var client = _clientFactory.CreateClient();
            var jsonContent = new StringContent(JsonSerializer.Serialize(NewStudent), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_appSettings.StudentApiBaseUrl}", jsonContent);


            if (response.IsSuccessStatusCode)
            {
                Message = "Student added successfully.";
                return RedirectToPage(); // Refresh page and reload all students
            }
            else
            {
                Message = "Failed to add student.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            // Search student by id
            if (SearchId <= 0)
            {
                Message = "Please enter a valid student ID.";
                return Page();
            }

            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"{_appSettings.StudentApiBaseUrl}/by-id?id={SearchId}");


            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                SearchedStudent = JsonSerializer.Deserialize<StudentResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (SearchedStudent == null)
                {
                    Message = $"Student with ID {SearchId} not found.";
                }
                else
                {
                    Message = null;
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Message = $"Student with ID {SearchId} not found.";
                SearchedStudent = null;
            }
            else
            {
                Message = "Error occurred while searching.";
            }

            // Reload all students for display as well
            await OnGetAsync();

            return Page();
        }
    }

    public class StudentRequest
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int CollegeYear { get; set; }
    }

    public class StudentResponse
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int CollegeYear { get; set; }
    }
}
