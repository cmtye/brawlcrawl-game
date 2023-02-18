using System.Collections;
using UnityEngine;

namespace Misc
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private Wave[] waves;
        [SerializeField] private float spawnDelay = 2f;
        [SerializeField] private float waveDelay = 5f;
        [SerializeField] private bool constantSpawn;
        [SerializeField] private Vector3 cameraOffset;
        [SerializeField] private GameObject lockEntry;
        [SerializeField] private GameObject lockExit;
        private Transform _playerReference;
        private bool _started;

        private int _waveIndex;
        private int _spawnIndex;
    
        private void StartArena()
        {
            _waveIndex = 0;
            GameManager.instance.SetCameraTarget(transform, cameraOffset, false);
            lockEntry.SetActive(true);
            lockExit.SetActive(true);
            _started = true;
            StartCoroutine(StartWavesRoutine());
        }

        private IEnumerator StartWavesRoutine()
        {
            while(_waveIndex < waves.Length)
            {
                if (constantSpawn) _waveIndex = 0;
                if (_waveIndex != 0) { yield return new WaitForSeconds(waveDelay); }

                yield return SpawnWaveRoutine();
                yield return new WaitForSeconds(waveDelay);
                _waveIndex++;
            }
            GameManager.instance.SetCameraTarget(GameManager.PlayerTransform, Vector3.zero, true);
            lockEntry.SetActive(false);
            lockExit.SetActive(false);
            Destroy(gameObject, 0.2f);
        }
    
        private IEnumerator SpawnWaveRoutine()
        {
            if(waves.Length == 0)
            {
                yield break;
            }

            if (_waveIndex >= waves.Length) yield break;
            var currentWave = waves[_waveIndex];
        
            while (_spawnIndex < currentWave.enemyPrefabs.Length)
            {
                var enemyPrefab = currentWave.enemyPrefabs[_spawnIndex];
                var spawnerTransform = transform;
                var spawnPoint = spawnerTransform.position;
                spawnPoint.y -= 2;
                Instantiate(enemyPrefab, spawnPoint, spawnerTransform.rotation);

                _spawnIndex++;
                yield return new WaitForSeconds(spawnDelay);
            }
        
            _spawnIndex = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_started) return;
            if (other.CompareTag("Player")) { StartArena(); }
        }
    }
}
