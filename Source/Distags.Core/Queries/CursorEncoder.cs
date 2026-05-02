using System.Buffers.Text;
using System.Text.Json;

namespace Distags.Core.Queries;

public static class CursorEncoder
{
    public static string Encode<TCursor>(TCursor cursor)
    {
        byte[] cursorBytes = JsonSerializer.SerializeToUtf8Bytes(cursor);
        string encodedCursor = Base64Url.EncodeToString(cursorBytes);
        return encodedCursor;
    }

    public static TCursor? Decode<TCursor>(string? encodedCursor)
    {
        if (encodedCursor == null)
            return default;

        int cursorBytesLength = Base64Url.GetMaxDecodedLength(encodedCursor.Length);
        byte[] cursorBytes = new byte[cursorBytesLength];
        if (!Base64Url.TryDecodeFromChars(encodedCursor, cursorBytes, out _))
            return default;

        TCursor? cursor = JsonSerializer.Deserialize<TCursor>(cursorBytes);
        return cursor;
    }
}