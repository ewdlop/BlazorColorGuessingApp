using Microsoft.AspNetCore.Components.Forms;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using System.ComponentModel.DataAnnotations;

namespace ColorGuessingApp.Pages;

public partial class Index
{
    private EditContext? EditContext { get; set; }
    private HexColor InputColor = new HexColor();
    private string CurrentColorHex { get; set; } = "#000000";
    private int CorrectCount = 0;
    private int CurrentColorInt = 0;
    private bool IsCorrect = true;
    private bool IsCorrectFormat = true;
    private readonly Random RNG = new Random();
    private void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.ClipRect(new SKRect(0,0,200,200));
        byte[] RGB = BitConverter.GetBytes(CurrentColorInt);
        canvas.Clear(new SKColor(RGB[0], RGB[1], RGB[2]));

    }

    protected override void OnInitialized()
    {
        EditContext = new(InputColor);
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
    public string Color => InputColor.Color;
    public void OnChanged(string color)
    {
        if (InputColor.Color == color) return;
        InputColor.Color = color;
        if (EditContext?.Validate() ?? false)
        {
            IsCorrectFormat = true;
            if (CurrentColorHex.ToLower() == InputColor.Color.ToLower())
            {
                CorrectCount++;
                CurrentColorHex = GenerateRandomHexColor();
                InputColor.Color = "#";
                IsCorrect = true;
            }
            else
            {
                IsCorrect = false;
            }
        }
        else
        {
            IsCorrectFormat = false;
        }
    }
}
public class HexColor
{
    [RegularExpression(@"^#(?:[0-9a-fA-F]{6})$",
         ErrorMessage = "Must be a color hex")]
    public string Color { get; set; } = "#000000";
}
    