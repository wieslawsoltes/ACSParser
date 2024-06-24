namespace ACSParser;

public static class Decompressor
{
    public static BYTE[] Decompress(BYTE[] compressed, LONG expectedDecompressedSize)
    {
        var compressedSize = (ULONG)compressed.Length;
        var decompressed = new BYTE[expectedDecompressedSize];
        var tempBuffer = new BYTE[compressedSize + expectedDecompressedSize];

        Array.Copy(compressed, tempBuffer, compressedSize);

        var result = DecompressCore(tempBuffer, compressedSize, decompressed, out _);
        if (result == 1)
        {
            return decompressed;
        }

        throw new InvalidOperationException("Decompression failed");
    }

    private static long DecompressCore(
        BYTE[] buffer,
        ULONG compressedDataLen,
        BYTE[] decompBuffer,
        out long actualDecompressedSize)
    {
        actualDecompressedSize = 0;

        if (compressedDataLen <= 7)
        {
            return 0;
        }

        var endBufferPtr = (LONG)compressedDataLen - 1;
        var endPaddingCounter = 0;
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

                var currentWritePtr = 0;
                var currentReadPtr = 5;
                var bitOffset = 0;
                return ProcessControlBits(buffer, decompBuffer, currentReadPtr, currentWritePtr, bitOffset, ref actualDecompressedSize);
            }

