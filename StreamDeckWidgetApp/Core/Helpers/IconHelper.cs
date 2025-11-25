using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace StreamDeckWidgetApp.Core.Helpers;

public static class IconHelper
{
    // --- Win32 API Tanımlamaları ---
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);
    
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern uint PrivateExtractIcons(string lpszFile, int nIconIndex, int cxIcon, int cyIcon, IntPtr[] phicon, int[] piconid, uint nIcons, uint flags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);

    public static string? ExtractAndSaveIcon(string sourceFilePath, string saveFolder)
    {
        try
        {
            if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

            System.Diagnostics.Debug.WriteLine($"İkon çıkarma başladı: {sourceFilePath}");

            // .lnk dosyaları için hedef exe'yi bul
            string targetPath = sourceFilePath;
            if (Path.GetExtension(sourceFilePath).Equals(".lnk", StringComparison.OrdinalIgnoreCase))
            {
                targetPath = GetShortcutTarget(sourceFilePath) ?? sourceFilePath;
                System.Diagnostics.Debug.WriteLine($"Kısayol hedefi: {targetPath}");
            }

            // 1. Önce en yüksek kaliteyi (256x256) almaya çalış
            Bitmap? bitmap = ExtractIconFromExe(targetPath, 256);

            // 2. Eğer 256 yoksa, 128 dene, o da yoksa 48 dene
            if (bitmap == null) bitmap = ExtractIconFromExe(targetPath, 128);
            if (bitmap == null) bitmap = ExtractIconFromExe(targetPath, 48);

            // 3. Hiçbiri yoksa standart yönteme başvur (Fallback)
            if (bitmap == null)
            {
                System.Diagnostics.Debug.WriteLine("Fallback metoda geçiliyor...");
                using var fallbackIcon = Icon.ExtractAssociatedIcon(targetPath);
                if (fallbackIcon != null) bitmap = fallbackIcon.ToBitmap();
            }

            if (bitmap == null)
            {
                System.Diagnostics.Debug.WriteLine("İkon çıkarılamadı!");
                return null;
            }

            // Dosya ismi oluştur
            string fileName = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}_{Guid.NewGuid().ToString().Substring(0, 5)}.png";
            string fullPath = Path.Combine(saveFolder, fileName);

            // 4. PNG olarak kaydet (Yüksek Kalite)
            bitmap.Save(fullPath, ImageFormat.Png);
            bitmap.Dispose();

            System.Diagnostics.Debug.WriteLine($"İkon başarıyla kaydedildi: {fullPath}");
            return fullPath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"İkon Hatası: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            return null;
        }
    }

    // Kısayol (.lnk) dosyasının hedefini bulan metot
    private static string? GetShortcutTarget(string shortcutPath)
    {
        try
        {
            var shell = Type.GetTypeFromProgID("WScript.Shell");
            if (shell == null) return null;

            dynamic? shellInstance = Activator.CreateInstance(shell);
            if (shellInstance == null) return null;

            var shortcut = shellInstance.CreateShortcut(shortcutPath);
            string targetPath = shortcut.TargetPath;
            
            System.Runtime.InteropServices.Marshal.ReleaseComObject(shortcut);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(shellInstance);

            return string.IsNullOrEmpty(targetPath) ? null : targetPath;
        }
        catch
        {
            return null;
        }
    }

    // Windows API kullanarak belirli boyutta ikon çeken metot
    private static Bitmap? ExtractIconFromExe(string path, int size)
    {
        try
        {
            if (!File.Exists(path))
            {
                System.Diagnostics.Debug.WriteLine($"Dosya bulunamadı: {path}");
                return null;
            }

            IntPtr[] phicon = new IntPtr[1];
            int[] piconid = new int[1];

            // API Çağrısı: Dosyadan belirtilen boyutta (size x size) ikonu belleğe al
            uint count = PrivateExtractIcons(path, 0, size, size, phicon, piconid, 1, 0);

            System.Diagnostics.Debug.WriteLine($"{size}x{size} için çıkarım sonucu: {count} ikon bulundu");

            if (count > 0 && phicon[0] != IntPtr.Zero)
            {
                // Handle'dan gerçek ikonu oluştur
                using var icon = Icon.FromHandle(phicon[0]);
                
                // Bitmap'e kopyala
                Bitmap bitmap = (Bitmap)icon.ToBitmap().Clone();
                
                // Bellek sızıntısını önlemek için handle'ı temizle
                DestroyIcon(phicon[0]);
                
                System.Diagnostics.Debug.WriteLine($"{size}x{size} bitmap başarıyla oluşturuldu");
                return bitmap;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ExtractIconFromExe hatası ({size}px): {ex.Message}");
            return null;
        }
    }
}
