using Microsoft.AspNetCore.Components.Forms;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using System.ComponentModel.DataAnnotations;

namespace ColorGuessingApp.Pages;

public partial class Index
{
    public enum ColorGuessingResultStatus
    {
        Correct,
        Incorrect,
        Close,
        WrongFormat
    }
    private EditContext? EditContext { get; set; }
    private HexColor _inputColor = new HexColor();
    private readonly Random RNG = new Random();
    public string PreviousColorHex { get; private set; } = "#000000";
    public string CurrentColorHex { get; private set; } = "#000000";
    public ColorGuessingResultStatus Status { get; private set; } = ColorGuessingResultStatus.Correct;
    public int CorrectCount { get; private set; } = 0;
    public int CurrentColorInt { get; private set; } = 0;
    private void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.ClipRect(new SKRect(0,0,200,200));
        byte[] RGB = BitConverter.GetBytes(CurrentColorInt);
        canvas.Clear(new SKColor(RGB[0], RGB[1], RGB[2]));
    }

    protected override void OnInitialized()
    {
        EditContext = new(_inputColor);
        CurrentColorHex = GenerateRandomHexColor();
        base.OnInitialized();
    }
    public string GenerateRandomHexColor()
    {
        string nextColorHex;
        do
        {
            CurrentColorInt = RNG.Next(0x1000000); // Generate a random int between 0x0 and 0xffffff
            nextColorHex = string.Format("#{0:x6}", CurrentColorInt); // Convert the int to a hex string and return it
        } while (CurrentColorHex.ToLower().Equals(nextColorHex.ToLower()));
        return nextColorHex;
    }
    public string Color => _inputColor.Color;
    public void OnChanged(string color)
    {
        if (_inputColor.Color == color) return;
        _inputColor.Color = color;
        if (EditContext?.Validate() ?? false)
        {
            int distance = CalculateColorDistance(CurrentColorHex, _inputColor.Color);
            if (distance == 0)
            {
                CorrectCount++;
                Reset();
                Status = ColorGuessingResultStatus.Correct;
            }
            else if(distance <= 30)
            {
                CorrectCount++;
                Reset();
                Status = ColorGuessingResultStatus.Close;
            }
            else
            {
                Status = ColorGuessingResultStatus.Incorrect;
            }
        }
        else
        {
            Status = ColorGuessingResultStatus.WrongFormat;
        }
    }
    private static byte[] HexColorStringToBytes(string colorHex)
    {
        if (colorHex.StartsWith("#"))
        {
            colorHex = colorHex[1..];
        }
        return Enumerable.Range(0, colorHex.Length)
            .Where(x => x % 2 == 0)
            .Select(x => Convert.ToByte(colorHex.Substring(x, 2), 16))
            .ToArray();
    }

    private static int CalculateColorDistance(string colorHex1, string colorHex2)
    {
        byte[] RGB1 = HexColorStringToBytes(colorHex1);
        byte[] RGB2 = HexColorStringToBytes(colorHex2);
        int distance = 0;
        for (int i = 0; i < 3; i++)
        {
            distance += Math.Abs(RGB1[i] - RGB2[i]);
        }
        return distance;
    }

    public void Reset()
    {
        PreviousColorHex = CurrentColorHex;
        CurrentColorHex = GenerateRandomHexColor();
        _inputColor.Color = "#";
    }
}

public class HexColor
{
    [RegularExpression(@"^#(?:[0-9a-fA-F]{6})$",
         ErrorMessage = "Must be a color hex")]
    public string Color { get; set; } = "#000000";
}
   