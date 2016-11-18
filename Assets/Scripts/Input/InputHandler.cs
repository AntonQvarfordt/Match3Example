using DG.Tweening;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public InputInstance PreviousInput;
    public InputInstance ActiveInput;

    public InputState CurrentState = InputState.Idle;

    private bool _inputDown;
    //private bool _inputUp;
    private bool _inputDrag;
    private bool _allowClick = true;

    public Vector3 LastInputPosition;
    public Vector3 InputDragDelta;
    public float DeltaMagnitude;

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.GetGameManager();
    }

    private void Update()
    {
        if (_gameManager.State != GameStates.Idle)
            return;

        if (!_allowClick)
            return;

        if (Input.GetButtonDown("Fire1"))
        {
            OnMouseDown();
            CurrentState = InputState.Holding;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            OnMouseUp();
            CurrentState = InputState.Idle;
        }

        if (!_inputDown) return;

        if (!_inputDrag)
        {
            CurrentState = InputState.Holding;
        }
        else
        {
            Drag();
        }
    }

    public void SetActiveInput(Jewel jewel)
    {
        ActiveInput = new InputInstance(jewel);
    }

    private void FixedUpdate()
    {
        if (_inputDown)
        {
            CalculateDragDelta();
        }
    }

    private void Drag()
    {
        if (ActiveInput != null)
        {
            ActiveInput.InputDrag.DragUpdate(RaycastJewels(), ActiveInput);
        }
    }

    private void CalculateDragDelta()
    {
        InputDragDelta = Input.mousePosition - LastInputPosition;
        DeltaMagnitude = InputDragDelta.magnitude;

        if (InputDragDelta.magnitude > 0)
        {
            if (!_inputDrag)
            {
                ActiveInput.CreateDrag();
                _inputDrag = true;
            }

            CurrentState = InputState.Dragging;
        }
        else if (_inputDrag)
        {
            _inputDrag = false;
        }

        LastInputPosition = Input.mousePosition;
    }

    private void OnMouseDown()
    {
        var hit = RaycastJewels();

        if (hit.collider == null)
            return;

        //_inputUp = false;
        _inputDown = true;
        LastInputPosition = Input.mousePosition;
        InputDragDelta = Input.mousePosition;

        if (hit.collider.tag == "Jewel")
        {
            DownOnJewel(hit.transform.GetComponent<Jewel>());
        }
    }

    private void DownOnJewel(Jewel jewel)
    {
        if (ActiveInput == null)
            SetActiveInput(jewel);

        jewel.Select(ActiveInput);
    }

    private void OnMouseUp()
    {
        ResetInput();


        var hit = RaycastJewels();

        if (hit.collider == null)
            return;

        ActiveInput.InputUp = new InputUp(hit.transform.gameObject);

        if (hit.collider.tag == "Jewel")
        {
            UpOnJewel(hit.transform.GetComponent<Jewel>());
        }

        ExitInput();
    }

    private void UpOnJewel(Jewel jewel)
    {
        jewel.LightOff();

        if (ShouldChangePlaces(ActiveInput))
        {
            var fromJewel = ActiveInput.JewelsDraggedOver[0];
            var toJewel = ActiveInput.JewelsDraggedOver[1];
            ChangePlaces(fromJewel, toJewel, 0.5f);
        }
    }

    private void InitInput(InputInstance newInstance)
    {
    }

    private static RaycastHit RaycastJewels()
    {
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(ray, out hit);

        return hit;
    }

    private void ExitInput()
    {
        PreviousInput = ActiveInput;
        if (ActiveInput.JewelsDraggedOver.Count > 0)
        {
            ActiveInput.UnlightDraggedOverJewels();
        }
        ActiveInput = null;
    }



    private void ResetInput()
    {
        if (ActiveInput == null)
            return;

        InputDragDelta = Input.mousePosition;
        _inputDrag = false;
        //_inputUp = true;
        _inputDown = false;
    }

    private static bool ShouldChangePlaces(InputInstance input)
    {
        if (input.JewelsDraggedOver.Count != 2)
            return false;

        var neighbors = input.JewelsDraggedOver[0].Neighbors;
        var isNeighbor = neighbors.Contains(input.JewelsDraggedOver[1]);

        return isNeighbor;
    }

    private void ChangePlaces(Jewel jewelA, Jewel jewelB, float speed)
    {
        _allowClick = false;

        var jewelASlot = jewelA.InSlot;
        var jewelBSlot = jewelB.InSlot;

        jewelASlot.SetJewelInSlot(jewelB, true);
        jewelBSlot.SetJewelInSlot(jewelA, true);

        var aPos = jewelA.transform.position;
        var bPos = jewelB.transform.position;

        jewelA.transform.DOScale(1.1f, speed * 0.5f).SetLoops(2, LoopType.Yoyo);
        jewelA.GetMeshRenderer.transform.DOMoveZ(-1, speed * 0.5f).SetLoops(2, LoopType.Yoyo);
        jewelA.transform.DOMove(bPos, speed);

        jewelB.transform.DOScale(1.1f, speed * 0.5f).SetLoops(2, LoopType.Yoyo);
        jewelB.GetMeshRenderer.transform.DOMoveZ(1, speed * 0.5f).SetLoops(2, LoopType.Yoyo);
        jewelB.transform.DOMove(aPos, speed).OnComplete(() =>
            ChangePlacesAnimationCompleted(jewelA, jewelB, speed)
        );
    }

    private void ChangePlacesAnimationCompleted(Jewel jewelA, Jewel jewelB, float speed)
    {
        var gameManager = GameManager.GetGameManager();
        var markedJewels = gameManager.MarkForDeath();

        if (markedJewels.Count > 2)
        {
            gameManager.SetActiveState(GameStates.Destroying);
            _allowClick = true;
        }
        else
        {
            ChangeBack(jewelA, jewelB, speed);
        }
    }

    private void ChangeBack(Jewel jewelA, Jewel jewelB, float speed)
    {
        var jewelASlot = jewelA.InSlot;
        var jewelBSlot = jewelB.InSlot;

        jewelASlot.SetJewelInSlot(jewelB, true);
        jewelBSlot.SetJewelInSlot(jewelA, true);

        var aPos = jewelA.transform.position;
        var bPos = jewelB.transform.position;

        jewelA.transform.DOScale(1.1f, speed * 0.5f).SetLoops(2, LoopType.Yoyo);
        jewelA.GetMeshRenderer.transform.DOMoveZ(-1, speed * 0.5f).SetLoops(2, LoopType.Yoyo);
        jewelA.transform.DOMove(bPos, speed);

        jewelB.transform.DOScale(1.1f, speed * 0.5f).SetLoops(2, LoopType.Yoyo);
        jewelB.GetMeshRenderer.transform.DOMoveZ(1, speed * 0.5f).SetLoops(2, LoopType.Yoyo);
        jewelB.transform.DOMove(aPos, speed).OnComplete(() =>
        {
            _allowClick = true;
        });
    }
}