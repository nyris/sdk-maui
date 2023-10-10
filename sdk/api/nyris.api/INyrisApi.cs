using JetBrains.Annotations;
using Nyris.Api.Api.Feedback;
using Nyris.Api.Api.ImageMatching;
using Nyris.Api.Api.ObjectProposal;
using Nyris.Api.Api.Recommendation;
using Nyris.Api.Api.TextSearch;

namespace Nyris.Api;

/// <summary>
///     Interface to main Nyris APIs.
/// </summary>
public interface INyrisApi
{
    /// <summary>
    ///     Gets or sets the API key.
    /// </summary>
    [CanBeNull]
    string ApiKey { get; set; }

    /// <summary>
    ///     Gets the Image Matching API.
    /// </summary>
    [NotNull]
    IImageMatchingApi ImageMatching { get; }

    /// <summary>
    ///     Gets the Object Proposal API.
    /// </summary>
    [NotNull]
    IObjectProposalApi ObjectProposal { get; }

    /// <summary>
    ///     Gets the Text Search API.
    /// </summary>
    [NotNull]
    ITextSearchApi TextSearch { get; }

    /// <summary>
    ///     Gets the Recommendations API.
    /// </summary>
    [NotNull]
    IRecommendationApi Recommendations { get; }

    /// <summary>
    ///     Gets the Manual Matching API.
    /// </summary>
    [NotNull]
    IFeedbackApi Feedback { get; }
}