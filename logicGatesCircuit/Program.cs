using System;
using System.Collections.Generic;

class Gate
{
    public string gateType;
    public int gateNum;
    public string[] input = new string[2];

    public Gate(int count)
    {
        gateNum = count;
        gateType = ReturnValidGate();
    }
    public string Run()
    {
        for (int inputCount = 0; inputCount < 2; inputCount++) { if (input[inputCount].StartsWith('m')) { input[inputCount] = ReturnManualInput(inputCount); } }
        if (gateType == "OR") { if (input[0] == "1" || input[1] == "1") { return "1"; } return "0"; }
        else if (gateType == "AND") { if (input[0] == "1" && input[1] == "1") { return "1"; } return "0"; }
        else if (gateType == "XOR") { if ((input[0] == "1" && input[1] == "0") || (input[0] == "0" && input[1] == "1")) { return "1"; } return "0"; }
        else if (gateType == "NAND") { if (input[0] == "1" && input[1] == "1") { return "0"; } return "1"; }
        else if (gateType == "NOR") { if (input[0] == "1" || input[1] == "1") { return "0"; } return "1"; }
        else { throw new Exception("why"); }
    }
    private string ReturnManualInput(int inputOrd)
    {
        string userInput;
        while (true)
        {
            Console.Write($"\nEnter input{inputOrd} for gate {gateNum}: ");
            userInput = Console.ReadLine();
            if (userInput == "1" || userInput == "0") { break; }
        }
        return userInput;
    }
    private string ReturnValidGate()
    {
        string userInput;
        while (true)
        {
            Console.WriteLine("\nLogic gates available :  OR  AND  XOR  NAND  NOR");
            Console.Write($"\nEnter logic gate numbered {gateNum} : ");
            userInput = Console.ReadLine().ToUpper();
            if (userInput == "OR" || userInput == "AND" || userInput == "XOR" || userInput == "NAND" || userInput == "NOR") { break; }
            WriteLineColored("Error, gate entered doesn't exist... Try again.", ConsoleColor.Red);
        }
        return userInput;
    }
    private void WriteLineColored(string output, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(output);
        Console.ResetColor();
    }
}

class MainClass
{
    public static List<Gate> allGates;
    public static string result;

    public static int ReturnNumGates()
    {
        int numGates;
        while (true)
        {
            Console.Write("\nEnter the number of gates in the circuit : ");
            string numGatesStr = Console.ReadLine();
            if (!int.TryParse(numGatesStr, out numGates)) { WriteLineColored("Error, input is not a whole number... Try again.", ConsoleColor.Red); continue; }
            if (numGates < 1) { WriteLineColored("Error, can't have less than 1 gate in a circuit.. Try again.", ConsoleColor.Red); }
            else if (numGates > 10) { WriteLineColored("Error, can't have more than 10 gate in a circuit.. Try again.", ConsoleColor.Red); }
            else { return numGates; }
        }
    }
    public static void UpdateCircuit(int numGates)
    {
        int inputInt;
        string inputStr;
        foreach (Gate gate in allGates)
        {
            for (int count = 0; count < 2; count++)
            {
                while (true)
                {
                    Console.Write($"\nEnter the input{count} for gate numbered {gate.gateNum} ({allGates[gate.gateNum].gateType} gate)" +
                    $"\n Enter : m (manualy input)" +
                    $"\n Enter : a gate number between 0 and {numGates - 1}, not including {gate.gateNum}" +
                    $"\n Entered : ");
                    inputStr = Console.ReadLine();

                    if (inputStr.ToLower() == "m") { gate.input[count] = "m"; break; }
                    else if (int.TryParse(inputStr, out inputInt))
                    {
                        if (inputInt > -1 && inputInt != gate.gateNum && inputInt < numGates) { gate.input[count] = inputStr; break; }
                        WriteLineColored("Gate number entered is not in the valid range... Try again.", ConsoleColor.Red); 
                    }
                    else { WriteLineColored("Error, invalid input entered... Try again.", ConsoleColor.Red); }
               
                }
            }
        }
    }
    static void RunCircuit(string finalGate, int numGates)
    {
        UpdateCircuit(numGates);
        try { RunGate(finalGate, finalGate, 0); }
        catch (StackOverflowException) { }
    }
    static string RunGate(string finalGate, string currentGate, int runCount)
    {
        string output = null;
        string[] input = new string[2];
        runCount += 1;
        if (runCount > 800) { throw new StackOverflowException(); }
        foreach (Gate gate in allGates)
        {
            if (gate.gateNum.ToString() == currentGate)
            {
                if (gate.input[0] != "m")
                { input[0] = RunGate(finalGate, gate.input[0], runCount); }
                if (gate.input[1] != "m")
                { input[1] = RunGate(finalGate, gate.input[1], runCount); }
                for (int count = 0; count < 2; count++)
                {
                    if (input[count] == null) { continue; }
                    gate.input[count] = input[count];
                }
                output = gate.Run();
                if (currentGate == finalGate) { result = output; }
                break;
            }
        }
        return output;
    }
    static string ReturnFinalGate(int numGates)
    {
        string finalGate;
        int finalGateInt;
        while (true)
        {
            Console.Write($"Enter the final gate number, between 0 and {numGates-1} : ");
            finalGate = Console.ReadLine();
            if (int.TryParse(finalGate, out finalGateInt)) { if (finalGateInt > -1 && finalGateInt < numGates) { return finalGate; } }
            else { Console.WriteLine("Error, invalid input entered... Try again."); }
        }
    }
    static void WriteLineColored(string output, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(output);
        Console.ResetColor();
    }

    public static void Main()
    {
        int numGates;
        result = null;
        string finalGate;
        while (true)
        {
            allGates = new List<Gate>();
            numGates = ReturnNumGates();
            if (numGates == 1)
            {
                allGates.Add(new Gate(0));
                allGates[0].input[0] = "m"; allGates[0].input[1] = "m";
                WriteLineColored($"Result = {allGates[0].Run()}", ConsoleColor.Green);
            }
            else
            {
                for (int gateCount = 0; gateCount < numGates; gateCount++) { allGates.Add(new Gate(gateCount)); }
                finalGate = ReturnFinalGate(numGates);
                RunCircuit(finalGate, numGates);
                if (result == null) { WriteLineColored("Circuit entered is invalid...", ConsoleColor.Red); }
                else { WriteLineColored($"Result = {result}", ConsoleColor.Green); }
            }
            
            Console.WriteLine("\nPress enter to continue...");
            Console.ReadLine();
            Console.Clear();
        }
    }
}
