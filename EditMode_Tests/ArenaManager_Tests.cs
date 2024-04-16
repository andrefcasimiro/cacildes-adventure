using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using AF.Arena;

public class EnemySpawnerTests
{

    ArenaManager arenaManager;

    [SetUp]
    public void SetUp()
    {
        arenaManager = new GameObject().AddComponent<ArenaManager>();
    }

    [Test]
    public void TestMaxEnemiesForRound1()
    {
        arenaManager.currentRoundIndex = 1;
        int expectedMaxEnemies = 1;

        int actualMaxEnemies = arenaManager.GetMaxEnemiesToSpawnBasedOnCurrentRound();

        Assert.AreEqual(expectedMaxEnemies, actualMaxEnemies);
    }

    [Test]
    public void TestMaxEnemiesForRound3()
    {
        arenaManager.currentRoundIndex = 3;
        int expectedMaxEnemies = 2;

        int actualMaxEnemies = arenaManager.GetMaxEnemiesToSpawnBasedOnCurrentRound();

        Assert.AreEqual(expectedMaxEnemies, actualMaxEnemies);
    }

    [Test]
    public void TestMaxEnemiesForRound5()
    {
        arenaManager.currentRoundIndex = 5;
        int expectedMaxEnemies = 3;

        int actualMaxEnemies = arenaManager.GetMaxEnemiesToSpawnBasedOnCurrentRound();

        Assert.AreEqual(expectedMaxEnemies, actualMaxEnemies);
    }

    [Test]
    public void TestMaxEnemiesForRound6()
    {
        arenaManager.currentRoundIndex = 6;
        int expectedMaxEnemies = 1;

        int actualMaxEnemies = arenaManager.GetMaxEnemiesToSpawnBasedOnCurrentRound();

        Assert.AreEqual(expectedMaxEnemies, actualMaxEnemies);
    }

    [Test]
    public void TestMaxEnemiesForRound7()
    {
        arenaManager.currentRoundIndex = 7;
        int expectedMaxEnemies = 2;

        int actualMaxEnemies = arenaManager.GetMaxEnemiesToSpawnBasedOnCurrentRound();

        Assert.AreEqual(expectedMaxEnemies, actualMaxEnemies);
    }

    [Test]
    public void TestMaxEnemiesForRound10()
    {
        arenaManager.currentRoundIndex = 10;
        int expectedMaxEnemies = 3;

        int actualMaxEnemies = arenaManager.GetMaxEnemiesToSpawnBasedOnCurrentRound();

        Assert.AreEqual(expectedMaxEnemies, actualMaxEnemies);
    }
}
