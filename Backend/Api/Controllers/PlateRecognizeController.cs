using Final_year_Project.Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Final_year_Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlateRecognizeController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public PlateRecognizeController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateEntryLogDto dto)
        {
            try
            {
                // Chuyển ảnh từ định dạng base64 sang mảng byte
                var imageBytes = ConvertImageBase64ToBytes(dto.ImageBase64);

                // Gửi ảnh đến API nhận diện biển số
                var resultJson = await SendImageToPlateRecognizerApiAsync(imageBytes);

                // Trích xuất thông tin biển số và độ tin cậy từ kết quả JSON
                var (plate, score) = ExtractPlateInfo(resultJson);

                // Trả kết quả về client (biển số viết hoa, kèm độ tin cậy)
                return Ok(new
                {
                    plate = plate.ToUpper(),
                    confidence = score
                });
            }
            catch (Exception ex)
            {
                // Nếu có lỗi xảy ra thì trả lỗi 500 cùng thông báo
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        // Chuyển chuỗi base64 thành mảng byte ảnh
        private byte[] ConvertImageBase64ToBytes(string base64)
        {
            var base64Data = base64.Split(',')[1]; // Loại bỏ phần "data:image/png;base64,"
            return Convert.FromBase64String(base64Data);
        }

        // Gửi ảnh tới Plate Recognizer API và nhận về kết quả JSON
        private async Task<string> SendImageToPlateRecognizerApiAsync(byte[] imageBytes)
        {
            var client = _httpClientFactory.CreateClient();

            // Gắn token xác thực vào Header
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Token", _config["PlateRecognizer:Token"]);

            // Tạo nội dung multipart chứa ảnh
            var content = new MultipartFormDataContent
            {
                {
                    new ByteArrayContent(imageBytes)
                    {
                        Headers = { ContentType = new MediaTypeHeaderValue("image/png") }
                    },
                    "upload",
                    "plate.png"
                }
            };

            // Gửi request POST đến API và đảm bảo phản hồi thành công
            var response = await client.PostAsync(_config["PlateRecognizer:ApiUrl"], content);
            response.EnsureSuccessStatusCode(); // Ném lỗi nếu mã trạng thái không phải 2xx

            // Trả về nội dung phản hồi (kiểu chuỗi JSON)
            return await response.Content.ReadAsStringAsync();
        }

        // Phân tích chuỗi JSON để lấy biển số và độ chính xác
        private (string plate, double score) ExtractPlateInfo(string jsonResult)
        {
            var json = JsonDocument.Parse(jsonResult);
            var results = json.RootElement.GetProperty("results");

            // Nếu có ít nhất 1 kết quả nhận diện được
            if (results.GetArrayLength() > 0)
            {
                var firstResult = results[0]; // Lấy kết quả đầu tiên
                var plate = firstResult.GetProperty("plate").GetString() ?? ""; // Lấy biển số
                var score = firstResult.GetProperty("score").GetDouble(); // Lấy độ tin cậy
                return (plate, score);
            }

            // Nếu không nhận diện được gì
            return ("", 0.0);
        }
    }
}
