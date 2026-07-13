using System;
using System.Windows;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using pdf_jpg.Services;

namespace pdf_jpg;

public partial class MainWindow : Window
{
    private readonly PdfConversionService _conversionService;

    public MainWindow()
    {
        InitializeComponent();
        _conversionService = new PdfConversionService();
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
            }
        }
        e.Handled = true;
    }
}