            endBufferPtr--;
            result = 0;
        }

        return result;
    }

    private static long ProcessControlBits(
        BYTE[] buffer,
        BYTE[] decompBuffer,
        LONG currentReadPtr,
        LONG currentWritePtr,
        LONG bitOffset,
        ref long actualDecompressedSize)
    {
        var controlBits = BitConverter.ToUInt64(buffer, currentReadPtr - 4);

        if ((controlBits & (1UL << bitOffset)) == 0)
        {
            return HandleLiteralByte(buffer, decompBuffer, controlBits, currentReadPtr, currentWritePtr, bitOffset, ref actualDecompressedSize);
        }

        LONG shiftedBitPosition;
        LONG lengthToAdd;
        LONG incrementSize;

        if ((controlBits & (2UL << bitOffset)) == 0)
        {
            shiftedBitPosition = bitOffset | 8;
            lengthToAdd = (LONG)((controlBits >> (bitOffset + 2) & 63) + 1);
            incrementSize = 1;
        }
        else if ((controlBits & (4UL << bitOffset)) == 0)
        {
            shiftedBitPosition = bitOffset + 12;
            lengthToAdd = (LONG)((controlBits >> (bitOffset + 3) & 511) + 65);
            incrementSize = 1;
        }
        else
        {
            var maskedValue = controlBits >> (bitOffset + 4);
            if ((controlBits & (8UL << bitOffset)) == 0)
            {
                shiftedBitPosition = bitOffset | 16;
                lengthToAdd = (LONG)(maskedValue & 4095) + 577;
                incrementSize = 1;
            }
            else
            {
                var offsetValue = (long)(maskedValue & 0xfffff);
                if (offsetValue == 0xfffff)
                {
                    actualDecompressedSize = currentWritePtr;
                    return 1;
                }

                shiftedBitPosition = bitOffset | 24;
                lengthToAdd = (LONG)offsetValue + 0x1241;
                incrementSize = 2;
            }
        }

        return DecodeNextBlock(buffer, decompBuffer, currentReadPtr, currentWritePtr, shiftedBitPosition, lengthToAdd, incrementSize, ref actualDecompressedSize);
    }

    private static long HandleLiteralByte(
        BYTE[] buffer,
        BYTE[] decompBuffer,
        ulong controlBits,
        LONG currentReadPtr,
        LONG currentWritePtr,
        LONG bitOffset,
        ref long actualDecompressedSize)
    {
        if (currentWritePtr >= decompBuffer.Length)
        {
            return 0;
        }

        decompBuffer[currentWritePtr] = (BYTE)(controlBits >> (bitOffset + 1));
        var newWritePtr = currentWritePtr + 1;
        var newBitOffset = bitOffset + 9;
        return UpdatePointersAfterCopy(buffer, decompBuffer, newWritePtr, currentReadPtr, newBitOffset, ref actualDecompressedSize);
    }

    private static long DecodeNextBlock(
        BYTE[] buffer,
        BYTE[] decompBuffer,
        LONG dataReadPtr,
        LONG dataWritePtr,
        LONG shiftedBitPosition,
        LONG lengthToAdd,
        LONG incrementSize,
        ref long actualDecompressedSize)
    {
        var copyLength = lengthToAdd;
        var nextDataReadPtr = (shiftedBitPosition >> 3) + dataReadPtr;
        var nextControlBits = BitConverter.ToUInt64(buffer, nextDataReadPtr - 4);
        var nextBitShift = shiftedBitPosition & 7;
        var retryCounter = 0;
        var bitMaskIndex = 0;

        return (1UL << nextBitShift & nextControlBits) != 0 
            ? CheckBitMaskLoop(buffer, decompBuffer, retryCounter, bitMaskIndex, nextControlBits, nextBitShift, dataWritePtr, copyLength, nextDataReadPtr, incrementSize, ref actualDecompressedSize) 
            : ProcessDecodedLength(buffer, decompBuffer, bitMaskIndex, nextBitShift, nextControlBits, incrementSize, dataWritePtr, copyLength, nextDataReadPtr, ref actualDecompressedSize);
    }

    private static long CheckBitMaskLoop(
        BYTE[] buffer,
        BYTE[] decompBuffer,
        LONG retryCounter,
        LONG bitMaskIndex,
        ulong nextControlBits,
        LONG nextBitShift,
        LONG dataWritePtr,
        LONG copyLength,
        LONG nextDataReadPtr,
        LONG incrementSize,
        ref long actualDecompressedSize)
    {
        if (retryCounter > 10)
        {
            return 0;
        }

        var incrementedRetryCounter = retryCounter + 1;
        retryCounter = incrementedRetryCounter;
        bitMaskIndex = incrementedRetryCounter;
        return (1UL << ((incrementedRetryCounter + nextBitShift) & 31) & nextControlBits) != 0 
            ? CheckBitMaskLoop(buffer, decompBuffer, retryCounter, bitMaskIndex, nextControlBits, nextBitShift, dataWritePtr, copyLength, nextDataReadPtr, incrementSize, ref actualDecompressedSize) 
            : ProcessDecodedLength(buffer, decompBuffer, bitMaskIndex, nextBitShift, nextControlBits, incrementSize, dataWritePtr, copyLength, nextDataReadPtr, ref actualDecompressedSize);
    }

    private static long ProcessDecodedLength(
        BYTE[] buffer,
        BYTE[] decompBuffer,
        LONG bitMaskIndex,
        LONG nextBitShift,
        ulong nextControlBits,
        LONG incrementSize,
        LONG dataWritePtr,
        LONG copyLength,
        LONG nextDataReadPtr,
        ref long actualDecompressedSize)
    {
        var bitMask = 1UL << (bitMaskIndex & 31);
        var incrementedBitShift = nextBitShift + 1;
        var decodedLength = (LONG)bitMask + incrementSize + (LONG)((nextControlBits >> ((bitMaskIndex + incrementedBitShift) & 31)) & (bitMask - 1));

        if (dataWritePtr < copyLength || decompBuffer.Length - dataWritePtr < decodedLength)
        {
            return 0;
        }

        var totalBitOffset = 2 * bitMaskIndex + incrementedBitShift;
        var newWritePtr = dataWritePtr;
        var newBitOffset = totalBitOffset;

        if (decodedLength > 0)
        {
            var copyDestPtr = dataWritePtr - copyLength;
            return CopyDecompressedData(
                buffer,
                decompBuffer,
                copyDestPtr,
                decodedLength,
                dataWritePtr,
                newWritePtr,
                nextDataReadPtr,
                newBitOffset,
                totalBitOffset,
                ref actualDecompressedSize);
        }

        return UpdatePointersAfterCopy(
            buffer,
            decompBuffer,
            newWritePtr,
            nextDataReadPtr,
            newBitOffset,
            ref actualDecompressedSize);
    }

    private static long CopyDecompressedData(
        BYTE[] buffer,
        BYTE[] decompBuffer,
        LONG copyDestPtr,
        LONG remainingLength,
        LONG copySourcePtr,
        LONG newWritePtr,
        LONG nextReadPtr,
        LONG newBitOffset,
        LONG totalBitOffset,
        ref long actualDecompressedSize)
    {
        while (remainingLength > 0)
        {
            decompBuffer[copySourcePtr] = decompBuffer[copyDestPtr];
            copyDestPtr++;
            copySourcePtr++;
            remainingLength--;
        }

        newWritePtr = copySourcePtr;

        return UpdatePointersAfterCopy(buffer, decompBuffer, newWritePtr, nextReadPtr, newBitOffset, ref actualDecompressedSize);
    }

    private static long UpdatePointersAfterCopy(
        BYTE[] buffer,
        BYTE[] decompBuffer,
        LONG newWritePtr,
        LONG nextReadPtr,
        LONG newBitOffset,
        ref long actualDecompressedSize)
    {
        var currentWritePtr = newWritePtr;
        var currentReadPtr = (newBitOffset >> 3) + nextReadPtr;
        var bitOffset = newBitOffset & 7;
        return ProcessControlBits(buffer, decompBuffer, currentReadPtr, currentWritePtr, bitOffset, ref actualDecompressedSize);
    }
}
