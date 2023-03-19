using System.Collections.Generic;
using System;
using System.Diagnostics;

public class StructCreation
{
    int iFirstRoom = 45;
    int iMaxRoom = 5;
    int iCreateRoomCount = 0;
    Random rand = new Random();

    //�������� Ž���� ����� �ε����� ������ ť
    Queue<int> qRoomIdx = new Queue<int>();

    int[] iaMap = new int[100];

    Stack<int> qEndRoom = new Stack<int>();

    public bool Run(int maxRoom, ref int[] destArray)
    {
        //�� ũ�� �ʱ�ȭ
        iMaxRoom = maxRoom;

        Check(iaMap, iFirstRoom); //�ʱ� ��ġ (�߾�)

        //ť�� ���� �ϳ��� ���� ���������� �ݺ�
        while (qRoomIdx.Count > 0)
        {
            int iRoom = qRoomIdx.Dequeue();
            bool bCreated = false;
            int iXPos = iRoom % 10; //�� �ε���

            //���� �پ��ִ� "bCreated ||" �� ���߿� �湮�Ѱ����� ���� ������ ���ߴ��� ������ ���� ���� ������ ���� �ʵ��� ����
            //���� ���� ���� ���ʿ� �پ����� �ʴٸ� �������� �̵��Ѵ�.

            if (iXPos > 0) bCreated = bCreated || Check(iaMap, iRoom - 1);

            //���� ���� ���� �����ʿ� �پ����� �ʴٸ� ���������� �̵��Ѵ�.
            if (iXPos < 8) bCreated = bCreated || Check(iaMap, iRoom + 1);

            //���� ���� ���� ���ʿ� �پ����� �ʴٸ� �������� �̵��Ѵ�.
            if (iRoom > 9) bCreated = bCreated | Check(iaMap, iRoom - 10);

            //���� ���� ���� �Ʒ��ʿ� �پ����� �ʴٸ� �Ʒ������� �̵��Ѵ�.
            if (iRoom < 90) bCreated = bCreated | Check(iaMap, iRoom + 10);

            //�ᱹ �ƹ������� ���� ������ ���ϴ� ��ġ��� ����� ť�� �߰��Ѵ�.
            if (!bCreated) qEndRoom.Push(iRoom);
        }

        if (iCreateRoomCount != iMaxRoom) return false;

        //���� �Ҹ����϶� Ư���� ��ġ�ϸ� �� ���� ���� �߻��ϹǷ�
        //���� Ȯ���� ��ġ
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
        //�̹� ������ ���� ������ ����
        if (map[i] != 0) return false;

        //�̹� �ֺ��� ���� 2�� �̻� ����� �� ��� ����
        if (NeighborCount(map, i) >= 2) return false;

        //���� ������ �� �� ��� ����
        if (iCreateRoomCount >= iMaxRoom) return false;
        // //���������� ���� 50%Ȯ���� ����
        if (rand.Next(2) == 1 && i != iFirstRoom)
        return false;
        //��� ���ǿ� ����ߴٸ� �ش� ��ġ�� ť�� �־��ش�.
        qRoomIdx.Enqueue(i);

        //�ش� ĭ�� ���� �������Ƿ� 1���� ����� �� ������ +1 ���ش�.
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
        //������� ť�� ����Ǿ� �ִµ�, �˰���� ������� ���� �ָ� �ִ� ���� ���� �������� ��ġ�Ѵ�.
        //�� ���� ���� �������µ� �ʿ��� �������� ������ ������ ��ġ�Ѵ�.
        destArray[qEndRoom.Pop()] = 9;
    }

}
