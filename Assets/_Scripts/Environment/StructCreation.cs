using System.Collections.Generic;
using System;
using UnityEngine;
using Random = System.Random;

public enum RoomType
{
    empty,
    common,
    EndRoom,
    CraftingRoom,
    Shop,
    KeyRoom
}

public class RoomNode
{
    public int Id { get; set; } //방의 위치를 1차원으로 표시할때의 인덱스
    public RoomType RoomType { get; set; } //방의 타입
    public RoomNode ParentNode { get; set; } //상위 노드
    public List<int> Children { get; set; } //직접 연결된 하위 노드
    public GameObject RoomObject { get; set; }

    public RoomNode(int id, RoomNode parentNode = null)
    {
        Id = id;
        ParentNode = parentNode;
        Children = new List<int>();
    }
}


public class StructCreation
{
    int iFirstRoom = 45;
    int iMaxRoom = 5;
    int iCreateRoomCount = 0;
    Random rand;

    Queue<RoomNode> qRoomIdx = new Queue<RoomNode>();

    Dictionary<int, RoomNode> roomGraph = new Dictionary<int, RoomNode>();

    LinkedList<RoomNode> qEndRoom = new LinkedList<RoomNode>();

    public bool Run(int maxRoom, ref Dictionary<int, RoomNode> destGraph, int seed, int seedAdjust)
    {
        rand = new Random(seed + (seedAdjust * 185303) % 16777216); //시드 실패시 조정하는값 16777216은 6자리 16진수의 최대값
        iMaxRoom = maxRoom;

        Check(roomGraph, iFirstRoom);

        while (qRoomIdx.Count > 0)
        {
            RoomNode currentRoom = qRoomIdx.Dequeue();
            bool bCreated = false;
            int iRoom = currentRoom.Id;
            int iXPos = iRoom % 10;

            if (iXPos > 0) bCreated = bCreated || Check(roomGraph, iRoom - 1, currentRoom);
            if (iXPos < 8) bCreated = bCreated || Check(roomGraph, iRoom + 1, currentRoom);
            if (iRoom > 9) bCreated = bCreated | Check(roomGraph, iRoom - 10, currentRoom);
            if (iRoom < 90) bCreated = bCreated | Check(roomGraph, iRoom + 10, currentRoom);

            if (!bCreated)
            {
                qEndRoom.AddLast(currentRoom); //모든 조건을 검사해봤지만 방이 생기지 않았으면 현재방을 엔드룸으로 설정
                currentRoom.RoomType = RoomType.EndRoom;

            }
        }

        if (iCreateRoomCount != iMaxRoom)
        {
            return false; //방 개수가 설정보다 적으면 안됨
        }
        if (qEndRoom.Count < 4) {
            return false; //엔드룸이 너무 적으면 안됨
        }

        PlaceSpecialRoom();

        destGraph = roomGraph;
        return true;
    }

    bool Check(Dictionary<int, RoomNode> graph, int i, RoomNode parent = null)
    {
        if (graph.ContainsKey(i)) return false; //이미 방이 있으면 포기

        if (NeighborCount(graph, i) >= 2) return false; //주변에 방이 2개 이상 만들어져 있으면 포기

        if (iCreateRoomCount >= iMaxRoom) return false; //최대 개수에 도달하였으면 포기

        if (rand.Next(2) == 1 && i != iFirstRoom) return false; //50%확률로 포기

        //위 모든 조건을 통과했다면 방이 만들어진 것으로 그래프에 추가해준다.
        RoomNode newNode = new RoomNode(i, parent); //자신의 인덱스와 부모 정보를 넣어주고
        graph[i] = newNode; //방 그래프에 할당
        qRoomIdx.Enqueue(newNode);

        newNode.RoomType = RoomType.common; //우선 기본타입 방으로 설정

        if (parent != null) //시작 노드가 아닌경우
        {
            parent.Children.Add(newNode.Id); //부모 노드에 자식으로 추가한다.
        }

        if (i == iFirstRoom) graph[i] = newNode; //만약 시작 노드인경우 45번 칸에 해당 노드를 넣는다.
        iCreateRoomCount++; //방하나 추가
        return true; //방이 만들어졌으로 true 반환
    }

    int NeighborCount(Dictionary<int, RoomNode> graph, int i)
    {
        int iXPos = i % 10;
        int n = 0;
        if (iXPos > 0) n += graph.ContainsKey(i - 1) ? 1 : 0;
        if (iXPos < 8) n += graph.ContainsKey(i + 1) ? 1 : 0;
        if (i > 9) n += graph.ContainsKey(i - 10) ? 1 : 0;
        if (i < 90) n += graph.ContainsKey(i + 10) ? 1 : 0;
        return n;
    }

    void PlaceSpecialRoom()
    {
        // 엔드룸은 큐에 저장되어 있는데, 알고리즘상 엔드룸 중 가장 멀리 있는 것이 제일 마지막에 위치한다.
        // 그 방을 층을 내려가는데 필요한 아이템이 나오는 방으로 배치한다.
        qEndRoom.Last.Value.RoomType = RoomType.KeyRoom;
        qEndRoom.RemoveLast();

        qEndRoom.RemoveFirst(); //너무 가까운곳 하나 버리고

        qEndRoom.First.Value.RoomType = RoomType.CraftingRoom; //다음 가까운곳은 제작실
        qEndRoom.RemoveFirst();

        qEndRoom.First.Value.RoomType = RoomType.Shop; //그 다음은 상점
        qEndRoom.RemoveFirst();
        // 필요한 경우, specialRoom.Id 값을 사용하여 해당 방에 특수 아이템 등을 할당할 수 있습니다.
    }
}
