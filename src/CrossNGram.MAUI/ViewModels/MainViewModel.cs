using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using CrossNGram.MAUI.Services;

namespace CrossNGram.MAUI.ViewModels;

public abstract class BindableBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? name=null)
    {
        if (Equals(storage, value)) return false;
        storage = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        return true;
    }
    protected void Raise([CallerMemberName] string? name=null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public sealed class MainViewModel : BindableBase
{
    private string _inputText = string.Empty;
    private string _resultText = string.Empty;
    private int _n = 2;
    private int _threshold = 1;
    private string _error = string.Empty;
    private readonly ICommand _segmentCommand;
    private readonly ICommand _copyCommand;

    public MainViewModel()
    {
        _segmentCommand = new Command(Segment);
        _copyCommand = new Command(CopyResult);
        UpdateCanSegment();
    }

    public string InputText
    {
        get => _inputText;
        set { if (SetProperty(ref _inputText, value)) UpdateCanSegment(); }
    }

    public string ResultText
    {
        get => _resultText;
        private set => SetProperty(ref _resultText, value);
    }

    public int N
    {
        get => _n;
        set { if (SetProperty(ref _n, value)) UpdateCanSegment(); }
    }

    public int Threshold
    {
        get => _threshold;
        set { if (SetProperty(ref _threshold, value)) UpdateCanSegment(); }
    }

    public string ErrorMessage
    {
        get => _error;
        private set { if (SetProperty(ref _error, value)) Raise(nameof(HasError)); }
    }
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    private bool _canSegment;
    public bool CanSegment
    {
        get => _canSegment;
        private set { SetProperty(ref _canSegment, value); }
    }

    public ICommand SegmentCommand => _segmentCommand;
    public ICommand CopyCommand => _copyCommand;

    private void UpdateCanSegment()
    {
        ErrorMessage = string.Empty;
        var validParams = (N >= 2 && N <= 5) && (Threshold >= 1 && Threshold <= 9);
        if (!validParams)
            ErrorMessage = "\u53c2\u6570\u975e\u6cd5: n in [2,5], threshold in [1,9].";
        var hasInput = !string.IsNullOrWhiteSpace(InputText);
        var sizeOk = Encoding.UTF8.GetByteCount(InputText ?? string.Empty) <= 32768;
        if (!sizeOk && string.IsNullOrEmpty(ErrorMessage))
            ErrorMessage = "\u6587\u672c\u8fc7\u5927 (> 32KB), \u8bf7\u6539\u7528 CLI \u7248\u672c\u5904\u7406.";
        CanSegment = validParams && hasInput && sizeOk;
    }

    private void Segment()
    {
        try
        {
            // Guard
            UpdateCanSegment();
            if (!CanSegment) return;

            var tokens = SegFacade.Segment(InputText, N, Threshold);
            ResultText = string.Join(" ", tokens);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"\u5904\u7406\u5931\u8D25: {ex.Message}";
        }
    }

    private async void CopyResult()
    {
        try
        {
            if (!string.IsNullOrEmpty(ResultText))
                await Clipboard.SetTextAsync(ResultText);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"\u590D\u5236\u5931\u8D25: {ex.Message}";
        }
    }
}
