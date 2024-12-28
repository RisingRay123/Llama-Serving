using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace llamaServing
{
    [Route("api/[controller]")]
    [ApiController]
    public class llamaController : ControllerBase
    {
        private readonly HttpClient _client;
        HttpRequestMessage request;
        
        public llamaController()
        {
            _client = new HttpClient();
        }
        [HttpGet]
        [Route("useLlamaAPI")]
        public async Task<IActionResult> useLlamaAPI(string prompt)
        {
            string Output = "";
            try
            {
                string requestBody = $"{{\"model\":\"llama3\",\"prompt\":\"{prompt}\",\"stream\":false}}";
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://localhost:11434/api/generate"),
                    Content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json")
                };

                HttpResponseMessage response = await _client.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize JSON response
                // Log the raw JSON response for debugging
                Console.WriteLine("Raw JSON response: " + responseBody);

                // Deserialize JSON response
                var jsonobj=JsonSerializer.Deserialize<llamaServing.Rootobject>(responseBody);
                //var jsonResponses = JsonSerializer.Deserialize<List<JsonResponse>>(responseBody);

                Output = jsonobj.response.ToString();
                Console.WriteLine(Output);

                return Ok(Output);

            }
            catch (FormatException ex)
            {
                return BadRequest($"Input string was not in a correct format: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}
