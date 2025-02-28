using System;
using System.Threading.Tasks;

using TesteUnecont.Api.Services;

using Xunit;

namespace TesteUnecont.Test
{
    public class LogConverterTest
    {
        [Fact]
        public async Task ConvertLogCdnToAgora_DeveRetornar_LogConvertido()
        {
            //Arrange
            DateTime timeStamp = DateTime.UtcNow;

            string logCdn =
                "312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2\r\n" +
                "101|200|MISS|\"POST /myImages HTTP/1.1\"|319.4\r\n" +
                "199|404|MISS|\"GET /not-found HTTP/1.1\"|142.9\r\n" +
                "312|200|INVALIDATE|\"GET /robots.txt HTTP/1.1\"|245.1";

            string logAgora =
                "#Version: 1.0\r\n" +
                $"#Date: {timeStamp}\r\n" +
                "#Fields: provider http-method status-code uri-path time-taken response-size cache-status\r\n" +
                "\"MINHA CDN\" GET 200 /robots.txt 100 312 HIT\r\n" +
                "\"MINHA CDN\" POST 200 /myImages 319 101 MISS\r\n" +
                "\"MINHA CDN\" GET 404 /not-found 143 199 MISS\r\n" +
                "\"MINHA CDN\" GET 200 /robots.txt 245 312 REFRESH_HIT";

            LogConverter service = new LogConverter();

            //Act
            var result = await service.ConvertLogCdnToAgora(logCdn, timeStamp);

            //Assert
            Assert.Equal(logAgora, result);
        }
    }
}
