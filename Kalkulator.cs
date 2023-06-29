using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace kalkulator
{
    public class Kalkulator : INotifyPropertyChanged
    {
        private string _wynik = "0";
        private string _operacja = null;
        private double? _operandLewy = null;
        private double? _operandPrawy = null;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Wynik
        {
            get => _wynik;
            set
            {
                _wynik = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wynik)));
            }
        }

        public string Dzialanie
        {
            get
            {
                if (_operandLewy == null)
                    return "";
                else if (_operandPrawy == null)
                    return $"{_operandLewy} {_operacja}";
                else
                    return $"{_operandLewy} {_operacja} {_operandPrawy} = ";
            }
        }

        public ICommand WprowadzCyfreCommand { get; }
        public ICommand WprowadzPrzecinekCommand { get; }
        public ICommand ZmienZnakCommand { get; }
        public ICommand KasujZnakCommand { get; }
        public ICommand CzyscWszystkoCommand { get; }
        public ICommand CzyscWynikCommand { get; }
        public ICommand WprowadzOperacjaCommand { get; }
        public ICommand WykonajDzialanieCommand { get; }

        public Kalkulator()
        {
            WprowadzCyfreCommand = new RelayCommand<string>(WprowadzCyfre);
            WprowadzPrzecinekCommand = new RelayCommand<object>(WprowadzPrzecinek);
            ZmienZnakCommand = new RelayCommand<object>(ZmienZnak);
            KasujZnakCommand = new RelayCommand<object>(KasujZnak);
            CzyscWszystkoCommand = new RelayCommand<object>(CzyscWszystko);
            CzyscWynikCommand = new RelayCommand<object>(CzyscWynik);
            WprowadzOperacjaCommand = new RelayCommand<string>(WprowadzOperacja);
            WykonajDzialanieCommand = new RelayCommand<object>(WykonajDzialanie);
        }

        private void WprowadzCyfre(string cyfra)
        {
            if (_operacja == null)
            {
                if (_wynik == "0")
                    _wynik = cyfra;
                else
                    _wynik += cyfra;
            }
            else
            {
                if (_operandPrawy == null)
                    _operandPrawy = double.Parse(cyfra);
                else
                    _operandPrawy = double.Parse(_operandPrawy.ToString() + cyfra);
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wynik)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dzialanie)));
        }

        private void WprowadzPrzecinek(object parameter)
        {
            if (_operacja == null)
            {
                if (!_wynik.Contains(","))
                    _wynik += ",";
            }
            else
            {
                if (_operandPrawy == null || !_operandPrawy.ToString().Contains(","))
                {
                    if (_operandPrawy == null)
                        _operandPrawy = 0;
                    _operandPrawy = double.Parse(_operandPrawy.ToString() + ",");
                }
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wynik)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dzialanie)));
        }

        private void ZmienZnak(object parameter)
        {
            if (_operacja == null)
            {
                if (_wynik != "0")
                {
                    if (_wynik.StartsWith("-"))
                        _wynik = _wynik.Substring(1);
                    else
                        _wynik = "-" + _wynik;
                }
            }
            else
            {
                if (_operandPrawy != null)
                    _operandPrawy *= -1;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wynik)));
        }

        private void KasujZnak(object parameter)
        {
            if (_operacja == null)
            {
                if (_wynik.Length > 1)
                    _wynik = _wynik.Substring(0, _wynik.Length - 1);
                else
                    _wynik = "0";
            }
            else
            {
                if (_operandPrawy != null)
                {
                    string operandPrawyStr = _operandPrawy.ToString();
                    if (operandPrawyStr.Length > 1)
                    {
                        operandPrawyStr = operandPrawyStr.Substring(0, operandPrawyStr.Length - 1);
                        _operandPrawy = double.Parse(operandPrawyStr);
                    }
                    else
                    {
                        _operandPrawy = null;
                    }
                }
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wynik)));
        }

        private void CzyscWszystko(object parameter)
        {
            _wynik = "0";
            _operacja = null;
            _operandLewy = null;
            _operandPrawy = null;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wynik)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dzialanie)));
        }

        private void CzyscWynik(object parameter)
        {
            _wynik = "0";

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wynik)));
        }

        private void WprowadzOperacja(string operacja)
        {
            if (_operacja == null)
            {
                _operacja = operacja;
                _operandLewy = double.Parse(_wynik);
                _wynik = "0";
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dzialanie)));
        }

        private void WykonajDzialanie(object parameter)
        {
            if (_operacja != null && _operandLewy != null && _operandPrawy != null)
            {
                switch (_operacja)
                {
                    case "+":
                        _wynik = (_operandLewy + _operandPrawy).ToString();
                        break;
                    case "-":
                        _wynik = (_operandLewy - _operandPrawy).ToString();
                        break;
                    case "*":
                        _wynik = (_operandLewy * _operandPrawy).ToString();
                        break;
                    case "/":
                        if (_operandPrawy != 0)
                            _wynik = (_operandLewy / _operandPrawy).ToString();
                        else
                            _wynik = "Error: Division by zero";
                        break;
                    case "sqrt":
                        _wynik = Math.Sqrt((double)_operandLewy).ToString();
                        break;
                    case "pow":
                        _wynik = Math.Pow((double)_operandLewy, _operandPrawy.Value).ToString();
                        break;
                    case "%":
                        _wynik = (_operandLewy * _operandPrawy / 100).ToString();
                        break;
                }

                _operacja = null;
                _operandLewy = null;
                _operandPrawy = null;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wynik)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dzialanie)));
            }
        }
    }
}

