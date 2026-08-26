using System;
using System.Collections.Generic;

public sealed class BowlingScore
{
    private readonly List<int> rolls = new();
    public IReadOnlyList<int> Rolls => rolls;
    public bool IsGameOver { get; private set; }
    public int CurrentFrame { get; private set; } = 1;
    public int CurrentRoll { get; private set; } = 1;
    public int FinalScore => IsGameOver ? GetCumulativeTotals()[9] ?? 0 : 0;

    public void Reset() { rolls.Clear(); IsGameOver = false; CurrentFrame = 1; CurrentRoll = 1; }

    public void RecordRoll(int pins)
    {
        if (IsGameOver) throw new InvalidOperationException("The game is already complete.");
        if (pins < 0 || pins > MaxPinsForNextRoll()) throw new ArgumentOutOfRangeException(nameof(pins));
        rolls.Add(pins);
        RecalculatePosition();
    }

    public int MaxPinsForNextRoll()
    {
        if (IsGameOver) return 0;
        int index = 0;
        for (int frame = 1; frame <= 9; frame++)
        {
            if (index >= rolls.Count) return 10;
            if (rolls[index] == 10) { index++; continue; }
            if (index + 1 >= rolls.Count) return 10 - rolls[index];
            index += 2;
        }
        int count = rolls.Count - index;
        if (count == 0) return 10;
        int first = rolls[index];
        if (count == 1) return first == 10 ? 10 : 10 - first;
        int second = rolls[index + 1];
        if (first == 10) return second == 10 ? 10 : 10 - second;
        return first + second == 10 ? 10 : 0;
    }

    public string[] GetFrameMarks()
    {
        string[] marks = new string[10]; int i = 0;
        for (int f = 0; f < 9 && i < rolls.Count; f++)
        {
            if (rolls[i] == 10) { marks[f] = "X"; i++; }
            else if (i + 1 < rolls.Count) { marks[f] = Mark(rolls[i]) + " " + (rolls[i] + rolls[i + 1] == 10 ? "/" : Mark(rolls[i + 1])); i += 2; }
            else { marks[f] = Mark(rolls[i]); i++; }
        }
        if (i < rolls.Count)
        {
            var parts = new List<string>();
            for (int j = i; j < rolls.Count; j++)
            {
                int value = rolls[j];
                bool spare = j > i && value < 10 && rolls[j - 1] + value == 10 && !(j == i + 2 && rolls[i] == 10);
                parts.Add(spare ? "/" : Mark(value));
            }
            marks[9] = string.Join(" ", parts);
        }
        return marks;
    }

    public int?[] GetCumulativeTotals()
    {
        int?[] totals = new int?[10]; int total = 0, i = 0;
        for (int frame = 0; frame < 10; frame++)
        {
            if (i >= rolls.Count) break;
            if (frame == 9)
            {
                if (!IsGameOver) break;
                while (i < rolls.Count) total += rolls[i++];
                totals[frame] = total; break;
            }
            if (rolls[i] == 10)
            {
                if (i + 2 >= rolls.Count) break;
                total += 10 + rolls[i + 1] + rolls[i + 2]; i++;
            }
            else
            {
                if (i + 1 >= rolls.Count) break;
                int pair = rolls[i] + rolls[i + 1];
                if (pair == 10) { if (i + 2 >= rolls.Count) break; total += 10 + rolls[i + 2]; }
                else total += pair;
                i += 2;
            }
            totals[frame] = total;
        }
        return totals;
    }

    private void RecalculatePosition()
    {
        int i = 0;
        for (int frame = 1; frame <= 9; frame++)
        {
            if (i >= rolls.Count) { CurrentFrame = frame; CurrentRoll = 1; return; }
            if (rolls[i] == 10) { i++; continue; }
            if (i + 1 >= rolls.Count) { CurrentFrame = frame; CurrentRoll = 2; return; }
            i += 2;
        }
        int n = rolls.Count - i;
        if (n == 0) { CurrentFrame = 10; CurrentRoll = 1; return; }
        int first = rolls[i];
        if (n == 1) { CurrentFrame = 10; CurrentRoll = 2; return; }
        int second = rolls[i + 1];
        bool bonus = first == 10 || first + second == 10;
        if (bonus && n < 3) { CurrentFrame = 10; CurrentRoll = 3; return; }
        IsGameOver = true; CurrentFrame = 10; CurrentRoll = bonus ? 3 : 2;
    }

    private static string Mark(int pins) => pins == 10 ? "X" : pins == 0 ? "-" : pins.ToString();
}
