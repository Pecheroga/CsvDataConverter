using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Converter.Helpers
{
    internal sealed class SelectAllBehavior : Behavior<TextBox>
    {
        public static event EventHandler<TextBox> TxtbEditStarted;

        protected override void OnAttached()
        {
            AssociatedObject.TextChanged += AssociatedObject_TextChanged;
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
            AssociatedObject.GotFocus += AssociatedObject_GotFocus;
            base.OnAttached();
        }

        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtbEditStarted != null)
            {
                TxtbEditStarted.Invoke(this, AssociatedObject);
            }
            base.OnDetaching();
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!AssociatedObject.IsKeyboardFocused)
            {
                AssociatedObject.Focus();
                e.Handled = true; // Отменяет сробатывание кнопки мыши
            }
            base.OnDetaching();
        }

        private void AssociatedObject_GotFocus(object sender, RoutedEventArgs e)
        {
            AssociatedObject.SelectAll();
            base.OnDetaching();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.GotFocus -= AssociatedObject_GotFocus;
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
            AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
        }
    }
}
