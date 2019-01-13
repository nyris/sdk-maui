using System;
using Nyris.Sdk.Network.API;
using Nyris.Sdk.Network.API.XOptions;
using Nyris.Sdk.Network.Model;
using Nyris.Sdk.Utils;

namespace Nyris.Ui.Android
{
    internal class NyrisSearcherConfig
    {
        public bool IsDebug { get; set; }

        public string ApiKey { get; set; }

        public string OutputFormat { get; set; } = Constants.DEFAULT_OUTPUT_FORMAT;

        public string Language { get; set; } = Constants.DEFAULT_LANGUAGE;

        public uint Limit { get; set; } = Options.DEFAULT_LIMIT;

        public Type ResponseType { set; get; } = typeof(OfferResponseDto);

        public ExactOptions ExactOptions { get; set; }

        public SimilarityOptions SimilarityOptions { get; set; }

        public OcrOptions OcrOptions { get; set; }

        public RegroupOptions RegroupOptions { get; set; }

        public RecommendationOptions RecommendationOptions { get; set; }

        public CategoryPredictionOptions CategoryPredictionOptions { get; set; }
    }
}
