using PeanutsEveryDay.Infrastructure.Modules.Converters;

namespace PeanutsEveryDay.Infrastructure.Tests;

public class ComicImageConverterTests
{
    [Fact]
    public async Task Image_ShouldBe_Resized()
    {
        // Arrange
        ComicImageConverter converter = new();

        int border = 5;

        Stream stream = new MemoryStream();
        Image<Rgba64> img = new(1200, 300);
        img.SaveAsPng(stream);
        stream.Seek(0, SeekOrigin.Begin);

        // Act
        var imgStream = await converter.ConvertFromStripToSquareAsync(stream);

        Image converterdImg = Image.Load(imgStream);

        // Assert
        Assert.True(converterdImg.Width == 500 + border * 2);
        Assert.True(converterdImg.Height == 400 + border * 4);
    }

    [Fact]
    public async Task Image_ShouldNotBe_Resized()
    {
        // Arrange
        ComicImageConverter converter = new();

        Stream stream = new MemoryStream();
        Image<Rgba64> img = new(1200, 500);
        img.SaveAsPng(stream);
        stream.Seek(0, SeekOrigin.Begin);

        // Act
        var imgStream = await converter.ConvertFromStripToSquareAsync(stream);

        Image converterdImg = Image.Load(imgStream);

        // Assert
        Assert.True(converterdImg.Width == img.Width);
        Assert.True(converterdImg.Height == img.Height);
    }
}
