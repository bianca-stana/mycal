using System;
using System.ComponentModel;
using System.Globalization;
using System.Management;
using MyCal.Properties;

namespace MyCal
{
    internal class ViewModel : INotifyPropertyChanged
    {
        #region Properties

        private bool _digitGrouping = (bool)Settings.Default["DigitGrouping"];
        public bool DigitGrouping
        {
            get => _digitGrouping;
            set
            {
                if (_digitGrouping == value)
                {
                    return;
                }

                _digitGrouping = value;
                NotifyPropertyChanged(nameof(DigitGrouping));
            }
        }

        private bool _writingDecimals;
        public bool WritingDecimals
        {
            get => _writingDecimals;
            set
            {
                if (_writingDecimals == value)
                {
                    return;
                }

                _writingDecimals = value;
                NotifyPropertyChanged(nameof(CurrentValueString));
            }
        }

        private int _addTrailingZeros;
        public int AddTrailingZeros
        {
            get => _addTrailingZeros;
            set
            {
                if (_addTrailingZeros == value)
                {
                    return;
                }

                _addTrailingZeros = value;
                NotifyPropertyChanged(nameof(CurrentValueString));
            }
        }

        private double _currentValue;
        public double CurrentValue
        {
            get => _currentValue;
            set
            {
                if (_currentValue == value)
                {
                    return;
                }

                _currentValue = value;
                NotifyPropertyChanged(nameof(CurrentValueString));
            }
        }

        public string CurrentValueString
        {
            get
            {
                var returnValue = DigitGrouping
                    ? CurrentValue.ToString("#,0.#", CultureInfo.CurrentCulture)
                    : CurrentValue.ToString(CultureInfo.InvariantCulture);

                if (WritingDecimals && (int)CurrentValue == CurrentValue)
                {
                    returnValue = returnValue + ".";
                }

                var trailingZeros = AddTrailingZeros;
                while (trailingZeros > 0)
                {
                    returnValue += "0";
                    trailingZeros--;
                }

                return returnValue;
            }
        }

        public double MemoryValue { get; set; }

        public double Result;

        private string _operationsString = string.Empty;
        public string OperationsString
        {
            get => _operationsString;
            set
            {
                if (_operationsString == value)
                {
                    return;
                }

                _operationsString = value;
                NotifyPropertyChanged(nameof(OperationsString));
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage == value)
                {
                    return;
                }

                _errorMessage = value;
                NotifyPropertyChanged(nameof(ErrorMessage));
            }
        }

        private int _physicalProcessorCount;
        public double PhysicalProcessorCount
        {
            get
            {
                foreach (var item in new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
                    _physicalProcessorCount += int.Parse(item["NumberOfProcessors"].ToString());

                return _physicalProcessorCount;
            }
        }

        public double LogicalProcessorCount => Environment.ProcessorCount;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Methods

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
