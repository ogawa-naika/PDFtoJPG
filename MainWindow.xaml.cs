using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using pdf_jpg.Services;

namespace pdf_jpg;

public partial class MainWindow : Window
{
    private readonly PdfConversionService _conversionService;
    private readonly string _settingsFilePath =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "pdf_jpg",
            "last-output-folder.txt");

    public MainWindow()
    {
        InitializeComponent();
        _conversionService = new PdfConversionService();

        LoadLastOutputFolder();
    }

    private void LoadLastOutputFolder()
    {
        try
        {
            if (!File.Exists(_settingsFilePath))
            {
                return;
            }

            string path = File.ReadAllText(_settingsFilePath).Trim();
            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                OutputFolderTextBox.Text = path;
            }
        }
        catch
        {
            // 読み込み失敗時は無視（アプリは継続）
        }
    }

    private void SaveLastOutputFolder(string folderPath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                return;
            }

            string? dir = Path.GetDirectoryName(_settingsFilePath);
            if (!string.IsNullOrWhiteSpace(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(_settingsFilePath, folderPath);
        }
        catch
        {
            // 保存失敗時は無視（アプリは継続）
        }
    }

    private void Window_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
        }
        e.Handled = true;
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0 && files[0].EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                PdfPathTextBox.Text = files[0];
                PdfPathTextBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "PDFファイル (*.pdf)|*.pdf|すべてのファイル (*.*)|*.*",
            Title = "PDFファイルを選択"
        };

        if (dialog.ShowDialog() == true)
        {
            PdfPathTextBox.Text = dialog.FileName;
            PdfPathTextBox.Foreground = System.Windows.Media.Brushes.Black;
        }
    }

    private void OutputFolderButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new VistaFolderBrowserDialog
        {
            Description = "出力先フォルダを選択してください"
        };

        if (dialog.ShowDialog(this) == true)
        {
            OutputFolderTextBox.Text = dialog.SelectedPath;
            SaveLastOutputFolder(dialog.SelectedPath);
        }
    }

    private async void ConvertButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(PdfPathTextBox.Text) || PdfPathTextBox.Text == "ここにPDFをドラッグ&ドロップ")
        {
            MessageBox.Show("PDFファイルを選択してください。", "確認", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (string.IsNullOrWhiteSpace(OutputFolderTextBox.Text))
        {
            MessageBox.Show("出力先フォルダを選択してください。", "確認", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        ConvertButton.IsEnabled = false;
        StatusTextBlock.Text = "変換中...";

        try
        {
            await _conversionService.ConvertPdfToJpgAsync(
                PdfPathTextBox.Text,
                OutputFolderTextBox.Text);

            StatusTextBlock.Text = "✓ 変換完了しました！";
            MessageBox.Show("変換が完了しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = "✗ エラーが発生しました";
            MessageBox.Show($"エラー: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            ConvertButton.IsEnabled = true;
        }
    }

    // ---- PDF テキストボックス用 D&D ----

    private void PdfPathTextBox_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0 && files[0].EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
                return;
            }
        }
        e.Effects = DragDropEffects.None;
        e.Handled = true;
    }

    private void PdfPathTextBox_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0 && files[0].EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                PdfPathTextBox.Text = files[0];
                PdfPathTextBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }
        e.Handled = true;
    }

    // ---- 出力フォルダ テキストボックス用 D&D ----

    private void OutputFolderTextBox_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0 && System.IO.Directory.Exists(files[0]))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
                return;
            }
        }
        e.Effects = DragDropEffects.None;
        e.Handled = true;
    }

    private void OutputFolderTextBox_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0 && System.IO.Directory.Exists(files[0]))
            {
                OutputFolderTextBox.Text = files[0];
                SaveLastOutputFolder(files[0]);
            }
        }
        e.Handled = true;
    }
}