using System;
using System.Collections;
using System.Diagnostics;

namespace ACSParser;

public unsafe static class Decompressor
{
    public static byte[] Decompress(byte[] compressed, int expectedDecompressedSize)
    {
        uint compressedSize = (uint)compressed.Length;
        
        // Allocate memory for decompressed data
        byte[] decompressed = new byte[expectedDecompressedSize];

        // Temporary buffer for decompression (size based on the function's behavior)
        byte[] tempBuffer = new byte[compressedSize + expectedDecompressedSize];

        // Copy compressed data to temp_buffer (the function seems to work backwards)
        Array.Copy(compressed, tempBuffer, compressedSize);

        // Variable to store the actual decompressed size
        long actualDecompressedSize = 0;

        // Call the decompression function

        fixed (byte* tempBufferPtr = tempBuffer)
        fixed (byte* decompressedPtr = decompressed)
        {
            long result = DecompressCore(
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
    
    
    public static long DecompressCore(long bufferPtr, long compressedDataLen, long decompStartPtr, long decompEndPtr,
        out long actualDecompressedSize)
    {
        actualDecompressedSize = 0;

        if (compressedDataLen <= 7)
        {
            return 0;
        }

        long endBufferPtr = compressedDataLen + bufferPtr;
        int endPaddingCounter = 0;
        endBufferPtr--;
        long result = 0;

        unsafe
        {
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
                    else
                    {
                        ulong decompBufferLimit = (ulong)(decompEndPtr + decompStartPtr);
                        long currentWritePtr = decompStartPtr;
                        long currentReadPtr = bufferPtr + 5;
                        long bitOffset = 0;
                        return ProcessControlBits(currentReadPtr, currentWritePtr, bitOffset, decompBufferLimit,
                            decompStartPtr, ref actualDecompressedSize);
                    }
                }

                endBufferPtr--;
                result = 0;
            }
        }

        return result;
    }

    private static unsafe long ProcessControlBits(long currentReadPtr, long currentWritePtr, long bitOffset,
        ulong decompBufferLimit, long decompStartPtr, ref long actualDecompressedSize)
    {
        ulong bitShift = (ulong)bitOffset;
        long dataReadPtr = currentReadPtr;
        long dataWritePtr = currentWritePtr;
        ulong controlBits = *(ulong*)(dataReadPtr - 4);

        if ((controlBits & (1UL << (int)bitShift)) != 0)
        {
            if ((controlBits & (2UL << (int)bitShift)) != 0)
            {
                if ((controlBits & (4UL << (int)bitShift)) != 0)
                {
                    ulong maskedValue = controlBits >> (int)(bitShift + 4);
                    if ((controlBits & (8UL << (int)bitShift)) != 0)
                    {
                        long offsetValue = (long)(maskedValue & 0xfffff);
                        if (offsetValue == 0xfffff)
                        {
                            actualDecompressedSize = dataWritePtr - decompStartPtr;
                            return 1;
                        }
                        else
                        {
                            long incrementSize = 2;
                            long shiftedBitPosition = (long)bitShift | 24;
                            long lengthToAdd = offsetValue + 0x1241;
                            return DecodeNextBlock(dataReadPtr, dataWritePtr, shiftedBitPosition, lengthToAdd,
                                incrementSize, decompBufferLimit, decompStartPtr, ref actualDecompressedSize);
                        }
                    }
                    else
                    {
                        long incrementSize = 1;
                        long shiftedBitPosition = (long)bitShift | 16;
                        long lengthToAdd = ((long)maskedValue & 4095) + 577;
                        return DecodeNextBlock(dataReadPtr, dataWritePtr, shiftedBitPosition, lengthToAdd,
                            incrementSize, decompBufferLimit, decompStartPtr, ref actualDecompressedSize);
                    }
                }
                else
                {
                    long incrementSize = 1;
                    long shiftedBitPosition = (long)bitShift + 12;
                    long lengthToAdd = (long)((controlBits >> (int)(bitShift + 3) & 511) + 65);
                    return DecodeNextBlock(dataReadPtr, dataWritePtr, shiftedBitPosition, lengthToAdd, incrementSize,
                        decompBufferLimit, decompStartPtr, ref actualDecompressedSize);
                }
            }
            else
            {
                long incrementSize = 1;
                long shiftedBitPosition = (long)bitShift | 8;
                long lengthToAdd = (long)((controlBits >> (int)(bitShift + 2) & 63) + 1);
                return DecodeNextBlock(dataReadPtr, dataWritePtr, shiftedBitPosition, lengthToAdd, incrementSize,
                    decompBufferLimit, decompStartPtr, ref actualDecompressedSize);
            }
        }
        else
        {
            if (decompBufferLimit <= (ulong)dataWritePtr)
            {
                return 0;
            }
            else
            {
                *(byte*)dataWritePtr = (byte)(controlBits >> (int)(bitShift + 1));
                long newWritePtr = dataWritePtr + 1;
                long nextReadPtr = dataReadPtr;
                long newBitOffset = (long)bitShift + 9;
                return UpdatePointersAfterCopy(newWritePtr, nextReadPtr, newBitOffset, decompBufferLimit,
                    decompStartPtr, ref actualDecompressedSize);
            }
        }
    }

    private static unsafe long DecodeNextBlock(long dataReadPtr, long dataWritePtr, long shiftedBitPosition,
        long lengthToAdd,
        long incrementSize, ulong decompBufferLimit, long decompStartPtr, ref long actualDecompressedSize)
    {
        long copyLength = lengthToAdd;
        long nextDataReadPtr = (shiftedBitPosition >> 3) + dataReadPtr;
        ulong nextControlBits = *(ulong*)(nextDataReadPtr - 4);
        ulong nextBitShift = (ulong)(shiftedBitPosition & 7);
        int retryCounter = 0;
        int bitMaskIndex = 0;

        if ((1UL << (int)nextBitShift & nextControlBits) != 0)
        {
            return CheckBitMaskLoop(retryCounter, bitMaskIndex, nextControlBits, nextBitShift, dataWritePtr, copyLength,
                decompBufferLimit, decompStartPtr, nextDataReadPtr, incrementSize, ref actualDecompressedSize);
        }
        else
        {
            return ProcessDecodedLength(bitMaskIndex, nextBitShift, nextControlBits, incrementSize, dataWritePtr,
                copyLength, decompBufferLimit, decompStartPtr, nextDataReadPtr, ref actualDecompressedSize);
        }
    }

    private static unsafe long CheckBitMaskLoop(int retryCounter, int bitMaskIndex, ulong nextControlBits,
        ulong nextBitShift,
        long dataWritePtr, long copyLength, ulong decompBufferLimit, long decompStartPtr, long nextDataReadPtr,
        long incrementSize, ref long actualDecompressedSize)
    {
        if (retryCounter > 10)
        {
            return 0;
        }
        else
        {
            int incrementedRetryCounter = retryCounter + 1;
            retryCounter = incrementedRetryCounter;
            bitMaskIndex = incrementedRetryCounter;
            if ((1UL << ((incrementedRetryCounter + (int)nextBitShift) & 31) & nextControlBits) != 0)
            {
                return CheckBitMaskLoop(retryCounter, bitMaskIndex, nextControlBits, nextBitShift, dataWritePtr,
                    copyLength, decompBufferLimit, decompStartPtr, nextDataReadPtr, incrementSize,
                    ref actualDecompressedSize);
            }
            else
            {
                return ProcessDecodedLength(bitMaskIndex, nextBitShift, nextControlBits, incrementSize, dataWritePtr,
                    copyLength, decompBufferLimit, decompStartPtr, nextDataReadPtr, ref actualDecompressedSize);
            }
        }
    }

    private static unsafe long ProcessDecodedLength(int bitMaskIndex, ulong nextBitShift, ulong nextControlBits,
        long incrementSize, long dataWritePtr, long copyLength, ulong decompBufferLimit, long decompStartPtr,
        long nextDataReadPtr, ref long actualDecompressedSize)
    {
        ulong bitMask = 1UL << (bitMaskIndex & 31);
        long incrementedBitShift = (long)nextBitShift + 1;
        long decodedLength = (long)bitMask + incrementSize +
                             (long)((nextControlBits >> ((bitMaskIndex + (int)incrementedBitShift) & 31)) &
                                    (bitMask - 1));

        if (dataWritePtr - decompStartPtr < copyLength ||
            decompBufferLimit - (ulong)dataWritePtr < (ulong)decodedLength)
        {
            return 0;
        }
        else
        {
            long totalBitOffset = 2 * bitMaskIndex + incrementedBitShift;
            long newWritePtr = dataWritePtr;
            long nextReadPtr = nextDataReadPtr;
            long newBitOffset = totalBitOffset;

            if (decodedLength > 0)
            {
                long copyDestPtr = dataWritePtr - copyLength;
                long remainingLength = decodedLength;
                long copySourcePtr = dataWritePtr;
                return CopyDecompressedData(copyDestPtr, remainingLength, copySourcePtr, newWritePtr, nextReadPtr,
                    newBitOffset, totalBitOffset, decompBufferLimit, decompStartPtr, ref actualDecompressedSize);
            }
            else
            {
                return UpdatePointersAfterCopy(newWritePtr, nextReadPtr, newBitOffset, decompBufferLimit,
                    decompStartPtr, ref actualDecompressedSize);
            }
        }
    }

    private static unsafe long CopyDecompressedData(long copyDestPtr, long remainingLength, long copySourcePtr,
        long newWritePtr, long nextReadPtr, long newBitOffset, long totalBitOffset, ulong decompBufferLimit,
        long decompStartPtr, ref long actualDecompressedSize)
    {
        while (remainingLength > 0)
        {
            *(byte*)copySourcePtr = *(byte*)copyDestPtr;
            copyDestPtr++;
            copySourcePtr++;
            remainingLength--;
        }

        newWritePtr = copySourcePtr;

        return UpdatePointersAfterCopy(newWritePtr, nextReadPtr, newBitOffset, decompBufferLimit, decompStartPtr,
            ref actualDecompressedSize);
    }

    private static unsafe long UpdatePointersAfterCopy(long newWritePtr, long nextReadPtr, long newBitOffset,
        ulong decompBufferLimit, long decompStartPtr, ref long actualDecompressedSize)
    {
        long currentWritePtr = newWritePtr;
        long currentReadPtr = (newBitOffset >> 3) + nextReadPtr;
        long bitOffset = newBitOffset & 7;
        return ProcessControlBits(currentReadPtr, currentWritePtr, bitOffset, decompBufferLimit, decompStartPtr,
            ref actualDecompressedSize);
    }
}
