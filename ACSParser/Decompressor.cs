using System.Collections;

namespace ACSParser;

public static class Decompressor
{
    private static byte GetByte(BitArray src, int position)
    {
        var dst = new BitArray(8);
        for (var i = 0; i < 8; i++)
        {
            dst[i] = src[position + i];
        }
        var bytes = new byte[1];
        dst.CopyTo(bytes, 0);
        return bytes[0];
    }

    private static int GetValue(BitArray src, int position, int bits)
    {
        var dst = new BitArray(32);
        for (var i = 0; i < bits; i++)
        {
            dst[i] = src[position + i];
        }
        var bytes = new byte[4];
        dst.CopyTo(bytes, 0);
        return Convert.ToInt32(bytes);
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
            if (!bits[bitPosition])
            {
                var data = GetByte(bits, bitPosition + 1);
                // Uncompressed Byte
                results[insertionPoint] = data;
                bitPosition += 9;
                insertionPoint += 1;
            }
            else
            {
                var numOffsetSeqBits = 0;
                for (var i = 0; i < 3; i++)
                {
                    if (bits[bitPosition + i])
                    {
                        numOffsetSeqBits++;
                    }
                }
                bitPosition += numOffsetSeqBits < 3 ? numOffsetSeqBits + 1 : numOffsetSeqBits;

                var nextValueSizeInBits = nextValueSizeBits[numOffsetSeqBits];
                var byteOffsetInResult = GetValue(bits, bitPosition, nextValueSizeInBits);
                bitPosition += nextValueSizeInBits + 1;
                byteOffsetInResult += valueToAdd[nextValueSizeInBits];

                // TODO: handle nextValueSizeInBits == 20
                
                
                
                
                
                var decodedByteCount = 2;
                
                var numDecodedByteSeqBits = 0;
                for (var i = 0; i < 3; i++)
                {
                    if (bits[bitPosition + i])
                    {
                        numDecodedByteSeqBits++;
                    }
                }
                bitPosition += numDecodedByteSeqBits < 3 ? numDecodedByteSeqBits + 1 : numDecodedByteSeqBits;
                
                
                

                
                /*
                var numericValue = GetByte(bits, bitPosition);

                valueBitCount += 
                */

                
                
                
                bitPosition += valueBitCount;
                
                



                // Compressed Bytes
                bitPosition += 1;

                // TODO:
            }
        }

        // Footer 0xFF
        bitPosition += 6 * 8;

        if (bitPosition != bits.Length)
        {
            throw new Exception("Invalid data length");
        }
        
        return results;
    }
}
