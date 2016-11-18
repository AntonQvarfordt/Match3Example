using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Jewel : MonoBehaviour
{
    public List<Jewel> Neighbors
    {
        get
        {
            var returnList = new List<Jewel>();
            var origin = new Vector3(transform.position.x, transform.position.y, -5);

            const int radius = 1;

            var neighbors = Physics.SphereCastAll(origin, radius, Vector3.forward);

            for (var i = 0; i < neighbors.Length; i++)
            {
                var hit = neighbors[i];
                if (hit.transform.tag == "Jewel" && hit.transform != transform)
                {
                    returnList.Add(hit.transform.GetComponent<Jewel>());
                }
            }

            return returnList;
        }
    }

    public JewelType GetJewelType
    {
        get { return _jewelType; }
    }

    public GameObject GetCore
    {
        get { return GetCoreFunc(); }
    }

    public JewelSlot InSlot;

    public GameObject GetMeshRenderer { get; private set; }

    public Vector2 ReflectIndex;

    public KeyValuePair<int, int> MatrixIndex;

    public List<Jewel> MarkedBuddies = new List<Jewel>();
    public bool MarkedForDeath;
    public bool InPlay;
    public bool IsLit;

    public AudioSource JewelAudioSource;

    public Color _litEmissionColor;

    public JewelType _jewelType;
    private Rigidbody _rBody;

    private GameObject GetCoreFunc()
    {
        foreach (Transform child in transform)
        {
            if (child.name == "core")
            {
                return child.gameObject;
            }

            foreach (Transform child2 in child)
            {
                if (child2.name == "core")
                {
                    return child2.gameObject;
                }
            }
        }
        return null;
    }

    public void CreateJewelLocal()
    {
        ResetJewel();

        var jewelObject = GameManager.GetGameManager().GetRandomJewel();
        _jewelType = jewelObject.JewelType;
        transform.name = _jewelType.ToString();

        _litEmissionColor = jewelObject.LitEmissionColor;

        GetMeshRenderer = Instantiate(jewelObject.Mesh);
        GetMeshRenderer.isStatic = false;
        GetMeshRenderer.transform.SetParent(transform);
        GetMeshRenderer.transform.localPosition = Vector3.zero;

        gameObject.layer = GameManager.GetLayer(this);
        gameObject.tag = "Jewel";

        GetMeshRenderer.layer = GameManager.GetLayer(this);

        GetMeshRenderer.transform.FindChild("core").gameObject.layer = GameManager.GetLayer(this);
    }

    private void ResetJewel()
    {
        MarkedForDeath = false;

        if (GetMeshRenderer != null)
        {
            DestroyImmediate(GetMeshRenderer);
        }
    }

    public void RevealJewel()
    {
        _rBody.isKinematic = true;
    }

    public List<Jewel> IdentifyMatchesHorizontal()
    {
        var returnList = new List<Jewel>();

        var hasMatches = true;

        Jewel rightSideJewel = null;

        if (MatrixIndex.Value > 0)
            rightSideJewel = this.GetJewelToRight();

        Jewel leftSideJewel = null;

        if (MatrixIndex.Value < (int)GameManager.GetGameManager().MatrixSize.x - 1)
            leftSideJewel = this.GetJewelToLeft();

        if (rightSideJewel != null && leftSideJewel != null)
        {
            if (rightSideJewel.GetJewelType != GetJewelType)
                hasMatches = false;
            if (leftSideJewel.GetJewelType != GetJewelType)
                hasMatches = false;

            if (!hasMatches)
                return returnList;

            returnList.Add(leftSideJewel);
            returnList.Add(this);
            returnList.Add(rightSideJewel);

            rightSideJewel.MarkedForDeath = true;
            leftSideJewel.MarkedForDeath = true;
            MarkedForDeath = true;

            return returnList;
        }

        return returnList;
    }

    public List<Jewel> IdentifyMatchesVertical()
    {
        var returnList = new List<Jewel>();

        var hasMatches = true;

        Jewel aboveJewel = null;
        if (MatrixIndex.Key < (int)GameManager.GetGameManager().MatrixSize.y - 1)
            aboveJewel = this.GetJewelAbove();

        Jewel belowJewel = null;
        if (MatrixIndex.Key > 0)
            belowJewel = this.GetJewelBelow();

        if (aboveJewel != null && belowJewel != null)
        {
            if (aboveJewel.GetJewelType != GetJewelType)
                hasMatches = false;
            if (belowJewel.GetJewelType != GetJewelType)
                hasMatches = false;

            if (!hasMatches)
                return returnList;

            returnList.Add(aboveJewel);
            returnList.Add(this);
            returnList.Add(belowJewel);

            aboveJewel.MarkedForDeath = true;
            belowJewel.MarkedForDeath = true;
            MarkedForDeath = true;

            return returnList;
        }

        return returnList;
    }

    public void AddRigidbody(int y)
    {
        gameObject.AddComponent<BoxCollider>();

        _rBody = gameObject.AddComponent<Rigidbody>();
        _rBody.constraints = RigidbodyConstraints.FreezeAll;
        _rBody.mass = 0.1f;
    }

    public void LightOn(bool slow = false)
    {
        if (IsLit)
            return;

        var lightAnimator = GetMeshRenderer.AddComponent<AnimateMaterialEmission>();
        lightAnimator.AnimateLight(_litEmissionColor, 15, GetCore.GetComponent<Renderer>());

        IsLit = true;

        lightAnimator.transform.DOShakePosition(0.15f, Vector3.one * 0.25f, 100);

        CancelInvoke();
    }

    public void LightOff(bool slow = false)
    {
        if (!IsLit)
            return;

        var speed = 4;

        if (slow)
            speed = 2;

        var lightAnimator = GetMeshRenderer.AddComponent<AnimateMaterialEmission>();
        lightAnimator.AnimateLight(Color.black, speed, GetCore.GetComponent<Renderer>());
        IsLit = false;
    }

    public void OnPlacesChanged(Jewel changedWith)
    {

    }

    public void Select(InputInstance inputInstance)
    {
        LightOn();
    }

    public void DestroySelf(float animationSpeed)
    {
        transform.DOScale(Vector3.one * 1.1f, 0.5f).OnComplete(Implode);
        transform.DOShakePosition(0.75f, 0.05f, 20, 90f, false, false).SetEase(Ease.InExpo).OnComplete(DestroyAnimationCompleted);

        var lightAnimator = GetMeshRenderer.AddComponent<AnimateMaterialEmission>();
        lightAnimator.AnimateLight(Color.white, 1, GetCore.GetComponent<Renderer>());
    }

    private void Implode()
    {
        transform.DOScale(transform.localScale*1.2f, 0.15f).SetEase(Ease.InQuad).OnComplete(() => transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.OutQuad));
        var exitAnimator = GetMeshRenderer.AddComponent<DestroyJewelAnimation>();
        exitAnimator.StartDestroyAnimation(Color.white, 0.2f, GetMeshRenderer.GetComponent<Renderer>());
    }

    private void DestroyAnimationCompleted()
    {
        transform.DOKill();
        GetCore.GetComponent<MeshRenderer>().material.DOKill();
        GetMeshRenderer.GetComponent<MeshRenderer>()
            .material.DOKill();
        InSlot.SetJewel = null;
        Destroy(gameObject);
    }
}