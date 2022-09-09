using System.Collections;
using UnityEngine;

public class ParticlesSpawner : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _playerRightHand;
    [SerializeField] private Transform _playerLeftHand;

    private GameObject _lastParticle;

    public void SpawnAtPlayerPosition(GameObject prefab)
    {
        SpawnAtPosition(_player.position, prefab);               
    }

    public void SpawnAtPlayerRightHand(GameObject prefab)
    {
        SpawnAtPosition(_playerRightHand.position, prefab);
    }

    public void SpawnAtPlayerLeftHand(GameObject prefab)
    {
        SpawnAtPosition(_playerLeftHand.position, prefab);
    }

    public void DestroyLastParticle()
    {
        if (_lastParticle == null)
            return;

        Destroy(_lastParticle);
    }

    private void SpawnAtPosition(Vector3 position, GameObject prefab)
    {
        _lastParticle = Instantiate(prefab, position, prefab.transform.rotation);
    }
}