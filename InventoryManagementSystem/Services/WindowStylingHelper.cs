using System.Windows;
using System.Windows.Media;

namespace InventoryManagementSystem.Services
{
    public static class WindowStylingHelper
    {
        public static void SetDefaultFontFamily(Window window)
        {
            window.FontFamily = new FontFamily(Properties.Settings.Default.AppFontFamily);
        }
    }
}
