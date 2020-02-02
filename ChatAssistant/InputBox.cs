using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChatAssistant
{
    class InputBox : TextBox
    {
        //event if new message has to be send
        public event EventHandler<string> TextSubmitted;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Check if we have pressed enter
            if (e.Key == Key.Enter)
            {
                // If we have control pressed...
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    // Add a new line at the point where the cursor is
                    var index = CaretIndex;

                    // Insert the new line
                    Text = Text.Insert(index, Environment.NewLine);

                    // Shift the caret forward to the newline
                    CaretIndex = index + Environment.NewLine.Length;

                    // Mark this key as handled by us
                    e.Handled = true;
                }
                else
                {
                    // Send the message
                    TextSubmitted?.Invoke(this, this.Text);
                    Clear();
                }
                // Mark the key as handled
                e.Handled = true;
            }
        }
    }
}
