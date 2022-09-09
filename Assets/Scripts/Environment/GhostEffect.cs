using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEffect : MonoBehaviour
{
    [SerializeField] private Material ghostMaterial;

    private GameObject _ghostClone;
    private Vector3 _lastPosition = Vector3.zero;
    private Vector3 _distanceFromLast;

    private void Awake()
    {
        //Prepara um objeto simplificado de template para o efeito
        _ghostClone = new GameObject();
        _ghostClone.SetActive(false);
        _ghostClone.name = gameObject.name + " GhostTemplate";
        MeshFilter filter = _ghostClone.AddComponent<MeshFilter>();
        filter.mesh = GetComponent<MeshFilter>().mesh;
        MeshRenderer meshRenderer = _ghostClone.AddComponent<MeshRenderer>();
        meshRenderer.material = ghostMaterial;
    }

    IEnumerator Start()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.01f);
            CreateInstance();
        }
    }

    private void CreateInstance()
    {
        //Se nao se moveu o suficiente, nao criar efeito
        _distanceFromLast = _lastPosition - transform.position;
        if (_distanceFromLast.sqrMagnitude < 0.04f)
            return;

        _lastPosition = transform.position;
        GameObject g = Instantiate(_ghostClone, transform.position, transform.rotation);
        g.SetActive(true);
        Destroy(g.gameObject, 0.2f);
    }

    public void DisableGhost()
    {
        Destroy(_ghostClone);
        StopAllCoroutines();
        enabled = false;
    }
}