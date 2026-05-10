using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Starting Chunk")]
    [SerializeField] private GameObject startingChunkPrefab;

    [Header("Chunks")]
    [SerializeField] private GameObject[] chunkPrefabs;
    [SerializeField] private int chunkPoolSize = 3;

    [Header("Streaming")]
    [SerializeField] private float spawnAhead = 80f;
    [SerializeField] private float recycleBehind = 20f;

    private Chunk _startingChunkTemplate;
    private Chunk[] _prefabTemplates;

    private readonly Dictionary<Chunk, ObjectPool<Chunk>> _pools = new();
    private readonly Dictionary<Chunk, Chunk> _instanceToPrefab = new();
    private readonly List<Chunk> _activeChunks = new();
    private readonly List<Chunk> _candidateBuffer = new();

    private float _spawnZ;
    private LaneMask _currentExit = LaneMask.All;

    void Awake()
    {
        if (startingChunkPrefab != null)
        {
            _startingChunkTemplate = startingChunkPrefab.GetComponent<Chunk>();

            if (_startingChunkTemplate == null)
            {
                Debug.LogError("LevelGenerator: Starting Chunk Prefab is missing a Chunk component.");
            }
            else
            {
                RegisterPool(_startingChunkTemplate);
            }
        }

        _prefabTemplates = new Chunk[chunkPrefabs.Length];

        for (int i = 0; i < chunkPrefabs.Length; i++)
        {
            Chunk template = chunkPrefabs[i].GetComponent<Chunk>();
            _prefabTemplates[i] = template;

            if (template == null)
            {
                Debug.LogError("LevelGenerator: One of the Chunk Prefabs is missing a Chunk component.");
                continue;
            }

            RegisterPool(template);
        }
    }

    void Start()
    {
        if (_startingChunkTemplate != null)
        {
            SpawnChunk(_startingChunkTemplate);
        }

        while (_spawnZ < spawnAhead)
        {
            SpawnNextChunk();
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        float scroll = GameManager.Instance.ScrollSpeed * Time.deltaTime;

        for (int i = 0; i < _activeChunks.Count; i++)
        {
            _activeChunks[i].transform.position += Vector3.back * scroll;
        }

        _spawnZ -= scroll;

        while (_spawnZ < spawnAhead)
        {
            SpawnNextChunk();
        }

        for (int i = _activeChunks.Count - 1; i >= 0; i--)
        {
            Chunk c = _activeChunks[i];

            if (c.transform.position.z + c.Length * 0.5f < -recycleBehind)
            {
                Recycle(i);
            }
        }
    }

    private void RegisterPool(Chunk template)
    {
        if (template == null) return;

        if (!_pools.ContainsKey(template))
        {
            _pools[template] = new ObjectPool<Chunk>(template, transform, chunkPoolSize);
        }
    }

    private void SpawnNextChunk()
    {
        Chunk prefab = PickNextChunk(_currentExit);

        if (prefab == null)
        {
            Debug.LogError("LevelGenerator: no chunk in the prefab list connects to the current exit state. " +
                           "Add a chunk whose Entry includes one of the open lanes.");
            return;
        }

        SpawnChunk(prefab);
    }

    private void SpawnChunk(Chunk prefab)
    {
        Chunk chunk = _pools[prefab].Get(transform);

        chunk.transform.SetPositionAndRotation(
            new Vector3(0f, 0f, _spawnZ + chunk.Length * 0.5f),
            Quaternion.identity
        );

        _activeChunks.Add(chunk);
        _instanceToPrefab[chunk] = prefab;

        _spawnZ += chunk.Length;
        _currentExit = chunk.Exit;
    }

    private Chunk PickNextChunk(LaneMask requiredOpen)
    {
        _candidateBuffer.Clear();

        for (int i = 0; i < _prefabTemplates.Length; i++)
        {
            Chunk t = _prefabTemplates[i];

            if (t != null && requiredOpen.ConnectsTo(t.Entry))
            {
                _candidateBuffer.Add(t);
            }
        }

        if (_candidateBuffer.Count == 0) return null;

        return _candidateBuffer[Random.Range(0, _candidateBuffer.Count)];
    }

    private void Recycle(int index)
    {
        Chunk chunk = _activeChunks[index];

        _pools[_instanceToPrefab[chunk]].Return(chunk);
        _instanceToPrefab.Remove(chunk);
        _activeChunks.RemoveAt(index);
    }
}