using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Button = System.Windows.Controls.Button;
using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;
using Clipboard = System.Windows.Clipboard;
using DataFormats = System.Windows.DataFormats;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace MyCal
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // BUG: Executing operations & saving intermediate results (solved?)
        // OPTIONAL: Make number smaller as it gets longer
        // OPTIONAL: Consider the order of the operations as an option in the app menu

        // User settings file: C:\Users\bianc\AppData\Local\MyCal\MyCal.exe_Url_mcc11cmhbwrt4b3dj2gtc4ve4n1tg4ts\1.0.0.0


        #region Properties

        private readonly ViewModel _model = new ViewModel();
        private double _temporaryValue;
        private string _previousExecutedOperation = string.Empty;
        private string _lastExecutedOperation = string.Empty;
        private string _lastClickedButton = " ";

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _model;

            KeyDown += OnKeyDown;

            var notifyIcon = new NotifyIcon
            {
                Icon = new Icon("calculator.ico"),
                Visible = true
            };

            notifyIcon.DoubleClick +=
                delegate
                {
                    Show();
                    WindowState = WindowState.Normal;
                };
        }

        #endregion

        #region Protected Methods

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }

            base.OnStateChanged(e);
        }

        #endregion

        #region Private Methods

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D0:
                case Key.NumPad0:
                    Button0.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.D1:
                case Key.NumPad1:
                    Button1.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.D2:
                case Key.NumPad2:
                    Button2.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.D3:
                case Key.NumPad3:
                    Button3.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.D4:
                case Key.NumPad4:
                    Button4.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.D5:
                case Key.NumPad5:
                    Button5.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.D6:
                case Key.NumPad6:
                    Button6.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.D7:
                case Key.NumPad7:
                    Button7.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.D8:
                case Key.NumPad8:
                    Button8.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.D9:
                case Key.NumPad9:
                    Button9.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.Add:
                    AddButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.Subtract:
                    SubstractButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.Multiply:
                    MultiplyButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.Divide:
                    DivideButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.Enter:
                    EqualsButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.Back:
                    BackspaceButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.Escape:
                    CButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
                case Key.Decimal:
                    PointButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    break;
            }
        }

        private void DigitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button))
            {
                return;
            }

            if (ErrorLabel.Visibility == Visibility.Visible)
            {
                HandleArithmeticError_After();
            }

            var digit = button.Content.ToString();

            if ("+-×÷=".Contains(_lastClickedButton))
            {
                _model.CurrentValue = 0;
            }

            if (_model.WritingDecimals == false)
            {
                _model.CurrentValue = _model.CurrentValue * 10 + double.Parse(digit);
            }
            else
            {
                if (digit != "0")
                {
                    var currentValueParts = _model.CurrentValueString.Split('.');
                    var decimalPosition = 1;
                    if (currentValueParts.Length > 1)
                    {
                        decimalPosition += currentValueParts[1].Length;
                    }

                    _model.CurrentValue = _model.CurrentValue + double.Parse(digit) / Math.Pow(10, decimalPosition);
                    _model.AddTrailingZeros = 0;
                }
                else
                {
                    _model.AddTrailingZeros++;
                }
            }

            _temporaryValue = _model.CurrentValue;

            if (_lastExecutedOperation == string.Empty)
            {
                _model.Result = _model.CurrentValue;
            }

            _lastClickedButton = digit;
        }

        private void SignButton_Click(object sender, RoutedEventArgs e)
        {
            _model.CurrentValue = -_model.CurrentValue;
            _temporaryValue = _model.CurrentValue;

            if (_lastExecutedOperation == string.Empty)
            {
                _model.Result = _model.CurrentValue;
            }

            _lastClickedButton = "±";
        }

        private void PointButton_Click(object sender, RoutedEventArgs e)
        {
            _model.WritingDecimals = true;

            _lastClickedButton = ".";
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            _model.OperationsString = string.Empty;

            if (ErrorLabel.Visibility == Visibility.Visible)
            {
                HandleArithmeticError_After();
            }

            switch (_lastExecutedOperation)
            {
                case "+":
                    _model.CurrentValue = _model.Result + _temporaryValue;
                    break;
                case "-":
                    _model.CurrentValue = _model.Result - _temporaryValue;
                    break;
                case "×":
                    _model.CurrentValue = _model.Result * _temporaryValue;
                    break;
                case "÷":
                    _model.CurrentValue = _model.Result / _temporaryValue;
                    break;
            }

            if (double.IsNaN(_model.CurrentValue))
            {
                HandleArithmeticError_Before("Result is undefined", "=");
                return;
            }

            if (double.IsInfinity(_model.CurrentValue))
            {
                HandleArithmeticError_Before("Cannot divide by zero", "=");
                return;
            }

            _model.Result = _model.CurrentValue;

            _previousExecutedOperation = string.Empty;
            _lastClickedButton = "=";

            _model.WritingDecimals = false;
            _model.AddTrailingZeros = 0;
        }

        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button))
            {
                return;
            }

            var operation = button.Content.ToString();

            if (!"+-×÷".Contains(_lastClickedButton))
            {
                if (!_model.OperationsString.EndsWith(") ") && _lastClickedButton != "%")
                {
                    _model.OperationsString += _model.CurrentValue.ToString(CultureInfo.InvariantCulture) + " " +
                                               operation + " ";
                }
                else
                {
                    _model.OperationsString += operation + " ";
                }
            }
            else
            {
                _model.OperationsString = _model.OperationsString.Remove(_model.OperationsString.Length - 2);
                _model.OperationsString += operation + " ";
            }

            if (_lastExecutedOperation != string.Empty && !"+-×÷".Contains(_lastClickedButton))
            {
                _previousExecutedOperation = _lastExecutedOperation;

                switch (_previousExecutedOperation)
                {
                    case "+":
                        _model.CurrentValue = _model.Result + _model.CurrentValue;
                        break;
                    case "-":
                        _model.CurrentValue = _model.Result - _model.CurrentValue;
                        break;
                    case "×":
                        _model.CurrentValue = _model.Result * _model.CurrentValue;
                        break;
                    case "÷":
                        _model.CurrentValue = _model.Result / _model.CurrentValue;
                        break;
                }

                if (double.IsNaN(_model.CurrentValue))
                {
                    HandleArithmeticError_Before("Result is undefined", "=");
                    return;
                }

                if (double.IsInfinity(_model.CurrentValue))
                {
                    HandleArithmeticError_Before("Cannot divide by zero", "=");
                    return;
                }

                _model.Result = _model.CurrentValue;
            }

            _lastExecutedOperation = operation;
            _lastClickedButton = operation;
        }

        private void PercentageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_lastClickedButton == "%")
            {
                return;
            }

            _model.CurrentValue = _model.CurrentValue * _model.Result / 100;
            _temporaryValue = _model.CurrentValue;

            if (_lastExecutedOperation == string.Empty)
            {
                _model.Result = _model.CurrentValue;
            }

            _model.OperationsString += _model.CurrentValue + " ";

            _lastClickedButton = "%";
        }

        private void SqrtButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentValue < 0)
            {
                HandleArithmeticError_Before("Invalid input", "√");
                return;
            }

            if (_lastClickedButton != "1/x" && _lastClickedButton != "x²" && _lastClickedButton != "√")
            {
                _model.OperationsString += " √(" + _model.CurrentValue.ToString(CultureInfo.InvariantCulture) + ") ";
            }
            else
            {
                var delimiter = string.Empty;

                switch (_lastClickedButton)
                {
                    case "1/x":
                        delimiter = " 1/(";
                        break;
                    case "x²":
                        delimiter = " sqr(";
                        break;
                    case "√":
                        delimiter = " √(";
                        break;
                }

                var specialOperationString =
                    _model.OperationsString.Substring(
                        _model.OperationsString.LastIndexOf(delimiter, StringComparison.Ordinal));
                specialOperationString = specialOperationString.Trim();

                _model.OperationsString =
                    _model.OperationsString.Substring(0,
                        _model.OperationsString.LastIndexOf(delimiter, StringComparison.Ordinal) + 1);
                _model.OperationsString += "√(" + specialOperationString + ") ";
            }

            _model.CurrentValue = Math.Sqrt(_model.CurrentValue);
            _temporaryValue = _model.CurrentValue;

            if (_lastExecutedOperation == string.Empty)
            {
                _model.Result = _model.CurrentValue;
            }

            _lastClickedButton = "√";
        }

        private void SquareButton_Click(object sender, RoutedEventArgs e)
        {
            if (_lastClickedButton != "1/x" && _lastClickedButton != "x²" && _lastClickedButton != "√")
            {
                _model.OperationsString += " sqr(" + _model.CurrentValue.ToString(CultureInfo.InvariantCulture) + ") ";
            }
            else
            {
                var delimiter = string.Empty;

                switch (_lastClickedButton)
                {
                    case "1/x":
                        delimiter = " 1/(";
                        break;
                    case "x²":
                        delimiter = " sqr(";
                        break;
                    case "√":
                        delimiter = " √(";
                        break;
                }

                var specialOperationString =
                    _model.OperationsString.Substring(
                        _model.OperationsString.LastIndexOf(delimiter, StringComparison.Ordinal));
                specialOperationString = specialOperationString.Trim();

                _model.OperationsString =
                    _model.OperationsString.Substring(0,
                        _model.OperationsString.LastIndexOf(delimiter, StringComparison.Ordinal) + 1);
                _model.OperationsString += "sqr(" + specialOperationString + ") ";
            }

            _model.CurrentValue = Math.Pow(_model.CurrentValue, 2);
            _temporaryValue = _model.CurrentValue;

            if (_lastExecutedOperation == string.Empty)
            {
                _model.Result = _model.CurrentValue;
            }

            _lastClickedButton = "x²";
        }

        private void InverseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentValue == 0)
            {
                HandleArithmeticError_Before("Cannot divide by zero", "1/x");
                return;
            }

            if (_lastClickedButton != "1/x" && _lastClickedButton != "x²" && _lastClickedButton != "√")
            {
                _model.OperationsString += " 1/(" + _model.CurrentValue.ToString(CultureInfo.InvariantCulture) + ") ";
            }
            else
            {
                var delimiter = string.Empty;

                switch (_lastClickedButton)
                {
                    case "1/x":
                        delimiter = " 1/(";
                        break;
                    case "x²":
                        delimiter = " sqr(";
                        break;
                    case "√":
                        delimiter = " √(";
                        break;
                }

                var specialOperationString =
                    _model.OperationsString.Substring(
                        _model.OperationsString.LastIndexOf(delimiter, StringComparison.Ordinal));
                specialOperationString = specialOperationString.Trim();

                _model.OperationsString =
                    _model.OperationsString.Substring(0,
                        _model.OperationsString.LastIndexOf(delimiter, StringComparison.Ordinal) + 1);
                _model.OperationsString += "1/(" + specialOperationString + ") ";
            }

            _model.CurrentValue = 1 / _model.CurrentValue;
            _temporaryValue = _model.CurrentValue;

            if (_lastExecutedOperation == string.Empty)
            {
                _model.Result = _model.CurrentValue;
            }

            _lastClickedButton = "1/x";
        }

        private void CEButton_Click(object sender, RoutedEventArgs e)
        {
            if (ErrorLabel.Visibility == Visibility.Visible)
            {
                HandleArithmeticError_After();
            }

            _model.CurrentValue = 0;

            _model.WritingDecimals = false;
            _model.AddTrailingZeros = 0;

            _lastClickedButton = "CE";
        }

        private void CButton_Click(object sender, RoutedEventArgs e)
        {
            if (ErrorLabel.Visibility == Visibility.Visible)
            {
                HandleArithmeticError_After();
            }

            _model.OperationsString = string.Empty;

            _model.CurrentValue = 0;
            _model.Result = 0;
            _temporaryValue = 0;

            _previousExecutedOperation = string.Empty;
            _lastExecutedOperation = string.Empty;

            _model.WritingDecimals = false;
            _model.AddTrailingZeros = 0;

            _lastClickedButton = "C";
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (ErrorLabel.Visibility == Visibility.Visible)
            {
                HandleArithmeticError_After();
            }

            if (_model.CurrentValueString.IndexOf('.') == _model.CurrentValueString.Length - 1)
            {
                _model.WritingDecimals = false;
                _model.AddTrailingZeros = 0;
            }
            else
            {
                var currentValueString = _model.CurrentValueString;
                currentValueString = currentValueString.Remove(currentValueString.Length - 1);

                _model.AddTrailingZeros = 0;
                while (currentValueString.EndsWith("0"))
                {
                    _model.AddTrailingZeros++;
                    currentValueString = currentValueString.Remove(currentValueString.Length - 1);
                }

                _model.CurrentValue = currentValueString.Length > 0 ? double.Parse(currentValueString) : 0;
            }

            _lastClickedButton = "⌫";
        }

        private void MemoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button))
            {
                return;
            }

            var operation = button.Content.ToString();

            switch (operation)
            {
                case "MC":
                    _model.MemoryValue = 0;
                    McButton.IsEnabled = false;
                    MrButton.IsEnabled = false;
                    break;
                case "MR":
                    _model.CurrentValue = _model.MemoryValue;
                    break;
                case "M+":
                    _model.MemoryValue += _model.CurrentValue;
                    McButton.IsEnabled = true;
                    MrButton.IsEnabled = true;
                    break;
                case "M-":
                    _model.MemoryValue -= _model.CurrentValue;
                    McButton.IsEnabled = true;
                    MrButton.IsEnabled = true;
                    break;
                case "MS":
                    _model.MemoryValue = _model.CurrentValue;
                    McButton.IsEnabled = true;
                    MrButton.IsEnabled = true;
                    break;
            }
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }

        private void CutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplicationCommands.Cut.Execute(null, null);
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplicationCommands.Copy.Execute(null, null);
        }

        private void PasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplicationCommands.Paste.Execute(null, null);
        }

        private void DigitGrouping_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["DigitGrouping"] = true;
            Properties.Settings.Default.Save();
        }

        private void DigitGrouping_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["DigitGrouping"] = false;
            Properties.Settings.Default.Save();
        }

        private void CutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(CurrentValueLabel.Content.ToString());
            _model.CurrentValue = 0;
        }

        private void CopyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(CurrentValueLabel.Content.ToString());
        }

        private void PasteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var text = Clipboard.GetData(DataFormats.Text) as string;

                if (!string.IsNullOrWhiteSpace(text))
                {
                    _model.CurrentValue = int.Parse(text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Paste failed!", MessageBoxButton.OK);
            }
        }

        private void HandleArithmeticError_Before(string errorMessage, string lastClickedButton)
        {
            _model.ErrorMessage = errorMessage;
            CurrentValueLabel.Visibility = Visibility.Hidden;
            ErrorLabel.Visibility = Visibility.Visible;

            _model.CurrentValue = 0;
            _model.Result = 0;
            _temporaryValue = 0;

            _previousExecutedOperation = string.Empty;
            _lastExecutedOperation = string.Empty;

            _lastClickedButton = lastClickedButton;

            _model.WritingDecimals = false;
            _model.AddTrailingZeros = 0;

            McButton.IsEnabled = false;
            MrButton.IsEnabled = false;
            MpButton.IsEnabled = false;
            MmButton.IsEnabled = false;
            MsButton.IsEnabled = false;
            PercentageButton.IsEnabled = false;
            SqrtButton.IsEnabled = false;
            SquaredButton.IsEnabled = false;
            InverseButton.IsEnabled = false;
            DivideButton.IsEnabled = false;
            MultiplyButton.IsEnabled = false;
            SubstractButton.IsEnabled = false;
            AddButton.IsEnabled = false;
            SignButton.IsEnabled = false;
            PointButton.IsEnabled = false;
        }

        private void HandleArithmeticError_After()
        {
            _model.OperationsString = string.Empty;

            _model.ErrorMessage = string.Empty;
            ErrorLabel.Visibility = Visibility.Hidden;
            CurrentValueLabel.Visibility = Visibility.Visible;

            McButton.IsEnabled = _model.MemoryValue == 0 ? false : true;
            MrButton.IsEnabled = _model.MemoryValue == 0 ? false : true;
            MpButton.IsEnabled = true;
            MmButton.IsEnabled = true;
            MsButton.IsEnabled = true;
            PercentageButton.IsEnabled = true;
            SqrtButton.IsEnabled = true;
            SquaredButton.IsEnabled = true;
            InverseButton.IsEnabled = true;
            DivideButton.IsEnabled = true;
            MultiplyButton.IsEnabled = true;
            SubstractButton.IsEnabled = true;
            AddButton.IsEnabled = true;
            SignButton.IsEnabled = true;
            PointButton.IsEnabled = true;
        }

        #endregion
    }
}