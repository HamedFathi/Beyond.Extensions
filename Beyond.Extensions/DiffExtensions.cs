// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace

using Beyond.Extensions.Enums;
using Beyond.Extensions.Types;

namespace Beyond.Extensions.ArrayExtended;

public static class DiffExtensions
{
    // Method to get the diff results between two sets of data.
    public static IEnumerable<DiffResult> GetDiffResult(this string[] oldData, string[] newData)
    {
        // Calculate the list of changes between the old and new data.
        var changes = CalculateDiff(oldData, newData).ToList();

        // Prepare the list to store the final diff results.
        var diffResults = new List<DiffResult>();

        // Process each change in the list and convert it to the appropriate DiffResult.
        for (int i = 0; i < changes.Count;)
        {
            var change = changes[i];

            if (change.Type == DiffChangeType.Unchanged)
            {
                // For unchanged text, add it to the diff result as is.
                diffResults.Add(new DiffResult
                {
                    OldText = change.Text,
                    NewText = change.Text,
                    Status = DiffChangeType.Unchanged
                });
                i++;
            }
            else if (change.Type == DiffChangeType.Inserted)
            {
                // For inserted text, set the OldText to null and add the new text.
                diffResults.Add(new DiffResult
                {
                    OldText = null,
                    NewText = change.Text,
                    Status = DiffChangeType.Inserted
                });
                i++;
            }
            else if (change.Type == DiffChangeType.Deleted)
            {
                // For deleted text, check if it was modified (deleted and inserted together).
                if (i < changes.Count - 1 && changes[i + 1].Type == DiffChangeType.Inserted)
                {
                    // If modified, add both the old and new text with the 'Modified' status.
                    diffResults.Add(new DiffResult
                    {
                        OldText = change.Text,
                        NewText = changes[i + 1].Text,
                        Status = DiffChangeType.Modified // or you could define your own 'Modified' status
                    });
                    i += 2; // Skip the next change because it has been handled here.
                }
                else
                {
                    // If not modified, add the old text as deleted.
                    diffResults.Add(new DiffResult
                    {
                        OldText = change.Text,
                        NewText = null,
                        Status = DiffChangeType.Deleted
                    });
                    i++;
                }
            }
        }

        return diffResults;
    }

    // Method to calculate the list of changes between the old and new data.
    public static IEnumerable<DiffChange> CalculateDiff(this string[] oldData, string[] newData)
    {
        // Create a dictionary to store edit paths with their endpoints.
        var editEditPath = new Dictionary<int, DiffEditPath> { { 1, new DiffEditPath(0, new List<DiffChange>()) } };

        var oldDataCount = oldData.Length;
        var newDataCount = newData.Length;

        // Loop over the distance between old and new data (Levenshtein distance).
        for (var distance = 0; distance <= oldDataCount + newDataCount; distance++)
        {
            // Loop over possible values of k.
            for (var k = -distance; k <= distance; k += 2)
            {
                // Determine if we go down or right in the edit graph.
                var goDown = k == -distance || k != distance && editEditPath[k - 1].Endpoint < editEditPath[k + 1].Endpoint;

                DiffEditPath previousEditPath;
                int xIndex;
                if (goDown)
                {
                    previousEditPath = editEditPath[k + 1];
                    xIndex = previousEditPath.Endpoint;
                }
                else
                {
                    previousEditPath = editEditPath[k - 1];
                    xIndex = previousEditPath.Endpoint + 1;
                }

                // Create a new edit history by copying the previous one and adding the current change.
                var currentHistory = new List<DiffChange>(previousEditPath.EditHistory);
                var yIndex = xIndex - k;

                // Check if there is an inserted text to be added to the history.
                if (yIndex > 0 && yIndex <= newDataCount && goDown)
                {
                    currentHistory.Add(new DiffChange(DiffChangeType.Inserted, newData[yIndex - 1], yIndex, DiffChangeOrigin.New));
                }
                // Check if there is a deleted text to be added to the history.
                else if (xIndex > 0 && xIndex <= oldDataCount)
                {
                    currentHistory.Add(new DiffChange(DiffChangeType.Deleted, oldData[xIndex - 1], xIndex, DiffChangeOrigin.Old));
                }

                // Traverse through the unchanged portion of the data.
                while (xIndex < oldDataCount && yIndex < newDataCount && oldData[xIndex] == newData[yIndex])
                {
                    currentHistory.Add(new DiffChange(DiffChangeType.Unchanged, oldData[xIndex], xIndex + 1, DiffChangeOrigin.Old));
                    xIndex++;
                    yIndex++;
                }

                // Check if we have reached the endpoint, i.e., all changes have been found.
                if (xIndex >= oldDataCount && yIndex >= newDataCount)
                {
                    return currentHistory;
                }

                // Save the current edit history for the given endpoint k.
                editEditPath[k] = new DiffEditPath(xIndex, currentHistory);
            }
        }

        // If the endpoint is not reached, throw an exception indicating that the edit script could not be found.
        throw new Exception("Could not find edit script");
    }
}