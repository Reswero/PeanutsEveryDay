using PeanutsEveryDay.Application.Modules.Converters;

namespace PeanutsEveryDay.Infrastructure.Modules.Converters;

public class ComicConverter : IComicImageConverter
{
    private const int _stripWidth = 1000;
    private const int _stripHeigth = 200;
    private const int _maxStripHeight = 300;

    private const int _partWidth = 500;
    private const int _partHeight = 200;
    private const int _partOffset = 2;

    private const int _border = 5;

    private const float _opacity = 1f;

    public async Task ConvertFromStripToSquareAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using Image strip = await Image.LoadAsync(stream, cancellationToken);
        stream.Seek(0, SeekOrigin.Begin);

        if (strip.Height > _maxStripHeight)
        {
            await strip.SaveAsPngAsync(stream, cancellationToken);
            stream.Seek(0, SeekOrigin.Begin);
            return;
        }

        strip.Mutate(i => i.Resize(_stripWidth, _stripHeigth));

        Point startPoint = new(0, 0);
        Point middlePoint = new(_partWidth, 0);

        Size partSize = new(_partWidth, _partHeight);

        using Image topPart = strip.Clone(i => i.Crop(new(startPoint, partSize)));
        using Image bottomPart = strip.Clone(i => i.Crop(new(middlePoint, partSize)));

        using Image<Rgba64> square = new(_partWidth + _border * 2, _partHeight * 2 + _border * 4);
        square.Mutate(i =>
        {
            i.BackgroundColor(Color.White);
            i.DrawImage(topPart, new Point(_border, _border), _opacity);
            i.DrawImage(bottomPart, new Point(_partOffset + _border, _partHeight + _border * 3), _opacity);
        });

        await square.SaveAsPngAsync(stream, cancellationToken);
        stream.Seek(0, SeekOrigin.Begin);
    }
}
