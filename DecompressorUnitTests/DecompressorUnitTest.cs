using System.Collections;
using ACSParser;

namespace DecompressorUnitTests;

public class DecompressorUnitTest
{
    /*
    [Fact]
    public void CountSeqBits_0()
    {
        var bits = new BitArray(new[] { 0b00000000 });
        var bitPosition = 0;
        var numOffsetSeqBits = Decompressor.CountSeqBits(bits, ref bitPosition);
        var nextValueSizeInBits = Decompressor.NextValueSizeBits[numOffsetSeqBits];

        Assert.Equal(0, numOffsetSeqBits);
        Assert.Equal(1, bitPosition);
        Assert.Equal(6, nextValueSizeInBits);
    }

    [Fact]
    public void CountSeqBits_1()
    {
        var bits = new BitArray(new[] { 0b00000001 });
        var bitPosition = 0;
        var numOffsetSeqBits = Decompressor.CountSeqBits(bits, ref bitPosition);
        var nextValueSizeInBits = Decompressor.NextValueSizeBits[numOffsetSeqBits];

        Assert.Equal(1, numOffsetSeqBits);
        Assert.Equal(2, bitPosition);
        Assert.Equal(9, nextValueSizeInBits);
    }

    [Fact]
    public void CountSeqBits_2()
    {
        var bits = new BitArray(new[] { 0b00000011 });
        var bitPosition = 0;
        var numOffsetSeqBits = Decompressor.CountSeqBits(bits, ref bitPosition);
        var nextValueSizeInBits = Decompressor.NextValueSizeBits[numOffsetSeqBits];
        
        Assert.Equal(2, numOffsetSeqBits);
        Assert.Equal(3, bitPosition);
        Assert.Equal(12, nextValueSizeInBits);
    }

    [Fact]
    public void CountSeqBits_3()
    {
        var bits = new BitArray(new[] { 0b00000111 });
        var bitPosition = 0;
        var numOffsetSeqBits = Decompressor.CountSeqBits(bits, ref bitPosition);
        var nextValueSizeInBits = Decompressor.NextValueSizeBits[numOffsetSeqBits];
        
        Assert.Equal(3, numOffsetSeqBits);
        Assert.Equal(3, bitPosition);
        Assert.Equal(20, nextValueSizeInBits);
    }

    [Fact]
    public void GetInt32()
    {
        var bits = new BitArray(new byte[] { 0b00000001, 0b00000000, 0b00000000, 0b00000000 });
        var value = Decompressor.GetInt32(bits, 0, 32);
        Assert.Equal(1, value);
    }

    [Fact]
    public void GetInt32_6_bits()
    {
        var nextValueSizeInBits = 6;
        var bits = new BitArray(new byte[] { 0b00000000, 0b00000000, 0b00000000 });
        var byteOffsetInResult = Decompressor.GetInt32(bits, 0, nextValueSizeInBits);

        Assert.Equal(0, byteOffsetInResult);

        var valueToAdd = Decompressor.ValueToAdd[nextValueSizeInBits];
        Assert.Equal(1, valueToAdd);

        byteOffsetInResult += valueToAdd;
        Assert.Equal(1, valueToAdd);
    }

    [Fact]
    public void GetInt32_9_bits()
    {
        var nextValueSizeInBits = 9;
        var bits = new BitArray(new byte[] { 0b00000000, 0b00000000, 0b00000000 });
        var byteOffsetInResult = Decompressor.GetInt32(bits, 0, nextValueSizeInBits);

        Assert.Equal(0, byteOffsetInResult);

        var valueToAdd = Decompressor.ValueToAdd[nextValueSizeInBits];
        Assert.Equal(65, valueToAdd);

        byteOffsetInResult += valueToAdd;
        Assert.Equal(65, valueToAdd);
    }

    [Fact]
    public void GetInt32_12_bits()
    {
        var nextValueSizeInBits = 12;
        var bits = new BitArray(new byte[] { 0b00000000, 0b00000000, 0b00000000 });
        var byteOffsetInResult = Decompressor.GetInt32(bits, 0, nextValueSizeInBits);

        Assert.Equal(0, byteOffsetInResult);

        var valueToAdd = Decompressor.ValueToAdd[nextValueSizeInBits];
        Assert.Equal(577, valueToAdd);

        byteOffsetInResult += valueToAdd;
        Assert.Equal(577, valueToAdd);
    }

    [Fact]
    public void GetInt32_20bits()
    {
        var nextValueSizeInBits = 20;
        var bits = new BitArray(new byte[] { 0b00000000, 0b00000000, 0b00000000 });
        var byteOffsetInResult = Decompressor.GetInt32(bits, 0, nextValueSizeInBits);

        Assert.Equal(0, byteOffsetInResult);

        var valueToAdd = Decompressor.ValueToAdd[nextValueSizeInBits];
        Assert.Equal(4673, valueToAdd);

        byteOffsetInResult += valueToAdd;
        Assert.Equal(4673, valueToAdd);
    }
    */

    [Fact]
    public void Sample()
    {
        byte[] compressedData = 
        {
            0x00, 0x40, 0x00, 0x04, 0x10, 0xD0, 0x90, 0x80,
            0x42, 0xED, 0x98, 0x01, 0xB7, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF
        };

        byte[] expectedUncompressedData = 
        {
            0x20, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0xA8, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        var uncompressedData = Decompressor.Decompress(compressedData, expectedUncompressedData.Length);

        Assert.Equal(expectedUncompressedData, uncompressedData);
    }
}
