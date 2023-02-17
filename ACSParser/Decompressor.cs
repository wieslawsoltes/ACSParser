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

    public static byte[] Decompress(byte[] compressedData, int decompressedDataSize)
    {
        var results = new byte[decompressedDataSize];
        var bits = new BitArray(compressedData);
        var bitPosition = 0;
        var insertionPoint = 0;

        // Header 0x00
        bitPosition += 8;

        // Data Length = Length - Header + Footer
        var dataLength = bits.Length - 8 + 6 * 8; 

        var nextValueSizeBits = new []
        {
            6, 9, 12, 20
        };

        var valueToAdd = new Dictionary<int, int>
        {
            [6] = 1, [9] = 65, [12] = 577, [20] = 2673
        };

        while (bitPosition < dataLength)
        {
            var firstBitOfSequence = !bits[bitPosition];
            bitPosition += 1;

            if (firstBitOfSequence)
            {
                // Uncompressed Byte
                var data = GetByte(bits, bitPosition + 1);
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
                bitPosition += numOffsetSeqBits < 3 ? numOffsetSeqBits + 1 : numOffsetSeqBits;

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
                    bitPosition += nextValueSizeInBits;
                    // the end of the bit stream has been reached
                    break;
                }
                bitPosition += nextValueSizeInBits + 1;
                byteOffsetInResult += valueToAdd[nextValueSizeInBits];

                // Following the offset bits, count the number of sequential bits which have a value of 1. The maximum number of bits is 11.

                var numDecodedByteSeqBits = 0;
                var remainingBitsInLastSequence = 0;
                var sequenceCount = 0;
                for (; sequenceCount < 11; sequenceCount++)
                {
                    if (bits[bitPosition + sequenceCount])
                    {
                        numDecodedByteSeqBits++;
                        remainingBitsInLastSequence++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (sequenceCount == 10 && bits[bitPosition + 1])
                {
                    throw new Exception("Error invalid sequence");
                }

                for (var i = 0; i < remainingBitsInLastSequence; i++)
                {
                    if (bits[bitPosition + i])
                    {
                        numDecodedByteSeqBits++;
                    }
                }

                int value = 0;
                for (var i = 0; i < numDecodedByteSeqBits; i++) 
                {
                    value = (value << 1) | 1;
                }

                bytesDecodedInSequence += value;

                // Copy the BYTEs one at a time,
                // incrementing the insertion point after each BYTE,
                // as the copying may overlap past the original insertion point

                for (var i = 0; i < bytesDecodedInSequence; i++)
                {
                    var sourcePoint = insertionPoint - byteOffsetInResult;
                    if (sourcePoint > results.Length - 1 || sourcePoint < 0)
                    {
                        throw new Exception("Invalid offset.")
                    }
                    results[insertionPoint] = results[sourcePoint];
                    insertionPoint += 1;
                }
            }
        }

        // Footer 0xFF
        // bitPosition += 6 * 8;

        if (bitPosition != bits.Length)
        {
            throw new Exception("Invalid data length");
        }
        
        return results;
    }
}
