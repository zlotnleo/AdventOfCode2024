var diskMap = File.ReadAllText("input.txt").Select(len => len - '0').ToArray();

Console.WriteLine(GetChecksum1());
Console.WriteLine(GetChecksum2());

return;

long GetChecksum1()
{
    var checksum = 0L;
    var position = 0;

    var movedFileIndex = diskMap.Length - 1;
    var blocksRemainingInMovedFile = diskMap[movedFileIndex];

    for (var mapIndex = 0; mapIndex < movedFileIndex; mapIndex++)
    {
        if (mapIndex % 2 == 0)
        {
            var fileId = mapIndex / 2;
            var fileSize = diskMap[mapIndex];
            checksum += GetChecksumIncrement(position, fileId, fileSize);
            position += fileSize;
        }
        else
        {
            var gapToFill = diskMap[mapIndex];
            while (gapToFill > 0)
            {
                if (blocksRemainingInMovedFile == 0)
                {
                    movedFileIndex -= 2;
                    blocksRemainingInMovedFile = diskMap[movedFileIndex];
                }

                var movedSize = Math.Min(gapToFill, blocksRemainingInMovedFile);
                var fileId = movedFileIndex / 2;
                if (movedSize == 0)
                {
                    position += gapToFill;
                    break;
                }

                blocksRemainingInMovedFile -= movedSize;
                checksum += GetChecksumIncrement(position, fileId, movedSize);
                position += movedSize;
                gapToFill -= movedSize;
            }
        }
    }

    var remainingFileId = movedFileIndex / 2;
    checksum += GetChecksumIncrement(position, remainingFileId, blocksRemainingInMovedFile);

    return checksum;
}

long GetChecksum2()
{
    var checksum = 0L;
    var position = 0;

    var settledFileIndices = new HashSet<int>();

    for (var mapIndex = 0; mapIndex < diskMap.Length; mapIndex++)
    {
        if (mapIndex % 2 == 0 && settledFileIndices.Add(mapIndex))
        {
            var fileId = mapIndex / 2;
            var fileSize = diskMap[mapIndex];
            checksum += GetChecksumIncrement(position, fileId, fileSize);
            position += fileSize;
        }
        else
        {
            var gapToFill = diskMap[mapIndex];
            while (gapToFill > 0)
            {
                var movedFileIndex = -1;
                for (var candidateFileIndex = diskMap.Length - 1; candidateFileIndex >= 0; candidateFileIndex -= 2)
                {
                    if (diskMap[candidateFileIndex] <= gapToFill && !settledFileIndices.Contains(candidateFileIndex))
                    {
                        movedFileIndex = candidateFileIndex;
                        break;
                    }
                }

                if (movedFileIndex == -1)
                {
                    position += gapToFill;
                    break;
                }

                settledFileIndices.Add(movedFileIndex);
                var movedSize = diskMap[movedFileIndex];
                var fileId = movedFileIndex / 2;

                checksum += GetChecksumIncrement(position, fileId, movedSize);
                position += movedSize;
                gapToFill -= movedSize;
            }
        }
    }

    return checksum;
}

long GetChecksumIncrement(int startingPosition, int fileId, int size) =>
    (long)fileId * size * (2 * startingPosition + size - 1) / 2;
