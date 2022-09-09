using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SkinnedPortalableObject : PortalableObject
{
    [Header("Skinned references")]
    [SerializeField] private SkinnedMeshRenderer[] _skinnedMeshRenders;

    private SkinnedMeshRenderer[] _cloneRenderes;

    protected override void CreateClone()
    {
        _cloneObject = new GameObject(gameObject.name + " (Clone Portal)");
        _cloneObject.transform.position = transform.position;
        _cloneObject.transform.rotation = transform.rotation;
        _cloneObject.SetActive(false);

        _cloneRenderes = new SkinnedMeshRenderer[_skinnedMeshRenders.Length];

        for (int i = 0; i < _skinnedMeshRenders.Length; i++)
        {
            GameObject g = new GameObject(_skinnedMeshRenders[i].gameObject.name);
            g.transform.SetParent(_cloneObject.transform);

            var skinnedMeshRender = g.AddComponent<SkinnedMeshRenderer>();
            _cloneRenderes[i] = skinnedMeshRender;
            
            skinnedMeshRender.sharedMesh = _skinnedMeshRenders[i].sharedMesh;
            skinnedMeshRender.materials = _skinnedMeshRenders[i].materials;
            CreateBones(skinnedMeshRender, _skinnedMeshRenders[i]);            
        }

        _cloneObject.transform.localScale = transform.localScale;
    }

    private void CreateBones(SkinnedMeshRenderer clone, SkinnedMeshRenderer original)
    {
        int rootBoneIndex = 0;
        Transform[] cloneBones = new Transform[original.bones.Length];

        for (int i = 0; i < original.bones.Length; i++)
        {
            if(original.bones[i] == original.rootBone)
                rootBoneIndex = i;

            GameObject g = new GameObject(original.bones[i].name);
            SetBoneTransform(g.transform, original.bones[i]);
            g.transform.localScale = original.bones[i].lossyScale;

            cloneBones[i] = g.transform;
        }

        Transform rootBone = cloneBones[rootBoneIndex];
        rootBone.parent = _cloneObject.transform;
        foreach (var b in cloneBones)
            b.parent = rootBone;

        clone.rootBone = rootBone;
        clone.bones = cloneBones;
    }

    private void Update()
    {
        if (!_cloneObject.activeInHierarchy)
            return;

        for (int i = 0; i < _cloneRenderes.Length; i++)
        {
            for (int j = 0; j < _cloneRenderes[i].bones.Length; j++)
                SetBoneTransform(_cloneRenderes[i].bones[j], _skinnedMeshRenders[i].bones[j]);
        }
    }
    
    private void SetBoneTransform(Transform cloneBone, Transform originalBone)
    {
        Vector3 relativePos = transform.InverseTransformPoint(originalBone.position);
        cloneBone.position = _cloneObject.transform.TransformPoint(relativePos);

        Quaternion relativeRot = Quaternion.Inverse(originalBone.rotation) * transform.rotation;
        cloneBone.rotation = _cloneObject.transform.rotation * relativeRot;
    }
}