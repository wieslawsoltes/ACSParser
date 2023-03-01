using System;
using System.Collections;
using System.Diagnostics;

namespace ACSParser;

public static class Decompressor
{
    private static byte GetByte(BitArray src, int startBitPosition)
    {
        var dst = new BitArray(8);
        for (var i = 0; i < 8; i++)
        {
            dst[i] = src[startBitPosition + i];
        }
        var bytes = new byte[1];
        dst.CopyTo(bytes, 0);
        return bytes[0];
    }

    private static Int32 GetInt32(BitArray src, int startBitPosition, int bitsCount)
    {
        var dst = new BitArray(32);
        for (var i = 0; i < bitsCount; i++)
        {
            dst[i] = src[startBitPosition + i];
        }
        var bytes = new byte[4];
        dst.CopyTo(bytes, 0);
        return BitConverter.ToInt32(bytes, 0);
    }

    private static int GetValueFromBitCount(int bitCount)
    {
        var value = 0;
        for (var i = 0; i < bitCount; i++) 
        {
            value = (value << 1) | 1;
        }
        return value;
    }

    public static byte[] Decompress(byte[] compressedData, int decompressedDataSize)
    {
        var results = new byte[decompressedDataSize];
        var bits = new BitArray(compressedData);
        var bitPosition = 0;
        var insertionPoint = 0;

//Debug(bits, 0, 7, "Header");

        // Header 0x00
        bitPosition += 8;

        var nextValueSizeBits = new []
        {
            6, 9, 12, 20
        };

        var valueToAdd = new Dictionary<int, int>
        {
            [6] = 1, [9] = 65, [12] = 577, [20] = 2673
        };

        while (bitPosition < bits.Length)
        {
#if DEBUG
            var startPosition = bitPosition;
#endif
            var firstBitOfSequence = !bits[bitPosition];
            bitPosition += 1;

            if (firstBitOfSequence)
            {
                // Uncompressed Byte
                var data = GetByte(bits, bitPosition);
                results[insertionPoint] = data;
                bitPosition += 8;
                insertionPoint += 1;
            }
            else
            {
                // Number of bits that make up the next value in the sequence

                var numOffsetSeqBits = 0;
                for (var i = 0; i < 3; i++)
                {
                    if (bits[bitPosition + i])
                    {
                        numOffsetSeqBits++;
                    }
                    else
                    {
                        break;
                    }
                }
                bitPosition += numOffsetSeqBits == 3 ? numOffsetSeqBits : numOffsetSeqBits + 1;

                var nextValueSizeInBits = nextValueSizeBits[numOffsetSeqBits];

                // Number of BYTEs that will be decoded from the sequence

                var bytesDecodedInSequence = 2;
                
                // Value is an offset,
                // in BYTEs, within the destination buffer,
                // subtracted from the buffer's current insertion point.

                var byteOffsetInResult = GetInt32(bits, bitPosition, nextValueSizeInBits);

                // If the bit count is 20:
                if (nextValueSizeInBits == 20 && byteOffsetInResult == 0x000FFFFF)
                {
                    // The end of the bit stream has been reached
                    bitPosition += nextValueSizeInBits;
                    break;
                }

                bitPosition += nextValueSizeInBits;
                byteOffsetInResult += valueToAdd[nextValueSizeInBits];

                // Following the offset bits, count the number of sequential bits which have a value of 1.
                // The maximum number of bits is 11.

                var numDecodedByteSeqBits1 = 0;
                var remainingBitsInLastSequence = 0;
                var sequenceCount = 0;
                for (; sequenceCount < 11; sequenceCount++)
                {
                    if (bits[bitPosition])
                    {
                        numDecodedByteSeqBits1++;
                        remainingBitsInLastSequence++;
                        bitPosition += 1;

                        if (sequenceCount == 10 && bits[bitPosition])
                        {
#if DEBUG
                            var endPosition = bitPosition;
                            Console.WriteLine(GetBitstreamString(bits, startPosition, endPosition));
#endif
                            throw new Exception($"Invalid sequence sequenceCount={sequenceCount}, bitPosition={bitPosition}, remainingBitsInLastSequence={remainingBitsInLastSequence}");
                        }
                    }
                    else
                    {
                        bitPosition += 1;
                        break;
                    }
                }

                bytesDecodedInSequence += GetValueFromBitCount(numDecodedByteSeqBits1);

                // TODO:
                if (numDecodedByteSeqBits1 == 11)
                {
                    remainingBitsInLastSequence = 0;
                }

                if (remainingBitsInLastSequence > 0)
                {
                    bytesDecodedInSequence += GetInt32(bits, bitPosition, remainingBitsInLastSequence);
                    bitPosition += remainingBitsInLastSequence;
                }

                // Copy the BYTEs one at a time,
                // incrementing the insertion point after each BYTE,
                // as the copying may overlap past the original insertion point

                for (var i = 0; i < bytesDecodedInSequence; i++)
                {
                    var sourcePoint = insertionPoint - byteOffsetInResult;
                    if (sourcePoint > results.Length - 1 || sourcePoint < 0 || insertionPoint > results.Length - 1)
                    {
#if DEBUG
                        var endPosition = bitPosition - 1;
                        Console.WriteLine(GetBitstreamString(bits, startPosition, endPosition));
#endif
                        throw new Exception($"Invalid offset insertionPoint={insertionPoint}, byteOffsetInResult={byteOffsetInResult}, numDecodedByteSeqBits1={numDecodedByteSeqBits1}, remainingBitsInLastSequence={remainingBitsInLastSequence}.");
                    }
                    results[insertionPoint] = results[sourcePoint];
                    insertionPoint += 1;
                }
            }
        }

        return results;
    }

    private static string GetBitstreamString(BitArray bits, int startPosition, int endPosition)
    {
        var bitString = "";

        for (var i = startPosition; i <= endPosition; i++)
        {
            bitString += bits[i] ? "1" : "0";
        }

        return $"{bitString} [{startPosition}..{endPosition}]";
    }
}
