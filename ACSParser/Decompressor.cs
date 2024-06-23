namespace ACSParser;

public static unsafe class Decompressor
{
    public static byte[] Decompress(byte[] compressed, int expectedDecompressedSize)
    {
        var compressedSize = (uint)compressed.Length;

        // Allocate memory for decompressed data
        var decompressed = new byte[expectedDecompressedSize];

        // Temporary buffer for decompression (size based on the function's behavior)
        var tempBuffer = new byte[compressedSize + expectedDecompressedSize];

        // Copy compressed data to temp_buffer (the function seems to work backwards)
        Array.Copy(compressed, tempBuffer, compressedSize);

        // Variable to store the actual decompressed size
        long actualDecompressedSize = 0;

        // Call the decompression function

        fixed (byte* tempBufferPtr = tempBuffer)
        fixed (byte* decompressedPtr = decompressed)
        {
            var result = DecompressCore(
                (long)tempBufferPtr,
                compressedSize,
                (long)decompressedPtr,
                (long)(decompressedPtr + expectedDecompressedSize),
                out actualDecompressedSize);

            if (result == 1)
            {
                // Console.WriteLine("Decompression successful");
                // Console.WriteLine($"Actual decompressed size: {actualDecompressedSize}");
                //
                // // Print the decompressed data
                // for (int i = 0; i < actualDecompressedSize; i++)
                // {
                //     Console.Write($"0x{decompressed[i]:X2},");
                // }
                // Console.WriteLine();
            }
            else
            {
                throw new InvalidOperationException("Decompression failed");
            }
        }

        return decompressed;
    }

    private static long DecompressCore(long bufferPtr, long compressedDataLen, long decompStartPtr, long decompEndPtr,
        out long actualDecompressedSize)
    {
        actualDecompressedSize = 0;

        if (compressedDataLen <= 7)
        {
            return 0;
        }

        var endBufferPtr = compressedDataLen + bufferPtr;
        var endPaddingCounter = 0;
        endBufferPtr--;
        long result = 0;

        while (*(byte*)endBufferPtr == 0xFF)
        {
            endPaddingCounter++;
            if (endPaddingCounter >= 6)
            {
                result = 0;
                if (*(byte*)bufferPtr != 0)
                {
                    return result;
                }

                var decompBufferLimit = (ulong)(decompEndPtr + decompStartPtr);
                var currentWritePtr = decompStartPtr;
                var currentReadPtr = bufferPtr + 5;
                long bitOffset = 0;
                return ProcessControlBits(currentReadPtr, currentWritePtr, bitOffset, decompBufferLimit,
                    decompStartPtr, ref actualDecompressedSize);
            }

            endBufferPtr--;
            result = 0;
        }

        return result;
    }

    private static long ProcessControlBits(long currentReadPtr, long currentWritePtr, long bitOffset,
        ulong decompBufferLimit, long decompStartPtr, ref long actualDecompressedSize)
    {
        var bitShift = (ulong)bitOffset;
        var dataReadPtr = currentReadPtr;
        var dataWritePtr = currentWritePtr;
        var controlBits = *(ulong*)(dataReadPtr - 4);

        if ((controlBits & (1UL << (int)bitShift)) != 0)
        {
            if ((controlBits & (2UL << (int)bitShift)) != 0)
            {
                if ((controlBits & (4UL << (int)bitShift)) != 0)
                {
                    var maskedValue = controlBits >> (int)(bitShift + 4);
                    if ((controlBits & (8UL << (int)bitShift)) != 0)
                    {
                        var offsetValue = (long)(maskedValue & 0xfffff);
                        if (offsetValue == 0xfffff)
                        {
                            actualDecompressedSize = dataWritePtr - decompStartPtr;
                            return 1;
                        }

                        long incrementSize = 2;
                        var shiftedBitPosition = (long)bitShift | 24;
                        var lengthToAdd = offsetValue + 0x1241;
                        return DecodeNextBlock(dataReadPtr, dataWritePtr, shiftedBitPosition, lengthToAdd,
                            incrementSize, decompBufferLimit, decompStartPtr, ref actualDecompressedSize);
                    }

                    {
                        long incrementSize = 1;
                        var shiftedBitPosition = (long)bitShift | 16;
                        var lengthToAdd = ((long)maskedValue & 4095) + 577;
                        return DecodeNextBlock(dataReadPtr, dataWritePtr, shiftedBitPosition, lengthToAdd,
                            incrementSize, decompBufferLimit, decompStartPtr, ref actualDecompressedSize);
                    }
                }

                {
                    long incrementSize = 1;
                    var shiftedBitPosition = (long)bitShift + 12;
                    var lengthToAdd = (long)((controlBits >> (int)(bitShift + 3) & 511) + 65);
                    return DecodeNextBlock(dataReadPtr, dataWritePtr, shiftedBitPosition, lengthToAdd, incrementSize,
                        decompBufferLimit, decompStartPtr, ref actualDecompressedSize);
                }
            }

            {
                long incrementSize = 1;
                var shiftedBitPosition = (long)bitShift | 8;
                var lengthToAdd = (long)((controlBits >> (int)(bitShift + 2) & 63) + 1);
                return DecodeNextBlock(dataReadPtr, dataWritePtr, shiftedBitPosition, lengthToAdd, incrementSize,
                    decompBufferLimit, decompStartPtr, ref actualDecompressedSize);
            }
        }

        if (decompBufferLimit <= (ulong)dataWritePtr)
        {
            return 0;
        }

        *(byte*)dataWritePtr = (byte)(controlBits >> (int)(bitShift + 1));
        var newWritePtr = dataWritePtr + 1;
        var nextReadPtr = dataReadPtr;
        var newBitOffset = (long)bitShift + 9;
        return UpdatePointersAfterCopy(newWritePtr, nextReadPtr, newBitOffset, decompBufferLimit,
            decompStartPtr, ref actualDecompressedSize);
    }

