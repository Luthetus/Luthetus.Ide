﻿using Luthetus.Common.RazorLib.Partitions.Models;

namespace Luthetus.Common.Tests.Basis.Partitions.Models;

public class PartitionedImmutableListTests
{
    [Fact]
    public void Constructor()
    {
        var partitionSize = 5_000;
        var partitionedImmutableList = new PartitionedImmutableList<char>(partitionSize);
        Assert.Equal(partitionSize, partitionedImmutableList.PartitionSize);
    }

    [Fact]
    public void Count()
    {
        var partitionSize = 5;
        var partitionedImmutableList = new PartitionedImmutableList<char>(partitionSize);

        partitionedImmutableList = partitionedImmutableList.AddRange("abcdefghijklmnopqrstuvwxyz");
        Assert.Equal(26, partitionedImmutableList.Count);
    }

    [Fact]
    public void IndexOperator()
    {
        var partitionSize = 5;
        var partitionedImmutableList = new PartitionedImmutableList<char>(partitionSize);

        // As of writing this, the 'AddRange' method is not implemented.
        // Therefore, many invocations of 'Add' are performed.
        partitionedImmutableList = partitionedImmutableList
            .Add('a').Add('b').Add('c').Add('d').Add('e').Add('f').Add('g').Add('h').Add('i')
            .Add('j').Add('k').Add('l').Add('m').Add('n').Add('o').Add('p').Add('q').Add('r')
            .Add('s').Add('t').Add('u').Add('v').Add('w').Add('x').Add('y').Add('z');

        int i = 0;
        Assert.Equal('a', partitionedImmutableList[i++]);
        Assert.Equal('b', partitionedImmutableList[i++]);
        Assert.Equal('c', partitionedImmutableList[i++]);
        Assert.Equal('d', partitionedImmutableList[i++]);
        Assert.Equal('e', partitionedImmutableList[i++]);
        Assert.Equal('f', partitionedImmutableList[i++]);
        Assert.Equal('g', partitionedImmutableList[i++]);
        Assert.Equal('h', partitionedImmutableList[i++]);
        Assert.Equal('i', partitionedImmutableList[i++]);
        Assert.Equal('j', partitionedImmutableList[i++]);
        Assert.Equal('k', partitionedImmutableList[i++]);
        Assert.Equal('l', partitionedImmutableList[i++]);
        Assert.Equal('m', partitionedImmutableList[i++]);
        Assert.Equal('n', partitionedImmutableList[i++]);
        Assert.Equal('o', partitionedImmutableList[i++]);
        Assert.Equal('p', partitionedImmutableList[i++]);
        Assert.Equal('q', partitionedImmutableList[i++]);
        Assert.Equal('r', partitionedImmutableList[i++]);
        Assert.Equal('s', partitionedImmutableList[i++]);
        Assert.Equal('t', partitionedImmutableList[i++]);
        Assert.Equal('u', partitionedImmutableList[i++]);
        Assert.Equal('v', partitionedImmutableList[i++]);
        Assert.Equal('w', partitionedImmutableList[i++]);
        Assert.Equal('x', partitionedImmutableList[i++]);
        Assert.Equal('y', partitionedImmutableList[i++]);
        Assert.Equal('z', partitionedImmutableList[i++]);
    }

    [Fact]
    public void GetEnumerator()
    {
        var partitionSize = 5;
        var partitionedImmutableList = new PartitionedImmutableList<char>(partitionSize);

        partitionedImmutableList = partitionedImmutableList.Add('a');
        partitionedImmutableList = partitionedImmutableList.Add('b');
        partitionedImmutableList = partitionedImmutableList.Add('c');

        // Testing the enumerator here so this foreach use case feels more like a 'for' loop on purpose
        int count = 0; 
        foreach (var character in partitionedImmutableList)
        {
            if (count == 0)
                Assert.Equal('a', character);
            else if (count == 1)
                Assert.Equal('b', character);
            else if (count == 2)
                Assert.Equal('c', character);
            else
                throw new ApplicationException("The test case is only 3 letters long as this moment. So this block should never run.");

            count++;
        }
    }

