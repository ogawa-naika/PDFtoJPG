using System;
using System.IO;
using System.Threading.Tasks;
using Ghostscript.NET.Rasterizer;
using SkiaSharp;

namespace pdf_jpg.Services;

public class PdfConversionService
{
    private const int DPI = 250;

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

        using (var rasterizer = new GhostscriptRasterizer())
        {
            // Ghostscript リソースの初期化
            rasterizer.Open(pdfPath);

            try
            {
                int pageCount = rasterizer.PageCount;

                // ページインデックスは 1 から始まる（0ベースではない）
                for (int pageNumber = 1; pageNumber <= pageCount; pageNumber++)
                {
                    // ページをレンダリング（250DPI）
                    var image = rasterizer.GetPage(pageNumber, DPI);

                    // ファイル名を決定
                    string outputFileName = pageCount == 1
                        ? $"{fileNameWithoutExtension}.jpg"
                        : $"{fileNameWithoutExtension}_page{pageNumber}.jpg";

                    string outputPath = Path.Combine(outputFolder, outputFileName);

                    // JPGで保存
                    SaveAsJpeg(image, outputPath);
                    image?.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"PDF変換処理でエラーが発生しました: {ex.Message}", ex);
            }
        }
    }

    private void SaveAsJpeg(SKBitmap bitmap, string outputPath)
    {
        if (bitmap == null)
            throw new ArgumentNullException(nameof(bitmap));

        using (var image = SKImage.FromBitmap(bitmap))
        using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 90))
        using (var stream = File.Create(outputPath))
        {
            data.SaveTo(stream);
        }
    }
}