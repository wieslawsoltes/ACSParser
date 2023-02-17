using System;
using System.Collections;

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

Debug(bits, 0, 7, "Header");

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
var startPosition = bitPosition;

            var firstBitOfSequence = !bits[bitPosition];
            bitPosition += 1;

            if (firstBitOfSequence)
            {
                Debug(bits, startPosition, bitPosition + 7, "Uncompressed");

                // Uncompressed Byte
                var data = GetByte(bits, bitPosition);
                results[insertionPoint] = data;
                // startBit + data = 9 bits
                bitPosition += 8;
                // data = 1 byte
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
Debug(bits, bitPosition, bitPosition + nextValueSizeInBits, "End of Stream");
                    bitPosition += nextValueSizeInBits;
                    // the end of the bit stream has been reached
                    break;
                }
//bitPosition += nextValueSizeInBits + 1;
bitPosition += nextValueSizeInBits;
//System.Diagnostics.Debug.WriteLine($"nextValueSizeInBits={nextValueSizeInBits}");
                byteOffsetInResult += valueToAdd[nextValueSizeInBits];

                // Following the offset bits, count the number of sequential bits which have a value of 1. The maximum number of bits is 11.

                                    


                var numDecodedByteSeqBits1 = 0;
                var remainingBitsInLastSequence = 0;
                var sequenceCount = 0;
                for (; sequenceCount < 11; sequenceCount++)
                {
//System.Diagnostics.Debug.WriteLine($"{bitPosition }={(bits[bitPosition] ? '1' : '0')}");
                    if (bits[bitPosition])
                    {
                        numDecodedByteSeqBits1++;
                        remainingBitsInLastSequence++;
                        bitPosition += 1;
                    }
                    else
                    {
                        bitPosition += 1;
                        break;
                    }
                }

                bytesDecodedInSequence += GetValueFromBitCount(numDecodedByteSeqBits1);

                if (sequenceCount == 10 && bits[bitPosition])
                {
                    throw new Exception("Error invalid sequence");
                }

                if (remainingBitsInLastSequence > 0)
                {
                    var numDecodedByteSeqBits2 = 0;
                    for (var i = 0; i < remainingBitsInLastSequence; i++)
                    {
                        if (bits[bitPosition])
                        {
                            numDecodedByteSeqBits2++;
                        }
                        bitPosition += 1;
                    }

                    bytesDecodedInSequence += GetValueFromBitCount(numDecodedByteSeqBits2);
                }


                


                // Copy the BYTEs one at a time,
                // incrementing the insertion point after each BYTE,
                // as the copying may overlap past the original insertion point

var endPosition = bitPosition - 1;
Debug(bits, startPosition, endPosition, $"Compressed, Offset={byteOffsetInResult}, Bytes={bytesDecodedInSequence}, remainingBitsInLastSequence={remainingBitsInLastSequence}");
/*
00000010000000000010000000001000
00001011000010010000000101000010
10110111000110011000000011101101
11111111111111111111111
 
00000000 [0..7] Header
000000100 [8..16] Uncompressed
000000000 [17..25] Uncompressed
100000000 [26..34] Compressed
010000000 [35..43] Uncompressed
10110000100 [44..54] Compressed
10000000101 [55..65] Compressed
000010101 [66..74] Uncompressed
1011100011001 [75..87] Compressed
100000001110110 [88..102] Compressed
111111111111111111111111 [107..127] End of Stream
 */
                
                for (var i = 0; i < bytesDecodedInSequence; i++)
                {
                    var sourcePoint = insertionPoint - byteOffsetInResult;
                    if (sourcePoint > results.Length - 1 || sourcePoint < 0)
                    {
                        throw new Exception("Invalid offset.");
                    }
                    results[insertionPoint] = results[sourcePoint];
                    insertionPoint += 1;
                }
            }
        }

        return results;
    }

    private static void Debug(BitArray bits, int startPosition, int endPosition, string message)
    {
        var bitString = "";

        for (var i = startPosition; i <= endPosition; i++)
        {
            bitString += bits[i] ? "1" : "0";
        }

        System.Diagnostics.Debug.WriteLine($"{bitString} [{startPosition}..{endPosition}] {message}");
    }
}
