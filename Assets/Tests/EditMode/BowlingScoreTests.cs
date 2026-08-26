using System;
using NUnit.Framework;

public sealed class BowlingScoreTests
{
    [Test] public void GutterGameScoresZero(){var s=Game(20,0);Assert.AreEqual(0,s.FinalScore);}
    [Test] public void AllOpenFramesScoreNinety(){var s=new BowlingScore();for(int i=0;i<10;i++){s.RecordRoll(4);s.RecordRoll(5);}Assert.AreEqual(90,s.FinalScore);}
    [Test] public void AllSparesScoreOneFifty(){var s=new BowlingScore();for(int i=0;i<21;i++)s.RecordRoll(5);Assert.AreEqual(150,s.FinalScore);}
    [Test] public void PerfectGameScoresThreeHundred(){var s=Game(12,10);Assert.AreEqual(300,s.FinalScore);}
    [Test] public void ConsecutiveStrikesApplyBonuses(){var s=new BowlingScore();s.RecordRoll(10);s.RecordRoll(10);s.RecordRoll(3);s.RecordRoll(4);for(int i=0;i<7;i++){s.RecordRoll(0);s.RecordRoll(0);}Assert.AreEqual(47,s.FinalScore);}
    [Test] public void TenthFrameSpareAwardsBonus(){var s=new BowlingScore();for(int i=0;i<18;i++)s.RecordRoll(0);s.RecordRoll(7);s.RecordRoll(3);s.RecordRoll(8);Assert.AreEqual(18,s.FinalScore);}
    [Test] public void IncompleteFrameHasNoFinalScore(){var s=new BowlingScore();s.RecordRoll(10);Assert.IsFalse(s.IsGameOver);Assert.AreEqual(0,s.FinalScore);}
    [Test] public void InvalidRollIsRejected(){var s=new BowlingScore();s.RecordRoll(8);Assert.Throws<ArgumentOutOfRangeException>(()=>s.RecordRoll(3));}
    static BowlingScore Game(int count,int pins){var s=new BowlingScore();for(int i=0;i<count;i++)s.RecordRoll(pins);return s;}
}
