using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using CrossNGram.MAUI.Services;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;

namespace CrossNGram.MAUI.ViewModels;

public abstract class BindableBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
        {
            return false;
        }

        storage = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    protected void Raise([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed class MainViewModel : BindableBase
{
    private const int MaxInputBytes = 32 * 1024;
    private const int MinN = 2;
    private const int MaxN = 5;
    private const int MinThreshold = 1;
    private const int MaxThreshold = 9;

    private string _inputText = string.Empty;
    private string _resultText = string.Empty;
    private int _n = MinN;
    private int _threshold = MinThreshold;
    private string _errorMessage = string.Empty;
    private bool _canSegment;

    private readonly Command _segmentCommand;
    private readonly Command _copyCommand;

    public MainViewModel()
    {
        _segmentCommand = new Command(Segment, () => CanSegment);
        _copyCommand = new Command(CopyResult, () => HasResult);
        UpdateCanSegment();
    }

    public string InputText
    {
        get => _inputText;
        set
        {
            if (SetProperty(ref _inputText, value))
            {
                if (string.IsNullOrEmpty(value))
                {
                    ResultText = string.Empty;
                }

                UpdateCanSegment();
            }
        }
    }

    public string ResultText
    {
        get => _resultText;
        private set
        {
            if (SetProperty(ref _resultText, value))
            {
                Raise(nameof(HasResult));
                _copyCommand.ChangeCanExecute();
            }
        }
    }

    public bool HasResult => !string.IsNullOrWhiteSpace(ResultText);

    public int N
    {
        get => _n;
        set
        {
            if (SetProperty(ref _n, value))
            {
                UpdateCanSegment();
            }
        }
    }

    public int Threshold
    {
        get => _threshold;
        set
        {
            if (SetProperty(ref _threshold, value))
            {
                UpdateCanSegment();
            }
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                Raise(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool CanSegment
    {
        get => _canSegment;
        private set
        {
            if (SetProperty(ref _canSegment, value))
            {
                _segmentCommand.ChangeCanExecute();
            }
        }
    }

    public ICommand SegmentCommand => _segmentCommand;

    public ICommand CopyCommand => _copyCommand;

    private void UpdateCanSegment()
    {
        var sanitized = InputText ?? string.Empty;
        var hasInput = !string.IsNullOrWhiteSpace(sanitized);
        var parametersValid = N is >= MinN and <= MaxN && Threshold is >= MinThreshold and <= MaxThreshold;
        var sizeValid = Encoding.UTF8.GetByteCount(sanitized) <= MaxInputBytes;

        if (!parametersValid)
        {
            ErrorMessage = $"n must be within [{MinN}, {MaxN}] and threshold within [{MinThreshold}, {MaxThreshold}].";
        }
        else if (!hasInput)
        {
            ErrorMessage = "Enter text before running the tokenizer.";
        }
        else if (!sizeValid)
        {
            ErrorMessage = "Input exceeds 32KB; please use the CLI for large files.";
        }
        else
        {
            ErrorMessage = string.Empty;
        }

        CanSegment = parametersValid && hasInput && sizeValid;
    }

    private void Segment()
    {
        if (!CanSegment)
        {
            return;
        }

        try
        {
            var tokens = SegFacade.Segment(InputText, N, Threshold);
            ResultText = string.Join(" ", tokens);
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Tokenization failed: {ex.Message}";
        }
    }

    private async void CopyResult()
    {
        if (!HasResult)
        {
            return;
        }

        try
        {
            await Clipboard.SetTextAsync(ResultText);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Copy failed: {ex.Message}";
        }
    }
}
