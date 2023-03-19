using System.Collections.Generic;
using System;
using System.Diagnostics;

public class StructCreation
{
    int iFirstRoom = 45;
    int iMaxRoom = 5;
    int iCreateRoomCount = 0;
    Random rand = new Random();

    //다음으로 탐색할 방들의 인덱스를 저장할 큐
    Queue<int> qRoomIdx = new Queue<int>();

    int[] iaMap = new int[100];

    Stack<int> qEndRoom = new Stack<int>();

    public bool Run(int maxRoom, ref int[] destArray)
    {
        //방 크기 초기화
        iMaxRoom = maxRoom;

        Check(iaMap, iFirstRoom); //초기 위치 (중앙)

        //큐에 방이 하나도 남지 않을때까지 반복
        while (qRoomIdx.Count > 0)
        {
            int iRoom = qRoomIdx.Dequeue();
            bool bCreated = false;
            int iXPos = iRoom % 10; //열 인덱스

            //각각 붙어있는 "bCreated ||" 는 나중에 방문한곳에서 방을 만들지 못했더라도 이전에 만든 값에 영향을 주지 않도록 있음
            //현재 셀이 가장 왼쪽에 붙어있지 않다면 왼쪽으로 이동한다.

            if (iXPos > 0) bCreated = bCreated || Check(iaMap, iRoom - 1);

            //현재 셀이 가장 오른쪽에 붙어있지 않다면 오른쪽으로 이동한다.
            if (iXPos < 8) bCreated = bCreated || Check(iaMap, iRoom + 1);

            //현재 셀이 가장 위쪽에 붙어있지 않다면 위쪽으로 이동한다.
            if (iRoom > 9) bCreated = bCreated | Check(iaMap, iRoom - 10);

            //현재 셀이 가장 아래쪽에 붙어있지 않다면 아래쪽으로 이동한다.
            if (iRoom < 90) bCreated = bCreated | Check(iaMap, iRoom + 10);

            //결국 아무곳에도 방을 만들지 못하는 위치라면 엔드룸 큐에 추가한다.
            if (!bCreated) qEndRoom.Push(iRoom);
        }

        if (iCreateRoomCount != iMaxRoom) return false;

        //조건 불만족일때 특수방 배치하면 빈 스택 오류 발생하므로
        //조건 확인후 배치
        PlaceSpecialRoom(iaMap);

        int ttmp = qEndRoom.Count;
        for (int i = 0; i < ttmp; i++)
        {
            int tmp = qEndRoom.Pop();
            Console.Write(tmp + " - ");
            iaMap[tmp] = 2;
        }


        destArray = iaMap;
        return true;
    }

    bool Check(int[] map, int i)
    {
        //이미 생성된 방이 있으면 포기
        if (map[i] != 0) return false;

        //이미 주변에 방이 2개 이상 만들어 진 경우 포기
        if (NeighborCount(map, i) >= 2) return false;

        //방의 개수가 꽉 찬 경우 포기
        if (iCreateRoomCount >= iMaxRoom) return false;
        // //무작위성을 위해 50%확률로 포기
        if (rand.Next(2) == 1 && i != iFirstRoom)
        return false;
        //모든 조건에 통과했다면 해당 위치를 큐에 넣어준다.
        qRoomIdx.Enqueue(i);

        //해당 칸은 방이 생겼으므로 1으로 만들고 방 개수를 +1 해준다.
        map[i] = 1;

        if (i == iFirstRoom) map[i] = 1;
        iCreateRoomCount++;
        return true;
    }

    int NeighborCount(int[] map, int i)
    {   
        int iXPos = i % 10;
        int n = 0;
        if (iXPos > 0) n += map[i - 1];
        if (iXPos < 8) n += map[i + 1];
        if (i > 9) n += map[i - 10];
        if (i < 80) n += map[i + 10];
        return n;
    }

    void PlaceSpecialRoom(int[] destArray)
    {
        //엔드룸은 큐에 저장되어 있는데, 알고리즘상 엔드룸중 가장 멀리 있는 것이 제일 마지막에 위치한다.
        //그 방을 층을 내려가는데 필요한 아이템이 나오는 방으로 배치한다.
        destArray[qEndRoom.Pop()] = 9;
    }

}
