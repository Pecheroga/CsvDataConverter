using System.Windows;

namespace Converter.Helpers
{
    public static class FocusExtension
    {
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached(
            "IsFocused",
            typeof(bool),
            typeof(FocusExtension),
            new PropertyMetadata(OnIsFocusedPropertyChanged)
            );

        private static void OnIsFocusedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = (UIElement)sender;
            if ((bool)e.NewValue)
            {
                element.Focus();
            }
        }

        public static void SetIsFocused(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(IsFocusedProperty, value);
        }
    }
}
