﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using wallabag.Api.Models;

namespace wallabag.Api
{
    public partial class WallabagClient
    {
        /// <summary>
        /// Retrieve all annotations for a specific item.
        /// </summary>
        /// <param name="item">The item for which the annotations should be retrieved.</param>
        /// <returns>If successful, a list of <see cref="WallabagAnnotation"/> items, otherwise null.</returns>
        public Task<IEnumerable<WallabagAnnotation>> GetAnnotationsAsync(WallabagItem item, CancellationToken cancellationToken = default(CancellationToken))
            => GetAnnotationsAsync(item.Id, cancellationToken);

        /// <summary>
        /// Retrieve all annotations for a specific item.
        /// </summary>
        /// <param name="itemId">The ID of the item for which the annotations should be retrieved.</param>
        /// <returns>If successful, a list of <see cref="WallabagAnnotation"/> items, otherwise null.</returns>
        public async Task<IEnumerable<WallabagAnnotation>> GetAnnotationsAsync(int itemId, CancellationToken cancellationToken = default(CancellationToken))
            => (await ExecuteHttpRequestAsync<WallabagAnnotationRoot>(HttpRequestMethod.Get, BuildApiRequestUri($"/annotations/{itemId}"), cancellationToken))?.Annotations;

        /// <summary>
        /// Adds a <see cref="WallabagAnnotation"/> to a specific item.
        /// </summary>
        /// <param name="item">The item for which the annotation should be added.</param>
        /// <param name="annotation">The new annotation for the item.</param>        
        /// <returns>If successful, the new <see cref="WallabagAnnotation"/>, otherwise null.</returns>
        public Task<WallabagAnnotation> AddAnnotationAsync(WallabagItem item, WallabagAnnotation annotation, CancellationToken cancellationToken = default(CancellationToken))
            => AddAnnotationAsync(item.Id, annotation, cancellationToken);

        /// <summary>
        /// Adds a <see cref="WallabagAnnotation"/> to a specific item.
        /// </summary>
        /// <param name="itemId">The ID of the item for which the annotation should be added.</param>
        /// <param name="annotation">The new annotation for the item.</param>
        /// <returns>If successful, the new <see cref="WallabagAnnotation"/>, otherwise null.</returns>
        public async Task<WallabagAnnotation> AddAnnotationAsync(int itemId, WallabagAnnotation annotation, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckValidityOfAnnotation(annotation);

            var parameters = new Dictionary<string, object>
            {
                { "ranges", annotation.Ranges },
                { "text", annotation.Text },
                { "quote", annotation.Quote }
            };

            var result = await ExecuteHttpRequestAsync<WallabagAnnotation>(HttpRequestMethod.Post, BuildApiRequestUri($"/annotations/{itemId}"), cancellationToken, parameters);

            return result;
        }

        /// <summary>
        /// Updates an existing annotation with new properties.
        /// </summary>
        /// <param name="oldAnnotation">The old annotation that should be updated.</param>
        /// <param name="newAnnotation">The new annotation that should replace the old annotation.</param>
        /// <returns>If successful, the returned <see cref="WallabagAnnotation"/>, otherwise null.</returns>
        public Task<WallabagAnnotation> UpdateAnnotationAsync(WallabagAnnotation oldAnnotation, WallabagAnnotation newAnnotation, CancellationToken cancellationToken = default(CancellationToken))
            => UpdateAnnotationAsync(oldAnnotation.Id, newAnnotation, cancellationToken);

        /// <summary>
        /// Updates an existing annotation with new properties.
        /// </summary>
        /// <param name="oldAnnotationId">The ID of the old annotation that should be updated.</param>
        /// <param name="newAnnotation">The new annotation that should replace the old annotation.</param>
        /// <returns>If successful, the returned <see cref="WallabagAnnotation"/>, otherwise null.</returns>
        public async Task<WallabagAnnotation> UpdateAnnotationAsync(int oldAnnotationId, WallabagAnnotation newAnnotation, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckValidityOfAnnotation(newAnnotation);

            var parameters = new Dictionary<string, object>
            {
                { "ranges", newAnnotation.Ranges },
                { "text", newAnnotation.Text }
            };

            if (!string.IsNullOrEmpty(newAnnotation.Quote))
                parameters.Add("quote", newAnnotation.Quote);

            var result = await ExecuteHttpRequestAsync<WallabagAnnotation>(HttpRequestMethod.Put, BuildApiRequestUri($"/annotations/{oldAnnotationId}"), cancellationToken, parameters);

            return result;
        }

        /// <summary>
        /// Deletes an annotation.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public Task<bool> DeleteAnnotationAsync(WallabagAnnotation annotation, CancellationToken cancellationToken = default(CancellationToken))
            => DeleteAnnotationAsync(annotation.Id, cancellationToken);

        /// <summary>
        /// Deletes an annotation.
        /// </summary>
        /// <param name="annotationId">The ID of the annotation.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public async Task<bool> DeleteAnnotationAsync(int annotationId, CancellationToken cancellationToken = default(CancellationToken))
            => (await ExecuteHttpRequestAsync<object>(HttpRequestMethod.Delete, BuildApiRequestUri($"/annotations/{annotationId}"), cancellationToken)) != null;

        private void CheckValidityOfAnnotation(WallabagAnnotation annotation)
        {
            var regex = new Regex(@"^/[a-z]{1,}\[[0-9]{1,}\]");

            if (annotation == null)
                throw new ArgumentNullException(nameof(annotation));

            if (annotation.Ranges.Count == 0)
                throw new ArgumentOutOfRangeException("annotation.Ranges.Count");

            foreach (var range in annotation.Ranges)
            {
                if (string.IsNullOrWhiteSpace(range.Start) ||
                    regex.Match(range.Start).Success == false)
                    throw new FormatException("The start of the range doesn't equal the required format: /<tag-name>[offset]");

                if (string.IsNullOrWhiteSpace(range.End) ||
                    regex.Match(range.End).Success == false)
                    throw new FormatException("The end of the range doesn't equal the required format: /<tag-name>[offset]");
            }
        }
    }
}
