using System;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;

namespace pdf_jpg.Services;

public class PdfConversionService
{
    private const int DPI = 300;
    private const int JPEG_QUALITY = 90;

    public async Task ConvertPdfToJpgAsync(string pdfPath, string outputFolder)
    {
        if (!File.Exists(pdfPath))
            throw new FileNotFoundException($"PDFファイルが見つかりません: {pdfPath}");

        if (!Directory.Exists(outputFolder))
            throw new DirectoryNotFoundException($"出力先フォルダが見つかりません: {outputFolder}");

        await Task.Run(() => ExecuteConversion(pdfPath, outputFolder));
    }

    private void ExecuteConversion(string pdfPath, string outputFolder)
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pdfPath);

        // PDF 読み込み設定（250DPI）
        var settings = new MagickReadSettings
        {
            Density = new Density(DPI, DPI),
            Format = MagickFormat.Pdf,
        };

        try
        {
            // 全ページを一度に読み込む
            using (var images = new MagickImageCollection())
            {
                images.Read(pdfPath, settings);

                int pageCount = images.Count;

                for (int i = 0; i < pageCount; i++)
                {
                    var image = images[i];

                    // 白背景を設定（PDFの透過部分を白にする）
                    image.BackgroundColor = MagickColors.White;
                    image.Alpha(AlphaOption.Remove);
                    image.Format = MagickFormat.Jpeg;
                    image.Quality = JPEG_QUALITY;

                    // ファイル名を決定
                    string outputFileName = pageCount == 1
                        ? $"{fileNameWithoutExtension}.jpg"
                        : $"{fileNameWithoutExtension}_page{i + 1}.jpg";

                    string outputPath = Path.Combine(outputFolder, outputFileName);

                    image.Write(outputPath);
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"PDF変換処理でエラーが発生しました: {ex.Message}", ex);
        }
    }
}