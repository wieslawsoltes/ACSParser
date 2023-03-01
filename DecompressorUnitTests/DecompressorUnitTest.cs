using System.Collections;
using ACSParser;

namespace DecompressorUnitTests;

public class DecompressorUnitTest
{

    [Fact]
    public void CountSeqBits_0()
    {
        var bits = new BitArray(4)
        {
            [0] = false,
            [1] = false,
            [2] = false,
            [3] = false,
        };

        var bitPosition = 0;

        var numOffsetSeqBits = Decompressor.CountSeqBits(bits, ref bitPosition);
        
        Assert.Equal(0, numOffsetSeqBits);
        Assert.Equal(1, bitPosition);
    }

    [Fact]
    public void CountSeqBits_1()
    {
        var bits = new BitArray(4)
        {
            [0] = true,
            [1] = false,
            [2] = false,
            [3] = false,
        };

        var bitPosition = 0;

        var numOffsetSeqBits = Decompressor.CountSeqBits(bits, ref bitPosition);
        
        Assert.Equal(1, numOffsetSeqBits);
        Assert.Equal(2, bitPosition);
    }

    [Fact]
    public void CountSeqBits_2()
    {
        var bits = new BitArray(4)
        {
            [0] = true,
            [1] = true,
            [2] = false,
            [3] = false,
        };

        var bitPosition = 0;

        var numOffsetSeqBits = Decompressor.CountSeqBits(bits, ref bitPosition);
        
        Assert.Equal(2, numOffsetSeqBits);
        Assert.Equal(3, bitPosition);
    }

    [Fact]
    public void CountSeqBits_3()
    {
        var bits = new BitArray(4)
        {
            [0] = true,
            [1] = true,
            [2] = true,
            [3] = false,
        };

        var bitPosition = 0;

        var numOffsetSeqBits = Decompressor.CountSeqBits(bits, ref bitPosition);
        
        Assert.Equal(3, numOffsetSeqBits);
        Assert.Equal(3, bitPosition);
    }

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
