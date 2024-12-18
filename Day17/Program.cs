using System.Collections.Immutable;
using System.Diagnostics;

var input = File.ReadAllLines("input.txt").Select(line => line.Split(" ").Last()).ToArray();
var inputA = long.Parse(input[0]);
var program = input[4].Split(',').Select(byte.Parse).ToArray();

var interpreterOutput = RunInterpreter(inputA);
var part1 = string.Join(',', interpreterOutput);
Console.WriteLine(part1);

var decompiledOutput = DecompiledProgram(inputA);
Debug.Assert(decompiledOutput.SequenceEqual(interpreterOutput));

var part2 = FindQuineAs(0, ImmutableStack.CreateRange(program)).Min();
Console.WriteLine(part2);

return;

List<byte> RunInterpreter(long regA)
{
    var regB = 0L;
    var regC = 0L;
    var output = new List<byte>();

    for (var pc = 0; pc < program.Length; pc += 2)
    {
        var operand = program[pc + 1];
        switch (program[pc])
        {
            case 0:
                regA >>= (int)GetComboOperand(operand);
                break;
            case 1:
                regB ^= operand;
                break;
            case 2:
                regB = GetComboOperand(operand) & 0b111;
                break;
            case 3:
                if (regA != 0)
                {
                    pc = operand - 2;
                }

                break;
            case 4:
                regB ^= regC;
                break;
            case 5:
                output.Add((byte)(GetComboOperand(operand) & 0b111));
                break;
            case 6:
                regB = regA >> (int)GetComboOperand(operand);
                break;
            case 7:
                regC = regA >> (int)GetComboOperand(operand);
                break;
        }
    }

    return output;

    long GetComboOperand(byte x) => x switch { <= 3 => x, 4 => regA, 5 => regB, 6 => regC };
}

List<byte> DecompiledProgram(long a)
{
    /*
     *  0: b = a & 0b111
     *  2: b = b ^ 3
     *  4: c = a >> b
     *  6: b = b ^ 5
     *  8: a = a >> 3
     * 10: b = b ^ c
     * 12: output (b & 0b111)
     * 14: if a != 0 goto 0
     */
    var output = new List<byte>();
    while (a != 0)
    {
        var tmp = (byte)(a & 0b111 ^ 3);
        var outputByte = (byte)(tmp ^ 5 ^ ((a >> tmp) & 0b111));
        output.Add(outputByte);
        a >>= 3;
    }
    
    return output;
}

IEnumerable<long> FindQuineAs(long candidateA, ImmutableStack<byte> expectedOutput)
{
    if (expectedOutput.IsEmpty)
    {
        yield return candidateA;
        yield break;
    }

    var nextExpectedOutput = expectedOutput.Pop(out var outputByte);
    for (byte last3BitsOfA = 0; last3BitsOfA < 8; last3BitsOfA++)
    {
        var nextCandidateA = candidateA << 3 | last3BitsOfA;
        var tmp = last3BitsOfA ^ 3;
        var computedOutput = tmp ^ 5 ^ ((nextCandidateA >> tmp) & 0b111);
        if (computedOutput == outputByte)
        {
            foreach (var a in FindQuineAs(nextCandidateA, nextExpectedOutput))
            {
                yield return a;
            }
        }
    }
}
