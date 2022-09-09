using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillInstance : MonoBehaviour
{
    public abstract void Init(Vector3 direction, Transform author);
}