using System;

namespace ACSParser
{
    public static class Decompressor
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

            // Call the decompression function

            long result = DecompressCore(
                tempBuffer,
                compressedSize,
                decompressed,
                out long actualDecompressedSize);

            if (result == 1)
            {
                return decompressed;
            }
            else
            {
                throw new InvalidOperationException("Decompression failed");
            }
        }
        
        private static long DecompressCore(byte[] buffer, uint compressedDataLen, byte[] decompBuffer,
            out long actualDecompressedSize)
        {
            actualDecompressedSize = 0;

            if (compressedDataLen <= 7)
            {
                return 0;
            }

            int endBufferPtr = (int)compressedDataLen - 1;
            int endPaddingCounter = 0;
            long result = 0;

            while (buffer[endBufferPtr] == 0xFF)
            {
                endPaddingCounter++;
                if (endPaddingCounter >= 6)
                {
                    result = 0;
                    if (buffer[0] != 0)
                    {
                        return result;
                    }
                    else
                    {
                        int currentWritePtr = 0;
                        int currentReadPtr = 5;
                        int bitOffset = 0;
                        return ProcessControlBits(buffer, decompBuffer, currentReadPtr, currentWritePtr, bitOffset,
                            ref actualDecompressedSize);
                    }
                }

                endBufferPtr--;
                result = 0;
            }

            return result;
        }

        private static long ProcessControlBits(byte[] buffer, byte[] decompBuffer, int currentReadPtr,
            int currentWritePtr, int bitOffset, ref long actualDecompressedSize)
        {
            ulong controlBits = BitConverter.ToUInt64(buffer, currentReadPtr - 4);

            if ((controlBits & (1UL << bitOffset)) == 0)
            {
                return HandleLiteralByte(buffer, decompBuffer, controlBits, currentReadPtr, currentWritePtr, bitOffset,
                    ref actualDecompressedSize);
            }

            int shiftedBitPosition;
            int lengthToAdd;
            int incrementSize;

            if ((controlBits & (2UL << bitOffset)) == 0)
            {
                shiftedBitPosition = bitOffset | 8;
                lengthToAdd = (int)((controlBits >> (bitOffset + 2) & 63) + 1);
                incrementSize = 1;
            }
            else if ((controlBits & (4UL << bitOffset)) == 0)
            {
                shiftedBitPosition = bitOffset + 12;
                lengthToAdd = (int)((controlBits >> (bitOffset + 3) & 511) + 65);
                incrementSize = 1;
            }
            else
            {
                ulong maskedValue = controlBits >> (bitOffset + 4);
                if ((controlBits & (8UL << bitOffset)) == 0)
                {
                    shiftedBitPosition = bitOffset | 16;
                    lengthToAdd = (int)(maskedValue & 4095) + 577;
                    incrementSize = 1;
                }
                else
                {
                    long offsetValue = (long)(maskedValue & 0xfffff);
                    if (offsetValue == 0xfffff)
                    {
                        actualDecompressedSize = currentWritePtr;
                        return 1;
                    }

                    shiftedBitPosition = bitOffset | 24;
                    lengthToAdd = (int)offsetValue + 0x1241;
                    incrementSize = 2;
                }
            }

            return DecodeNextBlock(buffer, decompBuffer, currentReadPtr, currentWritePtr, shiftedBitPosition,
                lengthToAdd, incrementSize, ref actualDecompressedSize);
        }

        private static long HandleLiteralByte(byte[] buffer, byte[] decompBuffer, ulong controlBits, int currentReadPtr,
            int currentWritePtr, int bitOffset, ref long actualDecompressedSize)
        {
            if (currentWritePtr >= decompBuffer.Length)
            {
                return 0;
            }

            decompBuffer[currentWritePtr] = (byte)(controlBits >> (bitOffset + 1));
            int newWritePtr = currentWritePtr + 1;
            int newBitOffset = bitOffset + 9;
            return UpdatePointersAfterCopy(buffer, decompBuffer, newWritePtr, currentReadPtr, newBitOffset,
                ref actualDecompressedSize);
        }

        private static long DecodeNextBlock(byte[] buffer, byte[] decompBuffer, int dataReadPtr, int dataWritePtr,
            int shiftedBitPosition, int lengthToAdd, int incrementSize, ref long actualDecompressedSize)
        {
            int copyLength = lengthToAdd;
            int nextDataReadPtr = (shiftedBitPosition >> 3) + dataReadPtr;
            ulong nextControlBits = BitConverter.ToUInt64(buffer, nextDataReadPtr - 4);
            int nextBitShift = shiftedBitPosition & 7;
            int retryCounter = 0;
            int bitMaskIndex = 0;

            if ((1UL << nextBitShift & nextControlBits) != 0)
            {
                return CheckBitMaskLoop(buffer, decompBuffer, retryCounter, bitMaskIndex, nextControlBits, nextBitShift,
                    dataWritePtr, copyLength, nextDataReadPtr, incrementSize, ref actualDecompressedSize);
            }
            else
            {
                return ProcessDecodedLength(buffer, decompBuffer, bitMaskIndex, nextBitShift, nextControlBits,
                    incrementSize, dataWritePtr, copyLength, nextDataReadPtr, ref actualDecompressedSize);
            }
        }

        private static long CheckBitMaskLoop(byte[] buffer, byte[] decompBuffer, int retryCounter, int bitMaskIndex,
            ulong nextControlBits, int nextBitShift, int dataWritePtr, int copyLength, int nextDataReadPtr,
            int incrementSize, ref long actualDecompressedSize)
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
                if ((1UL << ((incrementedRetryCounter + nextBitShift) & 31) & nextControlBits) != 0)
                {
                    return CheckBitMaskLoop(buffer, decompBuffer, retryCounter, bitMaskIndex, nextControlBits,
                        nextBitShift, dataWritePtr, copyLength, nextDataReadPtr, incrementSize,
                        ref actualDecompressedSize);
                }
                else
                {
                    return ProcessDecodedLength(buffer, decompBuffer, bitMaskIndex, nextBitShift, nextControlBits,
                        incrementSize, dataWritePtr, copyLength, nextDataReadPtr, ref actualDecompressedSize);
                }
            }
        }

        private static long ProcessDecodedLength(byte[] buffer, byte[] decompBuffer, int bitMaskIndex, int nextBitShift,
            ulong nextControlBits, int incrementSize, int dataWritePtr, int copyLength, int nextDataReadPtr,
            ref long actualDecompressedSize)
        {
            ulong bitMask = 1UL << (bitMaskIndex & 31);
            int incrementedBitShift = nextBitShift + 1;
            int decodedLength = (int)bitMask + incrementSize +
                                (int)((nextControlBits >> ((bitMaskIndex + incrementedBitShift) & 31)) & (bitMask - 1));

            if (dataWritePtr < copyLength || decompBuffer.Length - dataWritePtr < decodedLength)
            {
                return 0;
            }
            else
            {
                int totalBitOffset = 2 * bitMaskIndex + incrementedBitShift;
                int newWritePtr = dataWritePtr;
                int newBitOffset = totalBitOffset;

                if (decodedLength > 0)
                {
                    int copyDestPtr = dataWritePtr - copyLength;
                    return CopyDecompressedData(buffer, decompBuffer, copyDestPtr, decodedLength, dataWritePtr,
                        newWritePtr, nextDataReadPtr, newBitOffset, totalBitOffset, ref actualDecompressedSize);
                }
                else
                {
                    return UpdatePointersAfterCopy(buffer, decompBuffer, newWritePtr, nextDataReadPtr, newBitOffset,
                        ref actualDecompressedSize);
                }
            }
        }

        private static long CopyDecompressedData(byte[] buffer, byte[] decompBuffer, int copyDestPtr,
            int remainingLength, int copySourcePtr, int newWritePtr, int nextReadPtr, int newBitOffset,
            int totalBitOffset, ref long actualDecompressedSize)
        {
            while (remainingLength > 0)
            {
                decompBuffer[copySourcePtr] = decompBuffer[copyDestPtr];
                copyDestPtr++;
                copySourcePtr++;
                remainingLength--;
            }

            newWritePtr = copySourcePtr;

            return UpdatePointersAfterCopy(buffer, decompBuffer, newWritePtr, nextReadPtr, newBitOffset,
                ref actualDecompressedSize);
        }

        private static long UpdatePointersAfterCopy(byte[] buffer, byte[] decompBuffer, int newWritePtr,
            int nextReadPtr, int newBitOffset, ref long actualDecompressedSize)
        {
            int currentWritePtr = newWritePtr;
            int currentReadPtr = (newBitOffset >> 3) + nextReadPtr;
            int bitOffset = newBitOffset & 7;
            return ProcessControlBits(buffer, decompBuffer, currentReadPtr, currentWritePtr, bitOffset,
                ref actualDecompressedSize);
        }
    }
}
