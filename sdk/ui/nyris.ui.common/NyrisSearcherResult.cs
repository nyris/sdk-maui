using Nyris.UI.Common.Models;

namespace Nyris.UI.Common;

public record NyrisSearcherResult(string RequestCode, List<Offer> Offers, Dictionary<string, float>? PredictedCategories, byte[]? Screenshot);