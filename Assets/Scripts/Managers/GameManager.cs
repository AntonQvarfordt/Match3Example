using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public List<Jewel> DrawnJewels
    {
        get
        {
            var returnList = new List<Jewel>();
            for (var i = 0; i < JewelSlots.Count; i++)
            {
                for (var j = 0; j < JewelSlots[i].Count; j++)
                {
                    var jewelSlot = JewelSlots[i][j];
                    if (!jewelSlot.IsEmpty)
                        returnList.Add(jewelSlot.GetJewel);
                }
            }
            return returnList;
        }
    }

    public List<JewelSlot> GetAllSlots
    {
        get
        {
            var returnList = new List<JewelSlot>();
            for (var i = 0; i < JewelSlots.Count; i++)
            {
                for (var j = 0; j < JewelSlots[i].Count; j++)
                {
                    var jewelSlot = JewelSlots[i][j];
                    returnList.Add(jewelSlot);
                }
            }
            return returnList;
        }
    }

    public bool HasEmptySlots
    {
        get
        {
            for (var i = 0; i < JewelSlots.Count; i++)
            {
                for (var j = 0; j < JewelSlots[i].Count; j++)
                {
                    var jewelSlot = JewelSlots[i][j];
                    if (jewelSlot.IsEmpty)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public readonly float MoveAnimationTime = 0.25f;

    public readonly bool ShowDebugLogs = false;

    public GameStates State;

    public IGameState GameState;

    public JewelObject Citrine;
    public JewelObject Square;
    public JewelObject Circle;
    public JewelObject Prism;

    public static List<List<JewelSlot>> JewelSlots = new List<List<JewelSlot>>();

    public Vector2 BottomRight;
    public Vector2 MatrixSize;

    public IGameState DestroyingState;
    public IGameState MovingState;
    public IGameState IdleState;

    public bool HasMarkedForDeath(List<Jewel> jewelsList)
    {
        var jewels = jewelsList;

        for (var i = 0; i < jewels.Count; i++)
        {
            var jewel = jewels[i];
            if (jewel.MarkedForDeath)
            {
                return true;
            }
        }
        return false;
    }

    private void Awake()
    {
        DestroyingState = gameObject.AddComponent<DestroyingState>();
        MovingState = gameObject.AddComponent<MovingState>();
        IdleState = gameObject.AddComponent<IdleState>();
    }

    private void Start()
    {
        StartCoroutine(StartDelay());
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(1.5f);
        DrawBoard();
        yield return new WaitForSeconds(1);
        RevealJewels();
        yield return new WaitForSeconds(0.5f);
        SetActiveState(GameStates.Destroying);
    }

    private void ToMovingState()
    {
        SetActiveState(GameStates.Moving);
    }

    public void SetActiveState(GameStates state)
    {
        switch (state)
        {
            case GameStates.Destroying:
                State = GameStates.Destroying;
                GameState = DestroyingState;
                break;
            case GameStates.Moving:
                State = GameStates.Moving;
                GameState = MovingState;
                break;
            case GameStates.Idle:
                State = GameStates.Idle;
                GameState = IdleState;
                break;
            default:
                throw new ArgumentOutOfRangeException("state", state, null);
        }

        if (ShowDebugLogs)
            Debug.Log("States| Setting new active state: " + GameState.ToString());

        GameState.Init();
    }

    public static GameManager GetGameManager()
    {
        return GameObject.Find("/GameManager").GetComponent<GameManager>();
    }

    public void CreateRandomJewel()
    {
        CreateJewel(0, 0);
    }

    public void DrawBoard()
    {
        var board = GameObject.Find("/Board");

        for (var i = 0; i < MatrixSize.y; i++)
        {
            JewelSlots.Add(new List<JewelSlot>());

            var startPosY = -MatrixSize.y * 0.5f;
            var startPosX = -MatrixSize.x * 0.5f;

            for (var j = 0; j < MatrixSize.x; j++)
            {
                JewelSlots[i].Add(null);

                JewelSlots[i][j] = new JewelSlot(new KeyValuePair<int, int>(i, j), new Vector2(startPosX + j + 0.5f, startPosY + i + 0.5f));
                var jewel = CreateJewel(j, i);

                jewel.transform.SetParent(board.transform);
                jewel.transform.DOScale(0, 0.25f).From().SetDelay(0.1f * (j + i));
            }
        }

        var timeoutAfterLoops = 50;
        while (HasMarkedForDeath(MarkForDeath()))
        {
            timeoutAfterLoops--;
            ClearInitMatches();

            if (timeoutAfterLoops < 0)
            {
                break;
            }
        }
    }

    private void ClearInitMatches()
    {
        if (HasMarkedForDeath(MarkForDeath()))
        {
            var markedJewels = GetMarkedJewels();

            for (var i = 0; i < markedJewels.Count; i++)
            {
                var jewel = markedJewels[i];
                jewel.CreateJewelLocal();
            }
        }

        MarkForDeath();
    }

    public void UnmarkAllFromDeath()
    {
        var drawnJewels = DrawnJewels;
        for (var i = 0; i < drawnJewels.Count; i++)
        {
            var drawnJewel = drawnJewels[i];
            drawnJewel.MarkedForDeath = false;
        }
    }

    public List<Jewel> MarkForDeath()
    {
        UnmarkAllFromDeath();

        var returnList = new List<Jewel>();

        for (var i = 0; i < JewelSlots.Count; i++)
        {
            for (var j = 0; j < JewelSlots[i].Count; j++)
            {
                var jewel = JewelSlots[i][j].GetJewel;

                if (jewel == null)
                    continue;

                var foundHorizontalMatches = jewel.IdentifyMatchesHorizontal();
                var foundVerticalMatches = jewel.IdentifyMatchesVertical();

                for (var k = 0; k < foundHorizontalMatches.Count; k++)
                {
                    var hJewel = foundHorizontalMatches[k];
                    if (!returnList.Contains(hJewel))
                        returnList.Add(hJewel);
                }

                for (var l = 0; l < foundVerticalMatches.Count; l++)
                {
                    var vJewel = foundVerticalMatches[l];
                    if (!returnList.Contains(vJewel))
                        returnList.Add(vJewel);
                }
            }
        }

        return returnList;
    }

    public Jewel CreateJewel(int x, int y)
    {
        var go = new GameObject();

        var jewelScript = go.AddComponent<Jewel>();
        jewelScript.MatrixIndex = new KeyValuePair<int, int>(y, x);
        jewelScript.CreateJewelLocal();
        jewelScript.AddRigidbody(y);
        JewelSlots[y][x].SetJewelInSlot(jewelScript);

        go.name = jewelScript.GetJewelType.ToString();
        return jewelScript;
    }

    public static LayerMask GetLayer(Jewel jewel)
    {
        switch (jewel._jewelType)
        {
            case JewelType.Citrine:
                return LayerMask.NameToLayer("JewelRed");
            case JewelType.Square:
                return LayerMask.NameToLayer("JewelBlue");
            case JewelType.Circle:
                return LayerMask.NameToLayer("JewelYellow");
            case JewelType.Prism:
                return LayerMask.NameToLayer("JewelGreen");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public JewelObject GetRandomJewel()
    {
        var jewelMeshes = new JewelObject[4];
        jewelMeshes[0] = Citrine;
        jewelMeshes[1] = Square;
        jewelMeshes[2] = Circle;
        jewelMeshes[3] = Prism;

        return jewelMeshes[Random.Range(0, jewelMeshes.Length)];
    }

    public List<Jewel> GetMarkedJewels()
    {
        var jewels = DrawnJewels;
        var markedJewels = new List<Jewel>();
        for (var i = jewels.Count - 1; i >= 0; i--)
        {
            var jewel = jewels[i];
            if (jewel.MarkedForDeath)
                markedJewels.Add(jewel);
        }

        return markedJewels;
    }

    public void RevealJewels()
    {
        StartCoroutine(RevealJewelsCoroutine());
    }

    private IEnumerator RevealJewelsCoroutine()
    {
        var allJewels = DrawnJewels;

        for (var i = 0; i < allJewels.Count; i++)
        {
            var jewel = allJewels[i];
            if (jewel != null)
                jewel.RevealJewel();
            yield return new WaitForFixedUpdate();
        }
    }

    private void UnmarkAllJewels()
    {
        var allJewels = DrawnJewels;
        for (var i = 0; i < allJewels.Count; i++)
        {
            var jewel = allJewels[i];
            if (jewel != null)
                jewel.MarkedForDeath = false;
        }
    }
}

public enum JewelType
{
    Citrine,
    Square,
    Circle,
    Prism
}

public struct JewelInstance
{
    public JewelType JewelType;
    public GameObject Mesh;
}

[System.Serializable]
public class JewelSlot
{
    public bool IsEmpty
    {
        get { return _jewel == null; }
    }

    public Jewel GetJewel
    {
        get { return _jewel; }

    }

    public Jewel SetJewel
    {
        set { _jewel = value; }
    }

    public KeyValuePair<int, int> GetMatrixIndex
    {
        get { return _matrixIndex; }
    }

    public Vector2 GetSlotPosition { get; private set; }

    public JewelSlot(KeyValuePair<int, int> matrixIndex, Vector2 slotPosition)
    {
        _matrixIndex = matrixIndex;
        GetSlotPosition = slotPosition;
    }

    public void SetJewelInSlot(Jewel jewel, bool animatePos = false, Action completedMoveCallback = null)
    {
        _jewel = jewel;

        if (!animatePos)
            jewel.transform.localPosition = GetSlotPosition;

        jewel.MatrixIndex = _matrixIndex;
        jewel.InSlot = this;
    }

    public bool CheckMove()
    {
        if (_matrixIndex.Key == 0)
        {
            return false;
        }

        var slotBelow = GameManager.JewelSlots[_matrixIndex.Key - 1][_matrixIndex.Value];
        var returnValue = false;
        if (slotBelow.IsEmpty)
        {
            if (!IsEmpty)
            {
                slotBelow.SetJewelInSlot(_jewel, true);
                _jewel.transform.DOMove(slotBelow.GetSlotPosition, GameManager.GetGameManager().MoveAnimationTime);
                _jewel = null;
                returnValue = true;
            }
        }

        if (!IsEmpty || _matrixIndex.Key != GameManager.JewelSlots.Count - 1)
            return returnValue;

        CreateNewJewelOnTop();
        return returnValue;
    }

    private void CreateNewJewelOnTop()
    {
        var jewel = GameManager.GetGameManager().CreateJewel(_matrixIndex.Value, _matrixIndex.Key);
        jewel.transform.SetParent(GameObject.Find("/Board").transform);
        var moveFrom = new Vector2(jewel.InSlot.GetSlotPosition.x, jewel.InSlot.GetSlotPosition.y + 1);
        jewel.transform.DOMove(moveFrom, GameManager.GetGameManager().MoveAnimationTime).From();
    }

    private readonly KeyValuePair<int, int> _matrixIndex;

    private Jewel _jewel;
}
