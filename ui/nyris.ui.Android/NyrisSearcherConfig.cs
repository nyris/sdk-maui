using System;
using Nyris.Api.Api;
using Nyris.Api.Api.RequestOptions;
using Nyris.Api.Model;
using Nyris.Api.Utils;

namespace Nyris.Ui.Android
{
    internal class NyrisSearcherConfig
    {
        public bool IsDebug { get; set; }

        public string ApiKey { get; set; }

        public string OutputFormat { get; set; } = Constants.DefaultResultFormat;

        public string Language { get; set; } = Constants.DefaultLanguage;

        public int Limit { get; set; } = OptionDefaults.DefaultLimit;

        public Type ResponseType { set; get; } = typeof(OfferResponseDto);

        public ExactOptions ExactOptions { get; set; }

        public SimilarityOptions SimilarityOptions { get; set; }

        public OcrOptions OcrOptions { get; set; }

        public RegroupOptions RegroupOptions { get; set; }

        public RecommendationModeOptions RecommendationModeOptions { get; set; }

        public CategoryPredictionOptions CategoryPredictionOptions { get; set; }
    }
}
