namespace Nyris.UI.Common.Models;

public record Links(string? Main, string? Mobile) { 
    public override string ToString()
    {
        return $"Main: {Main}, \n" +
               $"Mobile: {Mobile}";
    }
}