    [Fact]
    public void Add()
    {
        var partitionSize = 5;
        var partitionedImmutableList = new PartitionedImmutableList<char>(partitionSize);

        // Each 'Add(...)' invocation should return a new object. So store them all
        // here and then check that they are all separate instances.
        var historyPartitionedImmutableList = new List<PartitionedImmutableList<char>>
        { 
            partitionedImmutableList
        };

        // Invoke 'Add(...)' one more times than what the partitionSize is.
        // where each invocation adds a character.
        // Therefore, the final invocation one can assert that a new partition was
        // made to hold that final character.
        for (int i = 0; i <= partitionSize; i++)
        {
            // 97 to 122 provides lowercase letters (both sides inclusive) (ASCII)
            var lowercaseLettersStartPositionInclusive = 97;
            var lowercaseLettersEndPositionInclusive = 122;
            var offsetModuloOperand = lowercaseLettersEndPositionInclusive - lowercaseLettersStartPositionInclusive;
            var character = (char)(lowercaseLettersStartPositionInclusive + (i % offsetModuloOperand));

            partitionedImmutableList = partitionedImmutableList.Add(character);
            historyPartitionedImmutableList.Add(partitionedImmutableList);
        }

        // Assertions
        {
            for (int i = 0; i < historyPartitionedImmutableList.Count; i++)
            {
                int count = i;
                var partitionedList = historyPartitionedImmutableList[i];

                Assert.Equal(partitionSize, partitionedList.PartitionSize);
                Assert.Equal(count, partitionedList.Count);

                if (count == 0)
                {
                    Assert.Empty(partitionedList.PartitionList);
                    Assert.Empty(partitionedList.PartitionMemoryMap);
                }
                else
                {
                    var expectedPartitionCount = Math.Ceiling((double)count / partitionSize);
                    Assert.Equal(expectedPartitionCount, partitionedList.PartitionList.Count);

                    var countLastPartition = count % partitionSize;

                    if (countLastPartition != 0)
                    {
                        var lastPartition = partitionedList.PartitionList.Last();
                        Assert.Equal(countLastPartition, lastPartition.Count);
                        Assert.Equal(countLastPartition, partitionedList.PartitionMemoryMap[(int)expectedPartitionCount - 1]);
                    }
                }
            }
        }
    }

    [Fact]
    public void AddRange()
    {
        var partitionSize = 5;
        var partitionedImmutableList = new PartitionedImmutableList<char>(partitionSize);

        partitionedImmutableList = partitionedImmutableList.AddRange("abcdefghijklmnopqrstuvwxyz");

        int i = 0;
        Assert.Equal('a', partitionedImmutableList[i++]);
        Assert.Equal('b', partitionedImmutableList[i++]);
        Assert.Equal('c', partitionedImmutableList[i++]);
        Assert.Equal('d', partitionedImmutableList[i++]);
        Assert.Equal('e', partitionedImmutableList[i++]);
        Assert.Equal('f', partitionedImmutableList[i++]);
        Assert.Equal('g', partitionedImmutableList[i++]);
        Assert.Equal('h', partitionedImmutableList[i++]);
        Assert.Equal('i', partitionedImmutableList[i++]);
        Assert.Equal('j', partitionedImmutableList[i++]);
        Assert.Equal('k', partitionedImmutableList[i++]);
        Assert.Equal('l', partitionedImmutableList[i++]);
        Assert.Equal('m', partitionedImmutableList[i++]);
        Assert.Equal('n', partitionedImmutableList[i++]);
        Assert.Equal('o', partitionedImmutableList[i++]);
        Assert.Equal('p', partitionedImmutableList[i++]);
        Assert.Equal('q', partitionedImmutableList[i++]);
        Assert.Equal('r', partitionedImmutableList[i++]);
        Assert.Equal('s', partitionedImmutableList[i++]);
        Assert.Equal('t', partitionedImmutableList[i++]);
        Assert.Equal('u', partitionedImmutableList[i++]);
        Assert.Equal('v', partitionedImmutableList[i++]);
        Assert.Equal('w', partitionedImmutableList[i++]);
        Assert.Equal('x', partitionedImmutableList[i++]);
        Assert.Equal('y', partitionedImmutableList[i++]);
        Assert.Equal('z', partitionedImmutableList[i++]);
    }

