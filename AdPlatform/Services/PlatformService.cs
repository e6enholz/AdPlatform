using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace AdPlatform.Services
{
    public class PlatformService
    {
        private readonly ILogger<PlatformService> _logger;

        private Dictionary<string, List<string>> _platforms = new Dictionary<string,
            List<string>>(StringComparer.OrdinalIgnoreCase);

        public PlatformService(ILogger<PlatformService> logger)
        {
            _logger = logger;
        }

        //загрузка файла
        public async Task<int> LoadFromFile(IFormFile file)
        {
            _platforms.Clear();

            _logger.LogInformation("Загрузка {FileName}", file.FileName);

            using var reader = new StreamReader(file.OpenReadStream());

            while (!reader.EndOfStream)
            {
                var line = (await reader.ReadLineAsync())?.Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(':', 2);
                if (parts.Length != 2)
                {
                    _logger.LogWarning("Пропущена строка {Line}", line);
                    continue;
                }

                string platformName = parts[0].Trim();
                string locationsStr = parts[1].Trim();

                var locations = locationsStr.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var loc in locations)
                {
                    string locTrimmed = loc.Trim();
                    if (!_platforms.ContainsKey(locTrimmed))
                        _platforms[locTrimmed] = new List<string>();

                    _platforms[locTrimmed].Add(platformName);
                    
                }
                
            }
            int count = _platforms.Values.SelectMany(v => v).Distinct(StringComparer.OrdinalIgnoreCase).Count();
            _logger.LogInformation("Файл загружен, найдено {Count} площадок", count);
            return count;
        }

        //поиск площадок
        public List<string> FindPlatforms(string location)
        {
            var timer=Stopwatch.StartNew();

            _logger.LogInformation("Поиск площадок для локации: {Location}", location);

            var rez = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(location))
            {
                _logger.LogWarning("Пустой ввод");
                return rez.ToList();
            }

            string current = location.Trim();
            if (!current.StartsWith("/"))
                current = "/" + current;
            if (current.Length > 1 && current.EndsWith("/"))
                current = current.Substring(0, current.Length - 1);

            while (current.Length > 0)
            {
                if (_platforms.ContainsKey(current))
                {
                    foreach (var p in _platforms[current])
                        rez.Add(p);
                }

                int lastSlash = current.LastIndexOf('/');
                if (lastSlash <= 0) break;
                current = current.Substring(0, lastSlash);
            }

            if (_platforms.ContainsKey("/"))
            {
                foreach (var p in _platforms["/"])
                    rez.Add(p);
            }

            timer.Stop();   
            _logger.LogInformation("Для локации {Location} найдено {Count} площадок. Время выполнения: {Ellapsed} ms",
                location, rez.Count, timer.ElapsedMilliseconds);
            return rez.ToList();
        }
    }
}
