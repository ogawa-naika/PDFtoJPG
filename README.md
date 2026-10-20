# PDF to JPG 変換ツール

WPF ベースの PDF から JPG への変換アプリケーションです。
ドラッグ&ドロップ対応で、簡単に複数ページの PDF を個別の JPG ファイルに変換できます。

## 機能

- **ドラッグ&ドロップ対応** - PDFファイルをウィンドウにドラッグ&ドロップで読み込み
- **250DPI 固定** - 高品質な画像出力（250DPI）
- **複数ページ対応** - PDFの全ページを個別JPGに自動変換
- **シンプルGUI** - 直感的で使いやすいインターフェース
- **バッチ処理** - ファイル選択ダイアログにも対応

## 必要要件

### システム要件
- **OS**: Windows 10/11
- **ランタイム**: .NET 10 以上
- **Ghostscript**: 別途インストール必要

### Ghostscript インストール

このアプリケーションは Ghostscript を内部で使用します。以下の手順でインストールしてください。

1. **公式サイトからダウンロード**
   - https://www.ghostscript.com/download/gsdnld.html

2. **Windows 版をインストール**
   - `gs10xx.0w64.exe` （64ビット推奨）

3. **確認**
   - コマンドプロンプトを開き、以下のコマンドを実行します。
     ```
     gswin64c -version
     ```
   - バージョン情報が表示されれば OK です。

## 使用方法
1. **アプリケーション起動**

2. **PDFファイルを選択**
- 方法1: PDFをウィンドウにドラッグ&ドロップ
- 方法2: 「参照」ボタンをクリックしてファイルを選択

3. **出力先フォルダを選択**
- 「参照」ボタンをクリックして、JPGの出力先を指定
- %LocalAppData%\pdf_jpg\last-output-folder.txt に最後に指定したフォルダが保存されます

4. **変換実行**
- 「JPGに変換」ボタンをクリック

5. **完了**
- 変換完了メッセージが表示されます
- 指定したフォルダにJPGファイルが出力されます

## 技術仕様

### 変換設定
- **DPI**: 250（固定）
- **JPEG品質**: 90%
- **カラーモード**: RGB
- **背景**: 白色（透過部分を白で処理）

### 使用ライブラリ
- **Magick.NET-Q16-AnyCPU** - PDF/画像処理
- **Ookii.Dialogs.Wpf** - ファイルダイアログ

## ライセンス
MIT License - 詳細は [LICENSE.txt](LICENSE.txt) を参照してください。

## 依存ライブラリのライセンス
- **Magick.NET** - Apache License 2.0
- **Ookii.Dialogs.Wpf** - BSD 3-Clause License

## 開発環境
- **IDE**: Visual Studio Community 2026
- **言語**: C# 14.0
- **フレームワーク**: .NET 10
- **UI**: WPF (Windows Presentation Foundation)


**作成**
- 2026/10/20 Ver 1.2.0
- 2026/07/13 Ver 1.1.0
- 2026/04/23 Ver 1.0.0
