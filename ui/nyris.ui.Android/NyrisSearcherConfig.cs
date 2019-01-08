using System;
using Nyris.Sdk.Network.API.XOptions;
using Nyris.Sdk.Network.Model;

namespace Nyris.Ui.Android
{
    internal class NyrisSearcherConfig
    {
        public bool IsDebug { get; set; }

        public string ApiKey { get; set; }

        public string OutputFormat { get; set; }

        public string Language { get; set; }

        public uint Limit { get; set; }

        public Type ResponseType { set; get; } = typeof(OfferResponse);

        public ExactOptions ExactOptions { get; set; }

        public SimilarityOptions SimilarityOptions { get; set; }

        public OcrOptions OcrOptions { get; set; }

        public RegroupOptions RegroupOptions { get; set; }

        public RecommendationOptions RecommendationOptions { get; set; }

        public CategoryPredictionOptions CategoryPredictionOptions { get; set; }
    }
}
