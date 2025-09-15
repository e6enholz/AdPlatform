using AdPlatform.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.Extensions.Logging;
using System.Text;

namespace AdPlatformTests
{
    public class AdUnitTest
    {
        private async Task<PlatformService> TestData()
        {
            var loggerMock= new Mock<ILogger<PlatformService>>();
            var service = new PlatformService(loggerMock.Object);

            var testData = @"������.������:/ru
                            ���������� �������:/ru/svrd/revda,/ru/svrd/pervik
                            ������ ��������� ���������:/ru/msk,/ru/permobl,/ru/chelobl
                            ������ �������:/ru/svrd";
            var fileMock = new Mock<IFormFile>(); 
            var ms= new MemoryStream(Encoding.UTF8.GetBytes(testData));
            fileMock.Setup(f=>f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f=>f.Length).Returns(ms.Length);

            await service.LoadFromFile(fileMock.Object);
            return service;
        }

        [Fact]
        public async Task FindPlatforms_patchCheck()
        {
            // Arrange 
            var service = await TestData();

            // Act
            var rez = service.FindPlatforms("/ru/msk");

            // Assert
            Assert.Contains("������.������", rez);
            Assert.Contains("������ ��������� ���������", rez);
            Assert.Equal(2, rez.Count);
        }

        [Fact]
        public async Task FindPlatforms_parentPatchCheck()
        {
            // Arrange 
            var service = await TestData();

            // Act
            var rez = service.FindPlatforms("/ru");

            // Assert
            Assert.Contains("������.������", rez);
            Assert.Single(rez); 
        }

        [Fact]
        public async Task FindPlatforms_unknownCheck()
        {
            // Arrange 
            var service = await TestData();

            // Act
            var rez = service.FindPlatforms("/zzz");

            // Assert
            Assert.NotNull(rez);
            Assert.Empty(rez);
        }
    }
}