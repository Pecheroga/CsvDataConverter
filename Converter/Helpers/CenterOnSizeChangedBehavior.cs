using System.Windows;

namespace Converter.Helpers
{
    public static class CenterOnSizeChangedBehavior
    {
        public static readonly DependencyProperty CenterOnSizeChangeProperty =
            DependencyProperty.RegisterAttached(
            "CenterOnSizeChange",
            typeof(bool),
            typeof(CenterOnSizeChangedBehavior),
            new UIPropertyMetadata(OnCenterOnSizeChangeProperty));

        private static void OnCenterOnSizeChangeProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window == null) return;
            if ((bool)e.NewValue)
            {
                window.SizeChanged += OnWindowSizeChanged;
            }
            else
            {
                window.SizeChanged -= OnWindowSizeChanged;
            }
        }

        private static void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = (Window)sender;
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Left = (SystemParameters.WorkArea.Width - window.ActualWidth) / 2 + SystemParameters.WorkArea.Left;
            window.Top = (SystemParameters.WorkArea.Height - window.ActualHeight) / 2 + SystemParameters.WorkArea.Top;
        }

        public static bool GetCenterOnSizeChange(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(CenterOnSizeChangeProperty);
        }

        public static void SetCenterOnSizeChange(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(CenterOnSizeChangeProperty, value);
        }
    }
}
