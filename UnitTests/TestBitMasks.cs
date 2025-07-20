using ChessBotCore;

namespace TestProject1;

public class TestBitMasks {
    [Fact]
    public void ColMask_0() {
        //arrange
        var mask = BitMask.Col[0];
        // ulong expected = 0b1000_0000
        ulong expected = 0x80_80_80_80_80_80_80_80;


        //assert
        Assert.Equal(expected, mask);
    }

    [Fact]
    public void PrintColMask_5() {
        //arrange
        string expected = """
                          00000100
                          00000100
                          00000100
                          00000100
                          00000100
                          00000100
                          00000100
                          00000100
                          """;
        //act
        var mask = BitMask.Col[5];
        var maskString = mask.Print();
        
        //assert
        Assert.Equal(expected, maskString);
    }
    [Fact]
    public void PrintColMask_7() {
        //arrange
        string expected = """
                          00000001
                          00000001
                          00000001
                          00000001
                          00000001
                          00000001
                          00000001
                          00000001
                          """;
        //act
        var mask = BitMask.Col[7];
        var maskString = mask.Print();
        
        //assert
        Assert.Equal(expected, maskString);
    }

    [Fact]
    public void PrintColMask_0() {
        //arrange
        string expected = """
                          10000000
                          10000000
                          10000000
                          10000000
                          10000000
                          10000000
                          10000000
                          10000000
                          """;
        //act
        var mask = BitMask.Col[0];
        var maskString = mask.Print();
        
        //assert
        Assert.Equal(expected, maskString);
    }

    [Fact]
    public void PrintRowMask_0() {
        //arrange
        string expected = """
                          00000000
                          00000000
                          00000000
                          00000000
                          00000000
                          00000000
                          00000000
                          11111111
                          """;
        //act
        var mask = BitMask.Row[0];
        var maskString = mask.Print();
        
        //assert
        Assert.Equal(expected, maskString);
    }

    [Fact]
    public void PrintRowMask_7() {
        //arrange
        string expected = """
                          11111111
                          00000000
                          00000000
                          00000000
                          00000000
                          00000000
                          00000000
                          00000000
                          """;
        //act
        var mask = BitMask.Row[7];
        var maskString = mask.Print();
        
        //assert
        Assert.Equal(expected, maskString);
    }

    [Fact]
    public void MaskCombinig() {
        //arrange
        // ulong expected = 0xFF_FF_FF_FF_FF_FF_00_FF;
        //act

        var one = BitMask.Row[1];

        var two =
            ~ (BitMask.Row[0] |
               BitMask.Row[2] |
               BitMask.Row[3] |
               BitMask.Row[4] |
               BitMask.Row[5] |
               BitMask.Row[6] |
               BitMask.Row[7]);

        //assert
        Assert.Equal(one, two);
    }
}