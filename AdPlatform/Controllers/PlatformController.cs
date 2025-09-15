using AdPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdPlatform.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlatformController : ControllerBase
    {
        private readonly PlatformService _service;
        public PlatformController(PlatformService service)
        { 
            _service = service; 
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile? file)
        {
            if (file == null) 
                return BadRequest(new { message = "Файл обязателен" });
            if (file.Length == 0)
                return BadRequest(new { message = "Ошибка: загружен пустой файл" });

            var count = await _service.LoadFromFile(file);

            if (count == 0)
                return BadRequest(new { message = "Ошибка: файл не содержит валидных данных" });

            await _service.LoadFromFile(file);
            return Ok(new { message = "Файл загружен успешно" });
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery] string? location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest(new { message = "Необходимо ввести локацию" });

            var platforms = _service.FindPlatforms(location);

            if (platforms == null || !platforms.Any())
                return Ok(new { message = "Ничего не найдено, проверьте введённые данные и повторите попытку" });

            return Ok(platforms);
        }
    }
}
