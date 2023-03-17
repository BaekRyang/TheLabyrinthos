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

    public bool Run(int p_MaxRoom, ref int[] p_DestArray)
    {
        //�� ũ�� �ʱ�ȭ
        iMaxRoom = p_MaxRoom;

        Check(p_DestArray, iFirstRoom); //�ʱ� ��ġ (�߾�)

        //ť�� ���� �ϳ��� ���� ���������� �ݺ�
        while (qRoomIdx.Count > 0)
        {
            int iRoom = qRoomIdx.Dequeue();
            Console.WriteLine("���� �˻� : " + iRoom);
            bool bCreated = false;
            int iXPos = iRoom % 10; //�� �ε���

            //���� �پ��ִ� "bCreated ||" �� ���߿� �湮�Ѱ����� ���� ������ ���ߴ��� ������ ���� ���� ������ ���� �ʵ��� ����
            //���� ���� ���� ���ʿ� �پ����� �ʴٸ� �������� �̵��Ѵ�.

            if (iXPos > 0) bCreated = bCreated || Check(p_DestArray, iRoom - 1);

            //���� ���� ���� �����ʿ� �پ����� �ʴٸ� ���������� �̵��Ѵ�.
            if (iXPos < 8) bCreated = bCreated || Check(p_DestArray, iRoom + 1);

            //���� ���� ���� ���ʿ� �پ����� �ʴٸ� �������� �̵��Ѵ�.
            if (iRoom > 9) bCreated = bCreated | Check(p_DestArray, iRoom - 10);

            //���� ���� ���� �Ʒ��ʿ� �پ����� �ʴٸ� �Ʒ������� �̵��Ѵ�.
            if (iRoom < 90) bCreated = bCreated | Check(p_DestArray, iRoom + 10);

            //�ᱹ �ƹ������� ���� ������ ���ϴ� ��ġ��� ����� ť�� �߰��Ѵ�.
            if (!bCreated)
            {
                qEndRoom.Push(iRoom);
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("����� : " + iRoom);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            }


        }

        if (iCreateRoomCount != iMaxRoom)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("���� �Ҹ��� : DISCARD");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            return false;
        }

        //���� �Ҹ����϶� Ư���� ��ġ�ϸ� �� ���� ���� �߻��ϹǷ�
        //���� Ȯ���� ��ġ
        PlaceSpecialRoom(p_DestArray);


        Console.WriteLine("����� ���� : " + qEndRoom.Count);
        int ttmp = qEndRoom.Count;
        for (int i = 0; i < ttmp; i++)
        {
            int tmp = qEndRoom.Pop();
            Console.Write(tmp + " - ");
            p_DestArray[tmp] = 2;
        }


        Console.WriteLine("������ �� �� : " + iCreateRoomCount);

        p_DestArray = p_DestArray;
        return true;
    }

    bool Check(int[] map, int i)
    {
        //�̹� ������ ���� ������ ����
        if (map[i] != 0)
        {
            Console.WriteLine("�̹� ����");
            return false;
        }

        //�̹� �ֺ��� ���� 2�� �̻� ����� �� ��� ����
        if (NeighborCount(map, i) >= 2)
        {
            Console.WriteLine("2�� �̻�");
            return false;
        }

        //���� ������ �� �� ��� ����
        if (iCreateRoomCount >= iMaxRoom)
        {
            Console.WriteLine("����");
            return false;
        }
        // //���������� ���� 50%Ȯ���� ����
        if (rand.Next(2) == 1 && i != iFirstRoom)
        {
            Console.WriteLine("50%");
            return false;
        }
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
        Console.Write("Ž�� : " + i + " - ");
        int iXPos = i % 10;
        int n = 0;
        if (iXPos > 0) n += map[i - 1];
        if (iXPos < 8) n += map[i + 1];
        if (i > 9) n += map[i - 10];
        if (i < 80) n += map[i + 10];
        Console.Write("R:" + n + " ");
        return n;
    }

    void PlaceSpecialRoom(int[] p_DestArray)
    {
        //������� ť�� ����Ǿ� �ִµ�, �˰���� ������� ���� �ָ� �ִ� ���� ���� �������� ��ġ�Ѵ�.
        //�� ���� ���� �������µ� �ʿ��� �������� ������ ������ ��ġ�Ѵ�.
        p_DestArray[qEndRoom.Pop()] = 9;
    }

    void Print(int[] map)
    {
        for (int i = 0; i < map.Length; i++)
        {
            // if (i % 10 == 0 || i > 89) Console.Write("��");
            // else 
            {
                switch (map[i])
                {
                    case 0:
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(" X ");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;

                    case 1:
                        if (i == iFirstRoom)
                        {
                            Console.BackgroundColor = ConsoleColor.Green;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(" S ");

                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(" O ");
                        }
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;

                    case 2:
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" E ");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;

                    case 9:
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" K ");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    default:
                        Console.Write(" �� ");
                        break;
                }
            }

            if (i % 10 == 9)
            {
                switch (i / 10)
                {
                    case 1:
                        Console.Write("  ");
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(" O ");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" : �Ϲ� ��");
                        break;

                    case 2:
                        Console.Write("  ");
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" E ");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" : �����(Ư���� ���� ����)");
                        break;

                    case 3:
                        Console.Write("  ");
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" K ");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" : Key ��");
                        break;

                    case 4:
                        Console.Write("  ");
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" S ");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" : ���� ��");
                        break;
                }
                Console.WriteLine();
            }
        }
    }
}