    [Fact]
    public void Clear()
    {
        var partitionSize = 5;
        var partitionedImmutableList = new PartitionedImmutableList<char>(partitionSize);

        partitionedImmutableList = partitionedImmutableList.AddRange("abcdefghijklmnopqrstuvwxyz");
        Assert.NotEmpty(partitionedImmutableList);

        partitionedImmutableList = partitionedImmutableList.Clear();
        Assert.Empty(partitionedImmutableList);
    }

    [Fact]
    public void IndexOf()
    {
        var partitionSize = 5;
        var partitionedImmutableList = new PartitionedImmutableList<char>(partitionSize);

        partitionedImmutableList = partitionedImmutableList.AddRange("abcdefghijklmnopqrstuvwxyz");

        Assert.Equal(25, partitionedImmutableList.IndexOf('z'));
        Assert.Equal(5, partitionedImmutableList.IndexOf('f'));

        // -1 signifies the entry was not found.
        Assert.Equal(-1, partitionedImmutableList.IndexOf('3'));
    }

    [Fact]
    public void Insert()
    {
        var partitionSize = 3;
        var partitionedImmutableList = new PartitionedImmutableList<char>(partitionSize);
        partitionedImmutableList = partitionedImmutableList.AddRange("abc");

        // Assertions pre invocation
        {
            Assert.Single(partitionedImmutableList.PartitionList);
            var partitionList = partitionedImmutableList.PartitionList[0];
            Assert.Equal('a', partitionList[0]);
            Assert.Equal('b', partitionList[1]);
            Assert.Equal('c', partitionList[2]);
        }

        partitionedImmutableList = partitionedImmutableList.Insert(0, '5');

        // Assertions post invocation
        {
            Assert.Equal(3, partitionedImmutableList.PartitionList.Count);

            // firstPartitionList
            {
                var partitionIndex = 0;
                var firstPartitionList = partitionedImmutableList.PartitionList[partitionIndex];
                Assert.Equal(2, firstPartitionList.Count);
                Assert.Equal(partitionedImmutableList.PartitionMemoryMap[partitionIndex], firstPartitionList.Count);
                Assert.Equal('5', firstPartitionList[0]);
                Assert.Equal('a', firstPartitionList[1]);
            }

            // secondPartitionList
            {
                var partitionIndex = 1;
                var secondPartitionList = partitionedImmutableList.PartitionList[partitionIndex];
                Assert.Single(secondPartitionList);
                Assert.Equal(partitionedImmutableList.PartitionMemoryMap[partitionIndex], secondPartitionList.Count);
                Assert.Equal('b', secondPartitionList[0]);
            }

            // thirdPartitionList
            {
                var partitionIndex = 2;
                var thirdPartitionList = partitionedImmutableList.PartitionList[partitionIndex];
                Assert.Single(thirdPartitionList);
                Assert.Equal(partitionedImmutableList.PartitionMemoryMap[partitionIndex], thirdPartitionList.Count);
                Assert.Equal('c', thirdPartitionList[0]);
            }
        }
    }

    [Fact]
    public void InsertRange()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Remove()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void RemoveAt()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void RemoveRange()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void ExplicitInterfaceGetEnumerator()
    {
        var partitionSize = 5;
        var partitionedImmutableList = new PartitionedImmutableList<char>(partitionSize);

        partitionedImmutableList = partitionedImmutableList.Add('a');
        partitionedImmutableList = partitionedImmutableList.Add('b');
        partitionedImmutableList = partitionedImmutableList.Add('c');
        
        var enumerable = (IEnumerable<char>)partitionedImmutableList;

        // Testing the enumerator here so this foreach use case feels more like a 'for' loop on purpose
        int count = 0;
        foreach (var character in enumerable)
        {
            if (count == 0)
                Assert.Equal('a', character);
            else if (count == 1)
                Assert.Equal('b', character);
            else if (count == 2)
                Assert.Equal('c', character);
            else
                throw new ApplicationException("The test case is only 3 letters long as this moment. So this block should never run.");

            count++;
        }
    }
}