    private static long DecodeNextBlock(long dataReadPtr, long dataWritePtr, long shiftedBitPosition,
        long lengthToAdd,
        long incrementSize, ulong decompBufferLimit, long decompStartPtr, ref long actualDecompressedSize)
    {
        var copyLength = lengthToAdd;
        var nextDataReadPtr = (shiftedBitPosition >> 3) + dataReadPtr;
        var nextControlBits = *(ulong*)(nextDataReadPtr - 4);
        var nextBitShift = (ulong)(shiftedBitPosition & 7);
        var retryCounter = 0;
        var bitMaskIndex = 0;

        if ((1UL << (int)nextBitShift & nextControlBits) != 0)
        {
            return CheckBitMaskLoop(retryCounter, bitMaskIndex, nextControlBits, nextBitShift, dataWritePtr, copyLength,
                decompBufferLimit, decompStartPtr, nextDataReadPtr, incrementSize, ref actualDecompressedSize);
        }

        return ProcessDecodedLength(bitMaskIndex, nextBitShift, nextControlBits, incrementSize, dataWritePtr,
            copyLength, decompBufferLimit, decompStartPtr, nextDataReadPtr, ref actualDecompressedSize);
    }

    private static long CheckBitMaskLoop(int retryCounter, int bitMaskIndex, ulong nextControlBits,
        ulong nextBitShift,
        long dataWritePtr, long copyLength, ulong decompBufferLimit, long decompStartPtr, long nextDataReadPtr,
        long incrementSize, ref long actualDecompressedSize)
    {
        if (retryCounter > 10)
        {
            return 0;
        }

        var incrementedRetryCounter = retryCounter + 1;
        retryCounter = incrementedRetryCounter;
        bitMaskIndex = incrementedRetryCounter;
        if ((1UL << ((incrementedRetryCounter + (int)nextBitShift) & 31) & nextControlBits) != 0)
        {
            return CheckBitMaskLoop(retryCounter, bitMaskIndex, nextControlBits, nextBitShift, dataWritePtr,
                copyLength, decompBufferLimit, decompStartPtr, nextDataReadPtr, incrementSize,
                ref actualDecompressedSize);
        }

        return ProcessDecodedLength(bitMaskIndex, nextBitShift, nextControlBits, incrementSize, dataWritePtr,
            copyLength, decompBufferLimit, decompStartPtr, nextDataReadPtr, ref actualDecompressedSize);
    }

    private static long ProcessDecodedLength(int bitMaskIndex, ulong nextBitShift, ulong nextControlBits,
        long incrementSize, long dataWritePtr, long copyLength, ulong decompBufferLimit, long decompStartPtr,
        long nextDataReadPtr, ref long actualDecompressedSize)
    {
        var bitMask = 1UL << (bitMaskIndex & 31);
        var incrementedBitShift = (long)nextBitShift + 1;
        var decodedLength = (long)bitMask + incrementSize +
                            (long)((nextControlBits >> ((bitMaskIndex + (int)incrementedBitShift) & 31)) &
                                   (bitMask - 1));

        if (dataWritePtr - decompStartPtr < copyLength ||
            decompBufferLimit - (ulong)dataWritePtr < (ulong)decodedLength)
        {
            return 0;
        }

        var totalBitOffset = 2 * bitMaskIndex + incrementedBitShift;
        var nextReadPtr = nextDataReadPtr;
        var newBitOffset = totalBitOffset;

        if (decodedLength > 0)
        {
            var copyDestPtr = dataWritePtr - copyLength;
            var remainingLength = decodedLength;
            var copySourcePtr = dataWritePtr;
            return CopyDecompressedData(copyDestPtr, remainingLength, copySourcePtr, nextReadPtr,
                newBitOffset, decompBufferLimit, decompStartPtr, ref actualDecompressedSize);
        }

        return UpdatePointersAfterCopy(dataWritePtr, nextReadPtr, newBitOffset, decompBufferLimit,
            decompStartPtr, ref actualDecompressedSize);
    }

    private static long CopyDecompressedData(long copyDestPtr, long remainingLength, long copySourcePtr,
        long nextReadPtr, long newBitOffset, ulong decompBufferLimit,
        long decompStartPtr, ref long actualDecompressedSize)
    {
        while (remainingLength > 0)
        {
            *(byte*)copySourcePtr = *(byte*)copyDestPtr;
            copyDestPtr++;
            copySourcePtr++;
            remainingLength--;
        }

        var newWritePtr = copySourcePtr;

        return UpdatePointersAfterCopy(newWritePtr, nextReadPtr, newBitOffset, decompBufferLimit, decompStartPtr,
            ref actualDecompressedSize);
    }

    private static long UpdatePointersAfterCopy(long newWritePtr, long nextReadPtr, long newBitOffset,
        ulong decompBufferLimit, long decompStartPtr, ref long actualDecompressedSize)
    {
        var currentWritePtr = newWritePtr;
        var currentReadPtr = (newBitOffset >> 3) + nextReadPtr;
        var bitOffset = newBitOffset & 7;
        return ProcessControlBits(currentReadPtr, currentWritePtr, bitOffset, decompBufferLimit, decompStartPtr,
            ref actualDecompressedSize);
    }
}
